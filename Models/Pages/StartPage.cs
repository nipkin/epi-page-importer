using System.ComponentModel.DataAnnotations;

namespace EpiPageImporter.Models.Pages
{
    [ContentType(
        DisplayName = "Start Page",
        GUID = "d5a8f1b2-1c7b-4e6f-9f12-123456789abc",
        Description = "The homepage of the website.")]
    public class StartPage : BasePageData
    {
        [Display(
            Name = "Heading",
            GroupName = SystemTabNames.Content,
            Order = 10)]
        public virtual string? Heading { get; set; }

        [Display(
            Name = "Intro",
            GroupName = SystemTabNames.Content,
            Order = 20)]
        public virtual XhtmlString? Intro { get; set; }
    }
}