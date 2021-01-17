using Microsoft.EntityFrameworkCore;
using TvMaze.Scraper.Abstractions.Entities;

namespace TvMaze.Scraper.Implementations
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<ShowEntity> Shows { get; set; }
        public DbSet<CastEntity> Casts { get; set; }
        public DbSet<PersonEntity> Persons { get; set; }
    }
}
