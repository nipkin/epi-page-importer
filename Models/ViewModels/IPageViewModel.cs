using EpiPageImporter.Models.Pages;

namespace EpiPageImporter.Models.ViewModels
{
    public interface IPageViewModel<out T> where T : BasePageData
    {
        T CurrentPage { get; }

        LayoutModel? Layout { get; set; }
    }
}