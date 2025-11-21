namespace EpiPageImporter.Business.Models
{
    public sealed class RecipesResponse
    {
        public List<RecipeDto>? Recipes { get; set; }
        public int Total { get; set; }
        public int Skip { get; set; }
        public int Limit { get; set; }
    }
}