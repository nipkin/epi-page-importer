using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Html;
using System.Text;

namespace EpiPageImporter.Business.Services
{
    public class MenuService(IContentRepository contentRepo, UrlResolver urlResolver, IPageRouteHelper pageRouteHelper)
    {
        private readonly IContentRepository _contentRepo = contentRepo;
        private readonly UrlResolver _urlResolver = urlResolver;
        private readonly IPageRouteHelper _pageRouteHelper = pageRouteHelper;

        public IHtmlContent RenderContentTree(ContentReference root, int maxDepth = 3)
        {
            var currentPage = _pageRouteHelper.Page;

            var sb = new StringBuilder();
            Build(root, currentPage, 0, maxDepth, sb);
            return new HtmlString(sb.ToString());
        }

        private void Build(ContentReference parent, PageData currentPage, int depth, int maxDepth, StringBuilder sb)
        {
            if (depth >= maxDepth) return;

            var children = _contentRepo.GetChildren<PageData>(parent);
            bool any = false;

            foreach (var child in children)
            {
                if (!any)
                {
                    sb.Append("<ul class=\"main-menu\">");
                    any = true;
                }

                var url = _urlResolver.GetUrl(child.ContentLink);
                var isCurrent = currentPage != null && child.ContentLink.CompareToIgnoreWorkID(currentPage.ContentLink);

                if(isCurrent)
                {
                    sb.Append("<li class=\"active\">");
                }
                else
                {
                    sb.Append("<li>");
                }
                   
                sb.AppendFormat("<a href=\"{0}\">{1}</a>",
                    url,
                    System.Net.WebUtility.HtmlEncode(child.Name));

         

                sb.Append("</li>");
            }

            if (any)
                sb.Append("</ul>");
        }
    }
}
