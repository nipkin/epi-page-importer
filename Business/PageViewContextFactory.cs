using EpiPageImporter.Business.Services;
using EpiPageImporter.Models.Pages;
using EpiPageImporter.Models.ViewModels;
using EPiServer.ServiceLocation;
using EPiServer.Web;

namespace EpiPageImporter.Business
{
    [ServiceConfiguration]
    public class PageViewContextFactory(
        IContentLoader contentLoader,
         MenuService menuHelper)
    {
        private readonly IContentLoader _contentLoader = contentLoader;
        private readonly MenuService _menuHelper = menuHelper;

        public virtual LayoutModel CreateLayoutModel(ContentReference currentContentLink, HttpContext httpContext)
        {
            var startPageContentLink = SiteDefinition.Current.StartPage;

            if (currentContentLink.CompareToIgnoreWorkID(startPageContentLink))
            {
                startPageContentLink = currentContentLink;
            }

            var startPage = _contentLoader.Get<StartPage>(startPageContentLink);

            return new LayoutModel
            {
                MainMenu = _menuHelper.RenderContentTree(startPage.ContentLink),
                StartPageLink = startPage.ContentLink
            };
        }
    }
}
