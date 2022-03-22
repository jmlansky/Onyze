using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Api.Dtos.Barrios;
using Touch.Api.Dtos.Localidades;
using Touch.Api.Dtos.Provincias;
using Touch.Api.Dtos.Sucursales;
using Touch.Api.Dtos.Usuarios;

namespace Touch.Api.Dtos.Cliente
{
    public class ClienteDto : ComunDto
    {


        [JsonProperty("calle")]
        public string Calle { get; set; }
        [JsonProperty("numero")]
        public string Numero { get; set; }
        [JsonProperty("piso")]
        public string Piso { get; set; }
        [JsonProperty("depto")]
        public string Depto { get; set; }
        [JsonProperty("telefono")]
        public string Telefono { get; set; }
        [JsonProperty("mail")]
        public string Mail { get; set; }
        [JsonProperty("nombre_referente")]
        public string NombreReferente { get; set; }
        [JsonProperty("cargo_referente")]
        public string CargoReferente { get; set; }
        [JsonProperty("codigo")]
        public string Codigo { get; set; }

        [JsonProperty("barrio")]
        public BarrioDto Barrio { get; set; }

        [JsonProperty("localidad")]
        public LocalidadDto Localidad { get; set; }

        [JsonProperty("provincia")]
        public ProvinciaDto Provincia { get; set; }

        [JsonProperty("sucursales")]
        public List<SucursalDto> Sucursales { get; set; } = new List<SucursalDto>();

        [JsonProperty("usuarios")]
        public List<GetUsuarioDto> Usuarios { get; set; } = new List<GetUsuarioDto>();
    }
}
