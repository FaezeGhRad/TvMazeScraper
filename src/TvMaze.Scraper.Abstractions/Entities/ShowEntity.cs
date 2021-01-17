using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TvMaze.Scraper.Abstractions.Entities
{
    public class ShowEntity : Entity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public IEnumerable<CastEntity> Cast { get; set; }
    }
}
