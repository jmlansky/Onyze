using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos
{
    public class PagedResponse<T>
    {
        public PagedResponse()
        { }

        public PagedResponse(int pageNumber, int pageSize, long totalRecords)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalRecords = totalRecords;
            TotalPages = Math.Ceiling((double)totalRecords / pageSize);
        }

        [JsonProperty("pageNumber")]
        public int PageNumber { get; set; }

        [JsonProperty("totalPages")]
        public double TotalPages { get; set; }

        [JsonProperty("pageRequest")]
        public int PageSize { get; set; }

        [JsonProperty("totalRecords")]
        public long TotalRecords { get; set; }

        [JsonProperty("list")]
        public List<T> List { get; set; } = new List<T>();
    }
}
