using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Domain.Common
{
    public class PaginationInfo
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }

    public class MetaData
    {
        public PaginationInfo Pagination { get; set; } = null!;
    }

    public class PagedResult<T> : Result<List<T>>
    {
        [System.Text.Json.Serialization.JsonPropertyOrder(4)]
        public MetaData Meta { get; set; }

        public PagedResult(List<T> data, int pageNumber, int pageSize, int totalRecords) : base(true, string.Empty, null!, data)
        {
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            Meta = new MetaData
            {
                Pagination = new PaginationInfo
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    TotalRecords = totalRecords,
                    HasPreviousPage = pageNumber > 1,
                    HasNextPage = pageNumber < totalPages
                }
            };
        }

        public PagedResult(List<T> data, int pageNumber, int pageSize, int totalRecords, string message) : base(true, message, null!, data)
        {
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            Meta = new MetaData
            {
                Pagination = new PaginationInfo
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    TotalRecords = totalRecords,
                    HasPreviousPage = pageNumber > 1,
                    HasNextPage = pageNumber < totalPages
                }
            };
        }

        // Factory method: Nhận vào List<T>
        public static PagedResult<T> Create(List<T> data, int totalRecords, int pageNumber, int pageSize)
        {
            return new PagedResult<T>(data, pageNumber, pageSize, totalRecords);
        }
    }
}
