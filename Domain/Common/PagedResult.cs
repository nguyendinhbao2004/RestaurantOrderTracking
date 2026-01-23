using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Domain.Common
{
    public class PagedResult<T> : Result<List<T>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }

        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        //Hai biến tiện ích (helper). Frontend chỉ cần check if (HasNextPage == false) thì ẩn nút "Next" đi.


        public PagedResult(List<T> data, int pageNumber, int pageSize, int totalRecords) : base(true, string.Empty, null, data)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalRecords = totalRecords;
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        }

        // Factory method: Nhận vào List<T>
        public static PagedResult<T> Create(List<T> data, int totalRecords, int pageNumber, int pageSize)
        {
            return new PagedResult<T>(data, pageNumber, pageSize, totalRecords);
        }
    }
}
