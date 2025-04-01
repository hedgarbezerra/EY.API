using System.ComponentModel.DataAnnotations;

namespace EY.Domain.Models;

public record PaginationInput([Required] int Index = 1, [Required] int Size = 10, [MaxLength(255)] string Query = "")
{
}

public class PaginatedList<T>
{
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

    public int PageIndex { get; }
    public int PreviousPageIndex { get; private set; }
    public int NextPageIndex { get; private set; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages { get; }
    public List<T> Data { get; private set; }
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;
}