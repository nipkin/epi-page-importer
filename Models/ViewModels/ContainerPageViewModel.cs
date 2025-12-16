using EpiPageImporter.Models.Pages;

namespace EpiPageImporter.Models.ViewModels
{
    public class ContainerPageViewModel(ContainerPage pageData) : PageViewModel<ContainerPage>(pageData)
    {
        public IEnumerable<RecipePage> Recipes { get; set; } = Array.Empty<RecipePage>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}
