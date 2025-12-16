using EpiPageImporter.Models.Pages;
using EpiPageImporter.Models.ViewModels;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EpiPageImporter.Business
{
    public class PageContextActionFilter : IResultFilter
    {
        private readonly PageViewContextFactory _contextFactory;

        public PageContextActionFilter(PageViewContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {

            var controller = context.Controller as Controller;
            var viewModel = controller?.ViewData.Model;

            if (viewModel is IPageViewModel<BasePageData> model)
            {
                var currentContentLink = context.HttpContext.GetContentLink();
                var layoutModel = model.Layout ?? _contextFactory.CreateLayoutModel(currentContentLink, context.HttpContext);

                if (context.Controller is IModifyLayout layoutController)
                {
                    layoutController.ModifyLayout(layoutModel);
                }

                model.Layout = layoutModel;
            }
        }
        public void OnResultExecuted(ResultExecutedContext context)
        {
        }
    }
}
