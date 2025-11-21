using EpiPageImporter.Models.Pages;
using EPiServer.Web.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace EpiPageImporter.Controllers
{
    public class RecipePageController : PageController<RecipePage>
    {
        public IActionResult Index(RecipePage currentPage)
        {
            return View(currentPage);
        }
    }
}