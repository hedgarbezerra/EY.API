using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Domain.Models
{
    public record PaginationInput([Required] int Index, [Required] int Size = 10, [MaxLength(255)] string Query = "")
    {
    }
    public class PaginatedList<T>
    {
        public int PageIndex { get; private set; }
        public int PreviousPageIndex { get; private set; }
        public int NextPageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get; private set; }
        public List<T> Data { get; private set; }
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public PaginatedList(List<T> paginatedData, int pageIndex, int pageSize, int count)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = count;
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
            NextPageIndex = HasNextPage ? pageIndex + 1 : 0;
            PreviousPageIndex = HasPreviousPage ? pageIndex - 1 : 0;
            Data = paginatedData;
        }
        public PaginatedList(IQueryable<T> source, int pageIndex, int pageSize = 10)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = source.Count();
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
            NextPageIndex = HasNextPage ? pageIndex + 1 : 0;
            PreviousPageIndex = HasPreviousPage ? pageIndex - 1 : 0;
            Data = source.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
        }
    }
}
