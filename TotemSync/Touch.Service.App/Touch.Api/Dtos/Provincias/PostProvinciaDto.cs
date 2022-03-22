using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Provincias
{
    public class PostProvinciaDto
    {
        [JsonProperty("nombre")]
        [MinLength(3)]
        public string Nombre{ get; set; }
    }
}
