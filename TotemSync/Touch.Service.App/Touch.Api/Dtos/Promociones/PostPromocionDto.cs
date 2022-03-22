using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Touch.Api.Dtos.Promociones
{
    public class PostPromocionDto : IPromocionDto
    {
        [JsonProperty("nombre", Required = Required.Always)]
        [MinLength(4)]
        public string Nombre { get; set; }

        [JsonProperty("fechaInicio", Required = Required.Always)]
        public DateTime FechaInicio { get; set; }

        [JsonProperty("fechaFin", Required = Required.Always)]
        public DateTime FechaFin { get; set; }

        [JsonProperty("hastaAgotarStock", Required = Required.Always)]
        public bool HastaAgotarStock { get; set; } = false;       


        [JsonProperty("porcentajeDescuento")]
        public decimal PorcentajeDescuento { get; set; }

        [JsonProperty("cantidadMinima")]
        public int CantidadMinima { get; set; }

        [JsonProperty("montoFijo")]
        public decimal MontoFijo { get; set; }


        [JsonProperty("idTipo", Required = Required.Always)]
        [Range(1, int.MaxValue, ErrorMessage = "Por favor ingrese un tipo de promoción válido")]
        public int IdTipo { get; set; }

        [JsonProperty("idTipoItem", Required = Required.Always)]
        public long IdTipoItem { get; set; }

        [JsonProperty("items")]
        public List<PostItemDePromocionDto> Items { get; set; }

        [JsonProperty("destinatarios")]
        public DestinatariosDePromoDto Destinatarios { get; set; }
    }
}
