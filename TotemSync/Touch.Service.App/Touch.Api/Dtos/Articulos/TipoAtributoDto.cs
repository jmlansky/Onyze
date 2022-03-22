using Newtonsoft.Json;
using System.Collections.Generic;
using Touch.Api.Dtos.TiposAtributo;

namespace Touch.Api.Dtos.Articulos
{
    public class TipoAtributoDto : ComunDto
    {
        public List<AtributoDelTipoDto> Atributos { get; set; } = new List<AtributoDelTipoDto>();
    }
}