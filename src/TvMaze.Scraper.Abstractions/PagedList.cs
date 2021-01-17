using System.Collections.Generic;

namespace TvMaze.Scraper.Abstractions
{
    public class PagedList<T> : List<T>
    {
        public int TotalCount { get; }
        public int TotalPages { get; }
        public int PageSize { get; }
        public int PageIndex { get; }
        public bool IsLastPage => (PageIndex + 1) >= TotalPages;

        public PagedList(IEnumerable<T> source, int totalCount, int totalPages, int pageSize, int pageNumber)
        {
            AddRange(source);

            TotalCount = totalCount;
            TotalPages = totalPages;
            PageSize = pageSize;
            PageIndex = pageNumber;
        }
    }
}