using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Api.Dtos.Barrios;
using Touch.Api.Dtos.Localidades;
using Touch.Api.Dtos.Provincias;
using Touch.Api.Dtos.Zonas;

namespace Touch.Api.Dtos.Sucursales
{
    public class PostSucursalDto
    {

        [JsonProperty("nombre")]
        public string Nombre { get; set; }

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

        [JsonProperty("idBarrio")]
        public long? IdBarrio { get; set; }

        [JsonProperty("idLocalidad")]
        public long? IdLocalidad { get; set; }

        [JsonProperty("idProvincia")]
        public long? IdProvincia { get; set; }
        [JsonProperty("zona")]
        public long? IdZona { get; set; }

        [JsonProperty("region")]
        public long? IdRegion { get; set; }

        [JsonProperty("grupo")]
        public long? IdGrupo { get; set; }
    }
}
