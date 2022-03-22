using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Sponsoreo
{
    public class PostSponsoreoDto
    {
        [JsonProperty("fechaInicio", Required = Required.Always)]
        [MinLength(10)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string FechaInicio { get; set; }

        [JsonProperty("fechaFin", Required = Required.Always)]
        [MinLength(10)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string FechaFin { get; set; }

        [JsonProperty("horaInicio", Required = Required.Always)]
        public string HoraInicio { get; set; }

        [JsonProperty("horaFin", Required= Required.Always)]
        public string HoraFin { get; set; }

        [JsonProperty("idArticulo")]
        [Range(1, long.MaxValue, ErrorMessage ="Por favor ingrese un id mayor a {1}.")]
        public long IdArticulo { get; set; }

        [JsonProperty("idFabricante")]
        public long IdFabricante { get; set; }
    }
}
