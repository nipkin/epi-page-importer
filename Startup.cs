using EpiPageImporter.Business;
using EpiPageImporter.Business.Mapping;
using EpiPageImporter.Business.Services;
using EPiServer.Cms.Shell;
using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Scheduler;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Mvc;

namespace EpiPageImporter;

public class Startup
{
    private readonly IWebHostEnvironment _webHostingEnvironment;

    public Startup(IWebHostEnvironment webHostingEnvironment)
    {
        _webHostingEnvironment = webHostingEnvironment;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        if (_webHostingEnvironment.IsDevelopment())
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(_webHostingEnvironment.ContentRootPath, "App_Data"));

            services.Configure<SchedulerOptions>(options => options.Enabled = false);
        }

        services
            .AddTransient<MenuService>()
            .AddScoped<SearchService>()
            .AddScoped<RecipeImportService>()
            .AddScoped<RecipePageMapper>()
            .AddScoped<CuisineService>()
            .AddScoped<RecipeImageService>()
            .Configure<MvcOptions>(options => options.Filters.Add<PageContextActionFilter>())
            .AddCmsAspNetIdentity<ApplicationUser>()
            .AddCms()
            .AddFind()
            .AddAdminUserRegistration()
            .AddEmbeddedLocalization<Startup>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapContent();
        });
    }
}
