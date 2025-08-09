using LocalSearcher.Api.Common.Interfaces;

namespace LocalSearcher.Api.Services;

public class IndexBackgroundService(IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = scopeFactory.CreateScope();
        var indexService = scope.ServiceProvider.GetRequiredService<IIndexService>();

        await indexService.BuildIndexAsync(stoppingToken);
    }
}