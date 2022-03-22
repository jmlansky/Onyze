using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Api.Dtos.Barrios;
using Touch.Api.Dtos.Cliente;
using Touch.Api.Dtos.Grupos;
using Touch.Api.Dtos.Localidades;
using Touch.Api.Dtos.Provincias;
using Touch.Api.Dtos.Sectores;
using Touch.Api.Dtos.Sucursales;
using Touch.Api.Dtos.Zonas;

namespace Touch.Api.Dtos.Totems
{
    public class TotemDto : ComunDto
    {

        [JsonProperty("serial")]
        public string Serial { get; set; }

        [JsonProperty("sucursal")]
        public SucursalDto Sucursal{ get; set; }

        [JsonProperty("sector")]
        public List<SectorDto> Sectores { get; set; } = new List<SectorDto>();
 
    }
}
