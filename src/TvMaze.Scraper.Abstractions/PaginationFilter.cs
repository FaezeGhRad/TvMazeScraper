namespace TvMaze.Scraper.Abstractions
{
    public class PaginationFilter
    {
        public int PageSize { get; }
        public int PageIndex { get; set; }

        public PaginationFilter()
        {
            PageIndex = 0;
            PageSize = 10;
        }

        public PaginationFilter(int pageNumber, int pageSize)
        {
            PageIndex = pageNumber < 0 ? 0 : pageNumber;
            PageSize = pageSize < 1 ? 10 : pageSize;
        }
    }
}
