using System;
using System.Threading;
using System.Threading.Tasks;

namespace TvMaze.Scraper.Abstractions.Services
{
    public interface ITvMazeSyncService : IDisposable
    {
        Task Synchronize(CancellationToken cancellationToken = default);
    }
}
