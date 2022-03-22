﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Articulos
{
    public class GetRelacionesDto : ComunDto
    {
        [JsonProperty("relaciones")]
        public List<RelacionDto> Relaciones { get; set; }

    }
}
