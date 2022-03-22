using System;

namespace Touch.Service.Promociones
{
    internal class TipoPromocionAttribute : Attribute
    {
        private string v;

        public TipoPromocionAttribute(string v)
        {
            this.v = v;
        }
    }
}