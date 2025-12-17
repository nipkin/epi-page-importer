using EpiPageImporter.Models.Pages;
using EpiPageImporter.Models.ViewModels;
using EPiServer.Web.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace EpiPageImporter.Controllers.Pages
{
    public class ContainerPageController(IContentLoader contentLoader) : PageController<ContainerPage>
    {
        private readonly IContentLoader _contentLoader = contentLoader ?? throw new ArgumentNullException(nameof(contentLoader));

        public IActionResult Index(ContainerPage currentPage, int page = 1)
        {
            if (currentPage == null) return NotFound();

            var pageSize = currentPage.PageSize <= 0 ? 10 : currentPage.PageSize;
            var pageIndex = Math.Max(1, page);

            var allRecipes = _contentLoader.GetChildren<RecipePage>(currentPage.ContentLink)
                                           .OrderBy(r => r.Name)
                                           .ToList();

            var totalItems = allRecipes.Count;
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var pagedRecipes = allRecipes
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new ContainerPageViewModel(currentPage)
            {
                CurrentPage = currentPage,
                Recipes = pagedRecipes,
                Page = pageIndex,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            return View(model);
        }
    }
}