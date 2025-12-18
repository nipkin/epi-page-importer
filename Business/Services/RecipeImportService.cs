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
    ISiteDefinitionResolver siteResolver,
    CuisineService cuisineService,
    RecipePageMapper mapper)
{
    private readonly IContentRepository _contentRepository = contentRepository;
    private readonly ISiteDefinitionResolver _siteResolver = siteResolver;
    private readonly CuisineService _cuisineService = cuisineService;
    private readonly RecipePageMapper _mapper = mapper;

    public string Run(Func<bool> stopRequested)
    {
        var recipes = FetchRecipes();
        if (!recipes.Any())
            return "No recipes found.";

        var site = _siteResolver.GetByHostname("localhost:5000", false);
        var startPage = _contentRepository.Get<StartPage>(site.StartPage);

        int created = 0, updated = 0;

        foreach (var dto in recipes)
        {
            if (stopRequested()) break;

            var container = _cuisineService.GetOrCreateCuisineContainer(dto.Cuisine, startPage.ContentLink);
            var existing = FindExisting(dto.Id, container);

            if (existing != null)
            {
                var clone = existing.CreateWritableClone() as RecipePage;
                _mapper.Map(dto, clone);
                _contentRepository.Save(clone, SaveAction.Publish, AccessLevel.NoAccess);
                updated++;
            }
            else
            {
                var page = _contentRepository.GetDefault<RecipePage>(container);
                page.ExternalRecipeId = dto.Id;
                _mapper.Map(dto, page);
                _contentRepository.Save(page, SaveAction.Publish, AccessLevel.NoAccess);
                created++;
            }
        }

        return $"Recipes imported. Created: {created}, Updated: {updated}.";
    }

    private IEnumerable<RecipeDto> FetchRecipes()
    {
        using var client = new HttpClient();
        var json = client.GetStringAsync("https://dummyjson.com/recipes?limit=20").Result;

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
