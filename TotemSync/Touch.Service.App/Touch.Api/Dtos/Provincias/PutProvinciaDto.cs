using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Provincias
{
    public class PutProvinciaDto
    {
        [JsonProperty("nombre", Required = Required.Always)]
        [MinLength(3)]
        public string Nombre { get; set; }
    }
}
