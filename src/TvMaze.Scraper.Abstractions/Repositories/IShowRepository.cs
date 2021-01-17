using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TvMaze.Scraper.Abstractions.Entities;

namespace TvMaze.Scraper.Abstractions.Repositories
{
    public interface IShowRepository
    {
        /// <summary>
        /// Returns a collection paginated shows based on the given filter
        /// </summary>
        /// <param name="filter">pagination filter</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns></returns>
        Task<PagedList<ShowEntity>> GetShows(PaginationFilter filter, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the last show
        /// </summary>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns></returns>
        Task<ShowEntity> GetLastShow(CancellationToken cancellationToken = default);

        /// <summary>
        /// Stores a collection shows to the given persisted page based on the given filter
        /// </summary>
        /// <param name="filter">pagination filter</param>
        /// <param name="shows">shows to save</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns></returns>
        Task SaveShows(PaginationFilter filter, IEnumerable<ShowEntity> shows, CancellationToken cancellationToken = default);
    }
}
