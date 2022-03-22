using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Api.Dtos.Barrios;
using Touch.Api.Dtos.Localidades;
using Touch.Api.Dtos.Provincias;
using Touch.Api.Dtos.Zonas;

namespace Touch.Api.Dtos.Totems
{
    public class PutTotemDto
    {
        [JsonProperty("nombre")]
        public string Nombre { get; set; }

        [JsonProperty("serial")]
        public string Serial { get; set; }

        [JsonProperty("idSucursal")]
        public long IdSucursal { get; set; }

        [JsonProperty("sectores")]
        public List<long> IdSectores { get; set; }

    }
}
