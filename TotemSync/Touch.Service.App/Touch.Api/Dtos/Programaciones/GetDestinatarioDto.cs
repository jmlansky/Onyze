using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Api.Dtos.Cliente;
using Touch.Api.Dtos.Localidades;
using Touch.Api.Dtos.Provincias;
using Touch.Api.Dtos.Regiones;
using Touch.Api.Dtos.Zonas;

namespace Touch.Api.Dtos.Programaciones
{
    public class GetDestinatarioDto
    {


        [JsonProperty("provincias")]
        public List<ProvinciaDto> Provincias { get; set; } = new List<ProvinciaDto>();

        [JsonProperty("zonas")]
        public List<ZonaDto> Zonas { get; set; } = new List<ZonaDto>();

        [JsonProperty("regiones")]
        public List<RegionDto> Regiones { get; set; } = new List<RegionDto>();

        [JsonProperty("localidades")]
        public List<LocalidadDto> Localidades { get; set; } = new List<LocalidadDto>();

        [JsonProperty("clientes")]
        public List<ClienteDto> Clientes { get; set; } = new List<ClienteDto>();


    }
}
