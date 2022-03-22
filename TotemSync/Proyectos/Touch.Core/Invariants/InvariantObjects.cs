using System;
using System.Collections.Generic;
using System.Text;

namespace Touch.Core.Invariants
{
    public static class InvariantObjects
    {
        public static Dictionary<long, string> TiposDeArchivos { set; get; }

        public enum ServiceMethod 
        {
            Insert,
            Update,
            Delete
        }

        public enum ServiceMethodsStatusCode
        {
            Ok = 200,
            PartialContent = 206,
            Error = 400
        }

        public enum TiposDeItemsDePromocion
        {
            Articulos,
            Categorias,
            Fabricantes
        }
    }
}
