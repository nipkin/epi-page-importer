using EpiPageImporter.Business.Models;
using EpiPageImporter.Models.Pages;
using EPiServer.Find;
using EPiServer.Find.Cms;
using EPiServer.Web.Routing;

namespace EpiPageImporter.Business.Services
{
    public class SearchService(IClient findClient, UrlResolver urlResolver)
    {
        private readonly IClient _findClient = findClient;
        private readonly UrlResolver _urlResolver = urlResolver;

        public async Task<IEnumerable<RecipeSearchHit>> SearchRecipes(string query, int maxResults = 8)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Enumerable.Empty<RecipeSearchHit>();

            var results = await _findClient
                .Search<RecipePage>()
                .For(query)
                .Filter(x => x.MatchType(typeof(RecipePage)))
                .FilterForVisitor()
                .Take(maxResults).GetContentResultAsync();

            return results.Select(x => new RecipeSearchHit
            {
                Title = x.RecipeName ?? x.Name,
                Url = _urlResolver.GetUrl(x.ContentLink)
            });
        }
    }
}
