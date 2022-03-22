using System;
using System.Collections.Generic;
using System.Text;

namespace Touch.Core.ResultadosPaginados
{
    public class ResultadoPaginado
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public long TotalRecords { get; set; }
    }
}
