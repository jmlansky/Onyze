using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Touch.Api.Dtos.Sectores;

namespace Touch.Api.Dtos.Programaciones
{
    public class GetProgramacionesDto:ComunDto
    {
        [JsonProperty("items")]
        public List<ItemDeProgramacioGetDto> Items { get; set; } = new List<ItemDeProgramacioGetDto>();

        [JsonProperty("periodos")]
        public List<PeriodoDto> Periodos { get; set; } = new List<PeriodoDto>();

        [JsonProperty("destinatarios")]
        public GetDestinatarioDto Destinatarios { get; set; }

        [JsonProperty("activa")]
        public bool Activa { get; set; }
    }
}
