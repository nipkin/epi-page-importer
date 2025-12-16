using EpiPageImporter.Models.Pages;
using EPiServer.Web.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace EpiPageImporter.Controllers.Pages
{
    public class StartPageController : PageController<StartPage>
    {
        public IActionResult Index(StartPage currentPage)
        {
            var viewModel = new Models.ViewModels.StartPageViewModel(currentPage);
            return View(viewModel);
        }
    }
}