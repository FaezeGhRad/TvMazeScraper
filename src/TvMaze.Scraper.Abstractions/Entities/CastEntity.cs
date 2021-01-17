using System.ComponentModel.DataAnnotations;

namespace TvMaze.Scraper.Abstractions.Entities
{
    public class CastEntity : Entity
    {
        [Required]
        public PersonEntity Person { get; set; }
        public int PersonId { get; set; }

        [Required]
        public ShowEntity Show { get; set; }
        public int ShowId { get; set; }
    }
}
