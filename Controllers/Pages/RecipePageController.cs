using EpiPageImporter.Models.Pages;
using EPiServer.Web.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace EpiPageImporter.Controllers.Pages
{
    public class RecipePageController : PageController<RecipePage>
    {
        public IActionResult Index(RecipePage currentPage)
        {
            var viewModel = new Models.ViewModels.RecipePageViewModel(currentPage);
            return View(viewModel);
        }
    }
}