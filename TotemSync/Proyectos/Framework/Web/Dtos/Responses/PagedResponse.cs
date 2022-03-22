using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Framework.Comun.Dtos.Responses
{
    public class PagedResponse<T>
    {
        public PagedResponse()
        { }

        public PagedResponse(int pageNumber, int pageSize, long totalRecords, long? currentRecords = null)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalRecords = totalRecords;
            TotalPages = Math.Ceiling((double)totalRecords / pageSize);
            CurrentRecords = currentRecords ?? TotalRecords;
        }

        [JsonProperty("pageNumber")]
        public int PageNumber { get; set; }

        [JsonProperty("totalPages")]
        public double TotalPages { get; set; }

        [JsonProperty("pageRequest")]
        public int PageSize { get; set; }

        [JsonProperty("totalRecords")]
        public long TotalRecords { get; set; }

        [JsonProperty("currentRecords")]
        public long? CurrentRecords { get; set; }

        [JsonProperty("list")]
        public List<T> List { get; set; } = new List<T>();
    }
}
