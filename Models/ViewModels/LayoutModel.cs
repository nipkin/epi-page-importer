using Microsoft.AspNetCore.Html;

namespace EpiPageImporter.Models.ViewModels
{
    public class LayoutModel
    {
        public IHtmlContent? MainMenu { get; set; }
        public ContentReference? StartPageLink { get; set; }
    }
}
