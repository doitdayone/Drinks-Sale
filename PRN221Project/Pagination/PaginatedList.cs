using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Tracing;

namespace PRN221Project.Pagination
{
    public class PaginatedList<T> : List<T>
    {
        public int TotalPages { get; set; }
        public int PageIndex { get; set; }
        public PaginatedList(List<T> items, int sourceSize, int pageIndex, int pageSize)
        {
            TotalPages = (int)Math.Ceiling(sourceSize / (double)pageSize);
            PageIndex = pageIndex;
            this.AddRange(items);
        }

        public bool HasPreviousPage()
        {
            return PageIndex > 0;
        }

        public bool HasNextPage()
        {
            return PageIndex < TotalPages-1;
        }

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            int count = source.Count();
            List<T> items = await source.Skip(pageIndex*pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
