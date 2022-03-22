using Newtonsoft.Json;
using System;
using System.Text.Json.Serialization;

namespace Touch.Api.Dtos.Articulos
{
    public class AtributoDto: ComunDto
    {      
        [JsonProperty("tipoAtributo")]
        public TipoAtributoDto TipoAtributo { get; set; } = new TipoAtributoDto();
    }
}