using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Api.Dtos.Barrios;
using Touch.Api.Dtos.Localidades;

namespace Touch.Api.Dtos.Cliente
{
    public class PostClienteDto 
    {
        [JsonProperty("nombre")]
        public string Nombre { get; set; }

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

        [JsonProperty("idBarrio")]
        public long? IdBarrio { get; set; }

        [JsonProperty("idLocalidad")]
        public long? IdLocalidad { get; set; }

        [JsonProperty("idProvincia")]
        public long? IdProvincia { get; set; }
    }
}
