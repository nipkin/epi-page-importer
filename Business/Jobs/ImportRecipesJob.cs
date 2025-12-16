using EpiPageImporter.Business.Models.Dtos;
using EpiPageImporter.Models.Media;
using EpiPageImporter.Models.Pages;
using EPiServer.DataAccess;
using EPiServer.Framework.Blobs;
using EPiServer.PlugIn;
using EPiServer.Scheduler;
using EPiServer.Security;
using EPiServer.Web;
using Newtonsoft.Json;
using System.Net;
using System.Security.Cryptography;

namespace EpiPageImporter.Business.Jobs
{
    [ScheduledPlugIn(
        DisplayName = "Recipe Import Job",
        Description = "Imports recipes from dummyjson.com and creates a structure of containerpages and recipepages under the startpage")]
    public class RecipeImportJob : ScheduledJobBase
    {
        private bool _stopRequested = false;

        private readonly IContentRepository _contentRepository;
        private readonly IBlobFactory _blobFactory;
        private readonly ISiteDefinitionResolver _siteDefinitionResolver;
        private readonly CategoryRepository _categoryRepository;

        public RecipeImportJob(
            IContentRepository contentRepository,
            IBlobFactory blobFactory,
            ISiteDefinitionResolver siteDefinitionResolver, 
            CategoryRepository categoryRepository)
        {
            _contentRepository = contentRepository;
            _blobFactory = blobFactory;
            _siteDefinitionResolver = siteDefinitionResolver;
            _categoryRepository = categoryRepository;
        }

        public override void Stop()
        {
            _stopRequested = true;
        }

        public override string Execute()
        {
            string url = "https://dummyjson.com/recipes?limit=20";

            using var client = new HttpClient();
            var json = client.GetStringAsync(url).Result;

            var wrapper = JsonConvert.DeserializeObject<RecipeListWrapper>(json);
            if (wrapper?.Recipes == null)
                return "No recipes found.";

            var site = _siteDefinitionResolver.GetByHostname("localhost:5000", false);
            var startPageRef = site?.StartPage;
            var startPage = _contentRepository.Get<StartPage>(startPageRef);

            int created = 0;
            int updated = 0;

            foreach (var dto in wrapper.Recipes)
            {
                if (_stopRequested) break;

                var containerRef = GetOrCreateCuisineContainer(dto.Cuisine ?? "Other", startPage.ContentLink);
                var existing = FindExistingRecipePage(dto.Id, containerRef);

                if (existing != null)
                {
                    UpdateRecipePage(existing, dto);
                    updated++;
                }
                else
                {
 
                    CreateRecipePage(dto, containerRef, dto.Id);
                    created++;
                }
            }

            return $"Recipes imported. Created: {created}, Updated: {updated}.";
        }

        private RecipePage? FindExistingRecipePage(int externalId, ContentReference parent)
        {
            var children = _contentRepository.GetChildren<RecipePage>(parent);

            return children.FirstOrDefault(x => x.ExternalRecipeId == externalId);
        }

        private ContentReference CreateRecipePage(RecipeDto dto, ContentReference parent, int externalId)
        {
            var page = _contentRepository.GetDefault<RecipePage>(parent);

            page.ExternalRecipeId = externalId;

            MapData(dto, page, parent);

            return _contentRepository.Save(page, SaveAction.Publish, AccessLevel.NoAccess);
        }


        private void UpdateRecipePage(RecipePage existingPage, RecipeDto dto)
        {
            var page = existingPage.CreateWritableClone() as RecipePage;

            MapData(dto, page, existingPage.ParentLink);

            _contentRepository.Save(page, SaveAction.Publish, AccessLevel.NoAccess);
        }

        private void MapData(RecipeDto dto, RecipePage page, ContentReference parent)
        {
            page.Name = dto.Name ?? $"Recipe {dto.Id}";
            page.RecipeName = dto.Name;
            page.Ingredients = dto.Ingredients ?? new List<string>();
            page.Instructions = new XhtmlString(
                string.Join("<br/>", dto.Instructions ?? new List<string>())
            );
            page.PrepTimeMinutes = dto.PrepTimeMinutes;
            page.CookTimeMinutes = dto.CookTimeMinutes;
            page.Servings = dto.Servings;
            page.Difficulty = dto.Difficulty;
            page.Cuisine = dto.Cuisine;
            page.CaloriesPerServing = dto.CaloriesPerServing;
            page.Tags = dto.Tags ?? new List<string>();
            page.UserId = dto.UserId;
            page.Rating = dto.Rating;
            page.ReviewCount = dto.ReviewCount;
            page.MealType = dto.MealType ?? new List<string>();

            if (!string.IsNullOrEmpty(dto.Cuisine))
            {
                int cuisineCat = GetOrCreateCuisineCategory(dto.Cuisine);
                if (!page.Category.Contains(cuisineCat))
                    page.Category.Add(cuisineCat);
            }

            if (!string.IsNullOrEmpty(dto.Image))
            {
                page.Image = ImportImage(dto.Image, page.Image);
            }
        }

        private ContentReference? ImportImage(string url, ContentReference currentImage)
        {
            try
            {
                using var http = new HttpClient();
                byte[] newData = http.GetByteArrayAsync(url).Result;

                var existingData = ReadExistingImage(currentImage);

                if (existingData != null)
                {
                    var newHash = ComputeHash(newData);
                    var existingHash = ComputeHash(existingData);

                    if (newHash == existingHash)
                    {
                        return currentImage;
                    }
                }

                var folderRef = GetOrCreateRecipeImagesFolder();

                var img = _contentRepository.GetDefault<ImageFile>(folderRef);
                img.Name = Path.GetFileName(url);

                var blob = _blobFactory.CreateBlob(img.BinaryDataContainer, ".jpg");
                using var s = blob.OpenWrite();
                s.Write(newData, 0, newData.Length);

                img.BinaryData = blob;

                return _contentRepository.Save(img, SaveAction.Publish, AccessLevel.NoAccess);
            }
            catch
            {
                return null;
            }
        }

        private ContentReference GetOrCreateRecipeImagesFolder()
        {
            var assetsRoot = SiteDefinition.Current.GlobalAssetsRoot;

            var existing = _contentRepository
                .GetChildren<ContentFolder>(assetsRoot)
                .FirstOrDefault(x => x.Name == "Recipe Images");

            if (existing != null)
                return existing.ContentLink;

            var folder = _contentRepository.GetDefault<ContentFolder>(assetsRoot);
            folder.Name = "Recipe Images";

            return _contentRepository.Save(folder, SaveAction.Publish, AccessLevel.NoAccess);
        }

        private byte[]? ReadExistingImage(ContentReference imageRef)
        {
            if (ContentReference.IsNullOrEmpty(imageRef))
                return null;

            if (!_contentRepository.TryGet(imageRef, out ImageData img))
                return null;

            using var stream = img.BinaryData.OpenRead();
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }

        private string ComputeHash(byte[] data)
        {
            var hash = SHA256.HashData(data);
            return Convert.ToBase64String(hash);
        }

        private ContentReference GetOrCreateCuisineContainer(string cuisine, ContentReference startPageRef)
        {
            var existing = _contentRepository
                .GetChildren<ContainerPage>(startPageRef)
                .FirstOrDefault(x => x.Name.Equals(cuisine, StringComparison.OrdinalIgnoreCase));
            int cuisineCategoryId = GetOrCreateCuisineCategory(cuisine);

            if (existing != null)
            {
                if (existing.Category == null || !existing.Category.Contains(cuisineCategoryId))
                {
                    var clone = existing.CreateWritableClone() as ContainerPage;
                    clone.Category.Add(cuisineCategoryId);
                    _contentRepository.Save(clone, SaveAction.Publish, AccessLevel.NoAccess);
                }

                return existing.ContentLink;
            }

            var container = _contentRepository.GetDefault<ContainerPage>(startPageRef);
            container.Name = cuisine;
            container.Category.Add(cuisineCategoryId);

            return _contentRepository.Save(container, SaveAction.Publish, AccessLevel.NoAccess);
        }

        private int GetOrCreateCuisinesRootCategory()
        {
            var root = _categoryRepository.GetRoot();
            var cuisinesCat = root.Categories
                .FirstOrDefault(c => c.Name.Equals("Cuisines", StringComparison.OrdinalIgnoreCase));

            if (cuisinesCat != null)
                return cuisinesCat.ID;

            var newCat = new Category
            {
                Name = "Cuisines",
                Description = "Root category containing cuisine types",
                Selectable = false,
                Parent = root
            };

            _categoryRepository.Save(newCat);

            return newCat.ID;
        }

        private int GetOrCreateCuisineCategory(string cuisine)
        {
            if (string.IsNullOrWhiteSpace(cuisine))
                cuisine = "Other";

            int rootId = GetOrCreateCuisinesRootCategory();
            var root = _categoryRepository.Get(rootId);

            var existing = root.Categories
                .FirstOrDefault(c => c.Name.Equals(cuisine, StringComparison.OrdinalIgnoreCase));

            if (existing != null)
                return existing.ID;

            var newCat = new Category
            {
                Name = cuisine,
                Description = $"{cuisine} cuisine category",
                Selectable = true,
                Parent = root
            };

            _categoryRepository.Save(newCat);
            return newCat.ID;
        }

        private class RecipeListWrapper
        {
            public List<RecipeDto>? Recipes { get; set; }
        }
    }
}