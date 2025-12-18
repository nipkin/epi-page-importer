using EpiPageImporter.Business.Services;
using EPiServer.PlugIn;
using EPiServer.Scheduler;

namespace EpiPageImporter.Business.Jobs;

[ScheduledPlugIn(
    DisplayName = "Recipe Import Job",
    Description = "Imports recipes from dummyjson.com")]
public class RecipeImportJob(RecipeImportService importService) : ScheduledJobBase
{
    private readonly RecipeImportService _importService = importService;
    private bool _stopRequested;

    public override void Stop() => _stopRequested = true;

    public override string Execute()
    {
        return _importService.Run(() => _stopRequested);
    }
}
