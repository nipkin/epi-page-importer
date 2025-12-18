using EpiPageImporter.Models.Pages;
using EPiServer.DataAccess;
using EPiServer.Security;

namespace EpiPageImporter.Business.Services;

public class CuisineService(
    IContentRepository contentRepository,
    CategoryRepository categoryRepository)
{
    private readonly IContentRepository _contentRepository = contentRepository;
    private readonly CategoryRepository _categoryRepository = categoryRepository;

    public ContentReference GetOrCreateCuisineContainer(string cuisine, ContentReference startPage)
    {
        cuisine ??= "Other";
        int categoryId = GetOrCreateCuisineCategory(cuisine);

        var existing = _contentRepository
            .GetChildren<ContainerPage>(startPage)
            .FirstOrDefault(x => x.Name.Equals(cuisine, StringComparison.OrdinalIgnoreCase));

        if (existing != null)
            return existing.ContentLink;

        var container = _contentRepository.GetDefault<ContainerPage>(startPage);
        container.Name = cuisine;
        container.Category.Add(categoryId);

        return _contentRepository.Save(container, SaveAction.Publish, AccessLevel.NoAccess);
    }

    public void AssignCuisineCategory(RecipePage page, string? cuisine)
    {
        if (string.IsNullOrWhiteSpace(cuisine)) return;

        int id = GetOrCreateCuisineCategory(cuisine);
        if (!page.Category.Contains(id))
            page.Category.Add(id);
    }

    private int GetOrCreateCuisineCategory(string cuisine)
    {
        var root = _categoryRepository.GetRoot();
        var cuisines = root.Categories.FirstOrDefault(c => c.Name == "Cuisines")
            ?? CreateRoot(root);

        var existing = cuisines.Categories
            .FirstOrDefault(c => c.Name.Equals(cuisine, StringComparison.OrdinalIgnoreCase));

        if (existing != null) return existing.ID;

        var cat = new Category { Name = cuisine, Parent = cuisines, Selectable = true };
        _categoryRepository.Save(cat);
        return cat.ID;
    }

    private Category CreateRoot(Category root)
    {
        var cat = new Category { Name = "Cuisines", Parent = root, Selectable = false };
        _categoryRepository.Save(cat);
        return cat;
    }
}
