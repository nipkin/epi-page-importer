using EpiPageImporter.Business.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EpiPageImporter.Controllers.Api
{
    [ApiController]
    [Route("api/recipe-search")]
    public class SearchController(SearchService searchService) : ControllerBase
    {
        private readonly SearchService _searchService = searchService;

        [HttpGet]
        public async Task<IActionResult> Search(string q)
        {
            var results = await _searchService.SearchRecipes(q);
            return Ok(results);
        }
    }
}
