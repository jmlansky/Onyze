using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Promociones
{
    public interface IPromocionDto
    {
        public string Nombre { get; set; }
        public int IdTipo { get; set; }
        public long IdTipoItem { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }        
        public decimal PorcentajeDescuento { get; set; }       
        public int CantidadMinima { get; set; }
        public decimal MontoFijo { get; set; }
        public bool HastaAgotarStock { get; set; }


        public List<PostItemDePromocionDto> Items { get; set; }
        public DestinatariosDePromoDto Destinatarios { get; set; }
    }
}
