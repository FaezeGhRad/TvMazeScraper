using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TvMaze.Scraper.Abstractions;
using TvMaze.Scraper.Abstractions.Entities;
using TvMaze.Scraper.Abstractions.Repositories;

namespace TvMaze.Scraper.Implementations.Repositories
{
    public class ShowRepository : IShowRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ShowRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        ///<inheritdoc/>
        public async Task<PagedList<ShowEntity>> GetShows(PaginationFilter filter, CancellationToken cancellationToken = default)
        {
            IEnumerable<ShowEntity> shows = await _dbContext.Shows
                .AsNoTracking()
                .Include(s => s.Cast)
                .ThenInclude(c => c.Person)
                .Skip(filter.PageIndex * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync(cancellationToken);

            int totalCount = await _dbContext.Shows.CountAsync();
            int totalPage = (int)Math.Ceiling((double)totalCount / filter.PageSize);

            return new PagedList<ShowEntity>(shows, totalCount, totalPage, filter.PageSize, filter.PageIndex);
        }

        ///<inheritdoc/>
        public async Task<ShowEntity> GetLastShow(CancellationToken cancellationToken = default)
        {
            var count = await _dbContext.Shows.CountAsync(cancellationToken);
            if (count > 0)
            {
                return await _dbContext.Shows.AsNoTracking().OrderByDescending(s => s.Id).FirstAsync(cancellationToken);
            }
            return null;
        }

        ///<inheritdoc/>
        public async Task SaveShows(PaginationFilter filter, IEnumerable<ShowEntity> shows, CancellationToken cancellationToken = default)
        {
            if (shows == null)
            {
                throw new ArgumentNullException(nameof(shows));
            }

            List<int> showIds = shows.Select(s => s.Id).OrderBy(s => s).ToList();

            IEnumerable<ShowEntity> showsInDb = await _dbContext.Shows
                .Include(s => s.Cast)
                .ThenInclude(c => c.Person)
                .Skip(filter.PageIndex * filter.PageSize)
                .Take(filter.PageSize)
                .Where(s => s.Id >= filter.PageIndex && s.Id <= filter.PageSize - 1)
                .ToListAsync();

            foreach (ShowEntity show in shows)
            {
                if ((await _dbContext.Shows.FindAsync(show.Id)) != null)
                {
                    // show exists in database,  we can perform update if needed. but skip it for now!
                    continue;
                }

                // add new persons or load existing ones to avoid duplicate tracking/inserions
                foreach (CastEntity cast in show.Cast ?? Enumerable.Empty<CastEntity>())
                {
                    PersonEntity person = await _dbContext.Persons.FindAsync(cast.Person.Id);
                    if (person == null)
                    {
                        _dbContext.Persons.Add(cast.Person);
                        person = cast.Person;
                    }

                    cast.Person = person;
                    cast.PersonId = person.Id;
                }

                _dbContext.Shows.Add(show);
            }

            // remove deleted shows from database
            IEnumerable<ShowEntity> deletedShows = showsInDb.Where(s => !showIds.Contains(s.Id));
            foreach (ShowEntity deletedShow in deletedShows)
            {
                _dbContext.Shows.Remove(deletedShow);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
