using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TvMaze.Scraper.Abstractions;
using TvMaze.Scraper.Abstractions.Entities;
using TvMaze.Scraper.Abstractions.Repositories;
using TvMaze.Scraper.API.Models;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]

namespace TvMaze.Scraper.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShowsController : ControllerBase
    {
        private readonly IShowRepository _showRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ShowsController> _logger;

        public ShowsController(IShowRepository showRepository, IMapper mapper, ILogger<ShowsController> logger)
        {
            _showRepository = showRepository ?? throw new ArgumentNullException(nameof(showRepository));
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShowDto>>> Get(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting TV shows : page '{page}'", page);

            List<ShowEntity> showEntities = await _showRepository.GetShows(new PaginationFilter(page, pageSize), cancellationToken);

            showEntities.ForEach(i => i.Cast = i.Cast.OrderByDescending(c => c.Person.Birthday));

            IEnumerable<ShowDto> shows = _mapper.Map<IEnumerable<ShowDto>>(showEntities);

            return Ok(shows);
        }
    }
}
