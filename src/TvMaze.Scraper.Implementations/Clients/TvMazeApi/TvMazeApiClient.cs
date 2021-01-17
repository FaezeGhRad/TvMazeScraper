using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TvMaze.Scraper.Abstractions;
using TvMaze.Scraper.Abstractions.Entities;
using TvMaze.Scraper.Implementations.Clients.TvMazeApi.Models;

namespace TvMaze.Scraper.Implementations.Clients.TvMazeApi
{
    public class TvMazeApiClient : ITvMazeApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TvMazeApiClient> _logger;

        public TvMazeApiClient(HttpClient httpClient, ILogger<TvMazeApiClient> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger;
        }

        public async Task<IEnumerable<ShowEntity>> GetShowsWithCast(PaginationFilter filter, CancellationToken cancellationToken = default)
        {
            string showsUri = $"/shows?page={filter.PageIndex}";

            HttpResponseMessage response = await GetAsync(showsUri, cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // no more pages , previous page was the last page
                return null;
            }

            response.EnsureSuccessStatusCode();

            string responseJson = await response.Content.ReadAsStringAsync();

            IList<ShowEntity> shows = JsonConvert.DeserializeObject<IList<ShowApiModel>>(responseJson)
                .Select(s => new ShowEntity { Id = s.Id, Name = s.Name })?.ToList();

            foreach (ShowEntity show in shows)
            {
                string castUri = $"/shows/{show.Id}/cast";

                show.Cast = (await GetAsync<IList<CastApiModel>>(castUri, cancellationToken))
                    .Select(c => new CastEntity { Person = new PersonEntity { Id = c.Person.Id, Name = c.Person.Name, Birthday = c.Person.Birthday } }).ToList();
            }

            return shows;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        protected virtual async Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken = default)
        {
            int retryCount = 0;
            HttpResponseMessage response;

            do
            {
                response = await _httpClient.GetAsync(requestUri, cancellationToken);

                // Check if exceeded the rate limit - back off for some time
                if (response.StatusCode == (System.Net.HttpStatusCode)429)
                {
                    // TODO: max retry could be configurable
                    if (retryCount < 60)
                    {
                        retryCount++;

                        TimeSpan backoffDuration = TimeSpan.FromSeconds(retryCount / 2);

                        _logger.LogWarning("Failed to get '{requestUri}' - Server is busy , retrying in '{backoffDuration}' seconds.", requestUri, backoffDuration.TotalSeconds);

                        await Task.Delay(backoffDuration, cancellationToken);
                        continue;
                    }
                }

                return response;
            }
            while (true);
        }

        protected async Task<T> GetAsync<T>(string requestUri, CancellationToken cancellationToken = default) where T : class
        {
            if (string.IsNullOrWhiteSpace(requestUri))
            {
                throw new ArgumentNullException(nameof(requestUri));
            }

            HttpResponseMessage response = await GetAsync(requestUri, cancellationToken);

            response.EnsureSuccessStatusCode();

            string responseJson = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(responseJson);
        }
    }
}
