using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Atributos
{
    public class AtributosFiltradosDto
    {
        [JsonProperty("nombre")]
        public string Nombre { get; set; }
    }
}
