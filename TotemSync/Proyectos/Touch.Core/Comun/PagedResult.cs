using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Touch.Core.Comun
{
    public class PagedResult
    {
        public PagedResult()
        { }
        public PagedResult(int pageNumber, int pageSize, long totalRecords)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalRecords = totalRecords;
            TotalPages = Math.Ceiling((double)totalRecords / pageSize);
        }

        public IPagedList PagedList { get; set; }
        public int PageNumber { get; set; }
        public double TotalPages { get; set; }
        public int PageSize { get; set; }
        public long TotalRecords { get; set; }
    }
}
