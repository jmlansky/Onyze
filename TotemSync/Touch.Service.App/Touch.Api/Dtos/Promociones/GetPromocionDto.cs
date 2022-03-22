using System;
using System.Collections.Generic;

namespace Touch.Api.Dtos.Promociones
{
    public class GetPromocionDto: ComunDto
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool HastaAgotarStock { get; set; }

        public decimal PorcentajeDescuento { get; set; }
        public long CantidadMinima { get; set; }
        public decimal MontoDescuento { get; set; }

        public TipoPromocionDto TipoDePromocion { get; set; }

        public TipoItemDto TipoDeItem { get; set; } 
        public List<IGetItemDePromocionDto> Items { get; set; } = new List<IGetItemDePromocionDto>();

        public List<GetProvinciaDePromocionDto> Provincias { get; set; }
        public List<GetRegionDePromocionDto> Regiones { get; set; } 
        public List<GetClienteDePromocionDto> Clientes { get; set; }
        public List<GetGrupoDeClientesDto> Grupos { get; set; }
    }
}
