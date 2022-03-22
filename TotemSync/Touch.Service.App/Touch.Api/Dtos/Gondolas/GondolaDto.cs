using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Api.Dtos.CategoriasDeArticulo;
using Touch.Api.Dtos.Estantes;
using Touch.Api.Dtos.Grilla;

namespace Touch.Api.Dtos.Gondolas
{
    public class GondolaDto : ComunDto
    {
        [JsonProperty("titulo")]
        public string Titulo { get; set; }

        [JsonProperty("colorTitulo")]
        public string ColorTitulo { get; set; }

        [JsonProperty("colorFondo")]
        public string ColorFondo { get; set; }

        [JsonProperty("colorEncabezado")]
        public string ColorEncabezado { get; set; }

        [JsonProperty("idEncabezado")]
        public long IdEncabezado { get; set; }

        [JsonProperty("idFondo")]
        public long IdFondo { get; set; }

        [JsonProperty("imagen")]
        public DateTime Imagen { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("estantes")]
        public List<EstanteDto> Estantes { get; set; } = new List<EstanteDto>();

        [JsonProperty("grilla")]
        public GrillaDeGondolaDto Grilla { get; set; } = new GrillaDeGondolaDto();

        [JsonProperty("activo")]
        public bool Activo { get; set; }


        [JsonProperty("categoria")]
        public CategoriaDto Categoria { get; set; }
    }
}
