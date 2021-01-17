using System.Collections.Generic;

namespace TvMaze.Scraper.API.Models
{
    public class ShowDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<CastDto> Cast { get; set; }
    }
}
