using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Html;
using System.Text;

namespace EpiPageImporter.Business.Services
{
    public class MenuService(IContentRepository contentRepo, UrlResolver urlResolver)
    {
        private readonly IContentRepository _contentRepo = contentRepo;
        private readonly UrlResolver _urlResolver = urlResolver;

        public IHtmlContent RenderContentTree(ContentReference root, int maxDepth = 3)
        {
            var sb = new StringBuilder();
            Build(root, 0, maxDepth, sb);
            return new HtmlString(sb.ToString());
        }

        private void Build(ContentReference parent, int depth, int maxDepth, StringBuilder sb)
        {
            if (depth >= maxDepth) return;

            var children = _contentRepo.GetChildren<PageData>(parent);
            bool any = false;

            foreach (var child in children)
            {
                if (!any)
                {
                    sb.Append("<ul>");
                    any = true;
                }

                var url = _urlResolver.GetUrl(child.ContentLink);

                sb.Append("<li>");
                sb.AppendFormat("<a href=\"{0}\">{1}</a>",
                    url,
                    System.Net.WebUtility.HtmlEncode(child.Name));

                Build(child.ContentLink, depth + 1, maxDepth, sb);

                sb.Append("</li>");
            }

            if (any)
                sb.Append("</ul>");
        }
    }
}
