using EpiPageImporter.Models.Pages;
using EpiPageImporter.Models.ViewModels;
using EPiServer.ServiceLocation;
using EPiServer.Web;

namespace EpiPageImporter.Infrastructure
{
    [ServiceConfiguration]
    public class PageViewContextFactory(
        IContentLoader contentLoader,
        NavigationRenderer navigationRenderer)
    {
        private readonly IContentLoader _contentLoader = contentLoader;
        private readonly NavigationRenderer _navigationRenderer = navigationRenderer;

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
                MainMenu = _navigationRenderer.RenderContentTree(startPage.ContentLink),
                StartPageLink = startPage.ContentLink
            };
        }
    }
}
