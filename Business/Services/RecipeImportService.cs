using EpiPageImporter.Business.Mapping;
using EpiPageImporter.Business.Models.Dtos;
using EpiPageImporter.Models.Pages;
using EPiServer.DataAccess;
using EPiServer.Security;
using EPiServer.Web;
using Newtonsoft.Json;

namespace EpiPageImporter.Business.Services;

public class RecipeImportService(
    IContentRepository contentRepository,
    CuisineService cuisineService,
    RecipePageMapper mapper,
    IHttpClientFactory httpClientFactory,
    ILogger<RecipeImportService> logger)
{
    private readonly IContentRepository _contentRepository = contentRepository;
    private readonly CuisineService _cuisineService = cuisineService;
    private readonly RecipePageMapper _mapper = mapper;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ILogger<RecipeImportService> _logger = logger;

    private const int RecipeLimit = 20;

    public async Task<string> RunAsync(Func<bool> stopRequested)
    {
        IEnumerable<RecipeDto> recipes;
        try
        {
            recipes = await FetchRecipesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch recipes from external API.");
            return "Import failed: could not fetch recipes.";
        }

        if (!recipes.Any())
            return "No recipes found.";

        var startPage = _contentRepository.Get<StartPage>(SiteDefinition.Current.StartPage);

        int created = 0, updated = 0, failed = 0;

        foreach (var dto in recipes)
        {
            if (stopRequested()) break;

            try
            {
                var container = _cuisineService.GetOrCreateCuisineContainer(dto.Cuisine, startPage.ContentLink);
                var existing = FindExisting(dto.Id, container);

                if (existing != null)
                {
                    var clone = existing.CreateWritableClone() as RecipePage;
                    await _mapper.MapAsync(dto, clone!);
                    _contentRepository.Save(clone!, SaveAction.Publish, AccessLevel.NoAccess);
                    updated++;
                }
                else
                {
                    var page = _contentRepository.GetDefault<RecipePage>(container);
                    page.ExternalRecipeId = dto.Id;
                    await _mapper.MapAsync(dto, page);
                    _contentRepository.Save(page, SaveAction.Publish, AccessLevel.NoAccess);
                    created++;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to import recipe {RecipeId} ({RecipeName}).", dto.Id, dto.Name);
                failed++;
            }
        }

        return $"Recipes imported. Created: {created}, Updated: {updated}, Failed: {failed}.";
    }

    private async Task<IEnumerable<RecipeDto>> FetchRecipesAsync()
    {
        var client = _httpClientFactory.CreateClient();
        var json = await client.GetStringAsync($"https://dummyjson.com/recipes?limit={RecipeLimit}");

        return JsonConvert.DeserializeObject<RecipeListWrapper>(json)?.Recipes
               ?? Enumerable.Empty<RecipeDto>();
    }

    private RecipePage? FindExisting(int externalId, ContentReference parent)
        => _contentRepository
            .GetChildren<RecipePage>(parent)
            .FirstOrDefault(x => x.ExternalRecipeId == externalId);

    private class RecipeListWrapper
    {
        public List<RecipeDto>? Recipes { get; set; }
    }
}
