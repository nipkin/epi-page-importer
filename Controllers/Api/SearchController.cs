using EpiPageImporter.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace EpiPageImporter.Controllers.Api
{
    [ApiController]
    [Route("api/recipe-search")]
    public class SearchController(SearchService searchService) : ControllerBase
    {
        private readonly SearchService _searchService = searchService;

        private const int MaxQueryLength = 200;

        [HttpGet]
        public async Task<IActionResult> Search(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return BadRequest("Query must not be empty.");

            if (q.Length > MaxQueryLength)
                return BadRequest($"Query must not exceed {MaxQueryLength} characters.");

            var results = await _searchService.SearchRecipes(q);
            return Ok(results);
        }
    }
}
