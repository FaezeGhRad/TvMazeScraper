using System;

namespace TvMaze.Scraper.Implementations.Clients.TvMazeApi.Models
{
    public class PersonApiModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? Birthday { get; set; }
    }
}
