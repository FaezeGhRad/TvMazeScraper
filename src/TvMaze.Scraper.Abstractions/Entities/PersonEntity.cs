using System;
using System.ComponentModel.DataAnnotations;

namespace TvMaze.Scraper.Abstractions.Entities
{
    public class PersonEntity : Entity
    {
        [Required]
        public string Name { get; set; }
        public DateTime? Birthday { get; set; }
    }
}
