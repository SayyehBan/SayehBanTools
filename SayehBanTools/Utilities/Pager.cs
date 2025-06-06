﻿namespace SayehBanTools.Utilities;
/// <summary>
/// صفحه بندی
/// </summary>
public class Pager
{
    /// <summary>
    /// این کلاس برای صفحه بندی استفاده میشود
    /// </summary>
    /// <param name="totalItems"></param>
    /// <param name="currentPage"></param>
    /// <param name="pageSize"></param>
    /// <param name="maxPages"></param>
    public Pager(long totalItems, int currentPage = 1, int pageSize = 10, int maxPages = 5)
    {
        // calculate total pages
        var totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);

        // ensure current page isn't out of range
        if (currentPage < 1)
        {
            currentPage = 1;
        }
        else if (currentPage > totalPages)
        {
            currentPage = totalPages;
        }

        int startPage, endPage;
        if (totalPages <= maxPages)
        {
            // total pages less than max so show all pages
            startPage = 1;
            endPage = totalPages;
        }
        else
        {
            // total pages more than max so calculate start and end pages
            var maxPagesBeforeCurrentPage = (int)Math.Floor((decimal)maxPages / (decimal)2);
            var maxPagesAfterCurrentPage = (int)Math.Ceiling((decimal)maxPages / (decimal)2) - 1;
            if (currentPage <= maxPagesBeforeCurrentPage)
            {
                // current page near the start
                startPage = 1;
                endPage = maxPages;
            }
            else if (currentPage + maxPagesAfterCurrentPage >= totalPages)
            {
                // current page near the end
                startPage = totalPages - maxPages + 1;
                endPage = totalPages;
            }
            else
            {
                // current page somewhere in the middle
                startPage = currentPage - maxPagesBeforeCurrentPage;
                endPage = currentPage + maxPagesAfterCurrentPage;
            }
        }
        // calculate start and end item indexes
        var startIndex = (currentPage - 1) * pageSize;
        var endIndex = Math.Min(startIndex + pageSize - 1, totalItems - 1);

        // create an array of pages that can be looped over
        var pages = Enumerable.Range(startPage, (endPage + 1) - startPage);


        TotalPages = totalPages;
        Pages = pages;
    }
    /// <summary>
    /// صفحه هات
    /// </summary>
    public IEnumerable<int> Pages { get; private set; }
    /// <summary>
    /// تعداد کل صفحه
    /// </summary>
    public int TotalPages { get; private set; }
}
/*
 طریقه استفاده از کلاس
public class PaginatedItemsDto<TEntity> where TEntity : class
{
    public PaginatedItemsDto(int pageIndex, int pageSize, long count, IEnumerable<TEntity> data)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        Count = count;
        Data = data;
        Pager = new Pager(count, 1, pageIndex, pageSize);
    }
    public int PageIndex { get; private set; }
    public int PageSize { get; private set; }
    public long Count { get; private set; }
    public IEnumerable<TEntity> Data { get; private set; }
    public Pager Pager { get; private set; }
    public bool HasPreviousPage
    {
        get
        {
            return (PageIndex > 1);
        }
    }

    public bool HasNextPage
    {
        get
        {
            return (PageIndex < Pager.TotalPages);
        }
    }
}
 */