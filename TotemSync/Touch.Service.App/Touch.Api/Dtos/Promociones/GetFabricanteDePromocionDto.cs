﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Promociones
{
    public class GetFabricanteDePromocionDto: IGetItemDePromocionDto
    {
        public long Id { get; set;}
        public string Nombre { get; set;}
        public decimal PrecioAnterior { get; set;}
        public decimal PrecioActual { get; set;}
    }
}
