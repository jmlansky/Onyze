using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Promociones
{
    public class GetPromocionCategoriaDto: IGetItemDePromocionDto
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public decimal PrecioAnterior { get => 0; set => throw new NotImplementedException(); }
        public decimal PrecioActual { get => 0; set => throw new NotImplementedException(); }
    }
}
