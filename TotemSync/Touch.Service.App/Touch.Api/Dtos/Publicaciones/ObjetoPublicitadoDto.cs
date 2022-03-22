using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Api.Dtos.TiposObjetoPublicitar;

namespace Touch.Api.Dtos.Publicaciones
{
    public class ObjetoPublicitadoDto
    {
        [JsonProperty("tipo")]
        public TipoObjetoPublicitarDto Tipo { get; set; }

        [JsonProperty("objeto")]
        public ComunDto Objeto { get; set; }
    }
}
