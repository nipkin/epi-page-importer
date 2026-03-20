using EpiPageImporter.Models.ViewModels;

namespace EpiPageImporter.Infrastructure
{
    internal interface IModifyLayout
    {
        void ModifyLayout(LayoutModel layoutModel);
    }
}
