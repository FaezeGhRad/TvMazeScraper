using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TvMaze.Scraper.Abstractions;
using TvMaze.Scraper.Abstractions.Entities;

namespace TvMaze.Scraper.Implementations.Clients.TvMazeApi
{
    public interface ITvMazeApiClient : IDisposable
    {
        Task<IEnumerable<ShowEntity>> GetShowsWithCast(PaginationFilter filter, CancellationToken cancellationToken = default);
    }
}
