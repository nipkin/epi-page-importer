using System.ComponentModel.DataAnnotations;

namespace EpiPageImporter.Models.Pages
{
    [ContentType(
        DisplayName = "Recipe Listing Page",
        GUID = "2f7c9d6a-8e4b-4c2b-9a6f-1a4d5e7b3c20",
        Description = "A listing page that contains RecipePage children.")]
    [AvailableContentTypes(Include = new[] { typeof(RecipePage) })]
    public class ContainerPage : BasePageData
    {
        [Display(
            Name = "Intro",
            Description = "Short introduction shown above the recipe listing.",
            GroupName = SystemTabNames.Content,
            Order = 10)]
        [CultureSpecific]
        public virtual string? Intro { get; set; }

        [Display(
            Name = "Page size",
            Description = "Number of recipes to show per page (used by the listing controller/view).",
            GroupName = SystemTabNames.Content,
            Order = 20)]
        public virtual int PageSize { get; set; } = 10;
    }
}