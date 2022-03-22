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
using Touch.Api.Dtos.Regiones;
using Touch.Api.Dtos.Zonas;

namespace Touch.Api.Dtos.Sucursales
{
    public class SucursalDto : ComunDto
    {

        [JsonProperty("idCliente")]
        public long IdCliente { get; set; }

        [JsonProperty("calle")]
        public string Calle { get; set; }

        [JsonProperty("altura")]
        public string Altura { get; set; }

        [JsonProperty("codigoPostal")]
        public string CodigoPostal { get; set; }

        [JsonProperty("telefono")]
        public string Telefono { get; set; }

        [JsonProperty("nombreReferente")]
        public string NombreReferente { get; set; }

        [JsonProperty("telefonoReferente")]
        public string TelefonoReferente { get; set; }

        [JsonProperty("cargoReferente")]
        public string CargoReferente { get; set; }

        [JsonProperty("casaCentral")]
        public bool CasaCentral { get; set; }

        [JsonProperty("barrio")]
        public BarrioDto Barrio { get; set; }
        [JsonProperty("localidad")]
        public LocalidadDto Localidad { get; set; }
        [JsonProperty("provincia")]
        public ProvinciaDto Provincia { get; set; }
        [JsonProperty("cliente")]
        public ClienteDto Cliente { get; set; }
        [JsonProperty("zona")]
        public ZonaDto Zona { get; set; }

        [JsonProperty("grupo")]
        public GrupoDto Grupo { get; set; }

        [JsonProperty("region")]
        public RegionDto Region { get; internal set; }
    }
}
