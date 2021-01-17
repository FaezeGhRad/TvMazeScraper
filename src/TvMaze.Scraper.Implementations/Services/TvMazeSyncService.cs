using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TvMaze.Scraper.Abstractions;
using TvMaze.Scraper.Abstractions.Entities;
using TvMaze.Scraper.Abstractions.Repositories;
using TvMaze.Scraper.Abstractions.Services;
using TvMaze.Scraper.Implementations.Clients.TvMazeApi;

namespace TvMaze.Scraper.Implementations.Services
{
    public class TvMazeSyncService : ITvMazeSyncService
    {
        private readonly ITvMazeApiClient _apiClient;
        private readonly IShowRepository _repository;
        private readonly ILogger<TvMazeSyncService> _logger;

        private static readonly object _lockObject = new object();

        private static bool _initialized;
        private static bool Initialized
        {
            get
            {
                lock (_lockObject)
                {
                    return _initialized;
                }
            }

            set
            {
                lock (_lockObject)
                {
                    _initialized = value;
                }
            }
        }

        public TvMazeSyncService(ITvMazeApiClient apiClient, IShowRepository showRepository, ILogger<TvMazeSyncService> logger)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            _repository = showRepository ?? throw new ArgumentNullException(nameof(showRepository));
            _logger = logger;
        }

        public async Task Synchronize(CancellationToken cancellationToken = default)
        {
            int nextPageIndex = 0;
            int pageSize = 250; // should be configurable!

            // Initialized's value indicates if all shows were already successfully fetched and stored (for the first time)
            // if not initialized we continue from were we left, otherwise we sync again from the beginning.
            if (!Initialized)
            {
                ShowEntity lastPersistedShow = await _repository.GetLastShow(cancellationToken);
                nextPageIndex = lastPersistedShow == null ? 0 : (int)Math.Floor((decimal)(lastPersistedShow.Id / pageSize)) + 1;
            }

            do
            {
                _logger.LogInformation("Start synchronizing page: {page}", nextPageIndex);

                IEnumerable<ShowEntity> showsWithCast = await _apiClient.GetShowsWithCast(new PaginationFilter(nextPageIndex, pageSize), cancellationToken);

                if (showsWithCast == null)
                {
                    // no more pages, previous page was the last page
                    _logger.LogInformation("Page {page} doesn't exist", nextPageIndex);
                    break;
                }

                await _repository.SaveShows(new PaginationFilter(nextPageIndex, pageSize), showsWithCast, cancellationToken);

                _logger.LogInformation("Successfully synchronized page: {page} ", nextPageIndex);

                nextPageIndex++;

            } while (true);

            Initialized = true;

            _logger.LogInformation("Successfully synchronized all {page} pages", nextPageIndex);
        }

        public void Dispose()
        {
            _apiClient?.Dispose();
        }
    }
}
