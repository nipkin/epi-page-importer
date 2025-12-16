using EpiPageImporter.Models.Pages;

namespace EpiPageImporter.Models.ViewModels
{
    public class PageViewModel<T>(T pageData) : IPageViewModel<T> where T : BasePageData
    {
        public T CurrentPage { get; set; } = pageData;
        public LayoutModel? Layout { get; set; }
    }
}
