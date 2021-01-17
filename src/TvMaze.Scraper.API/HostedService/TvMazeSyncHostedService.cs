using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TvMaze.Scraper.Abstractions.Services;
using TvMaze.Scraper.Implementations;

namespace TvMaze.Scraper.API.HostedService
{
    public class TvMazeSyncHostedService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<TvMazeSyncHostedService> _logger;

        public TvMazeSyncHostedService(IServiceProvider services, ILogger<TvMazeSyncHostedService> logger)
        {
            _services = services;
            _logger = logger;
        }

        public async override Task StartAsync(CancellationToken cancellationToken)
        {
            using (IServiceScope scope = _services.CreateScope())
            {
                using (ApplicationDbContext applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                {
                    await applicationDbContext.Database.MigrateAsync();
                }
            }

            await base.StartAsync(cancellationToken);
        }

        protected async override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await Task.Yield();            
            while (!cancellationToken.IsCancellationRequested)
            {
                await Synchronize(cancellationToken);

                await Task.Delay(TimeSpan.FromHours(1), cancellationToken);
            }
        }

        private async Task Synchronize(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Synchronizing TvMaze shows starts.");

                using (IServiceScope scope = _services.CreateScope())
                {
                    ITvMazeSyncService tvMazeSyncService = scope.ServiceProvider.GetRequiredService<ITvMazeSyncService>();

                    await tvMazeSyncService.Synchronize(cancellationToken);
                }

                _logger.LogInformation("Synchronizing TvMaze shows ends.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to synchronize");
                throw;
            }
        }
    }
}
