using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Api.Dtos.TiposMultimedia;
using Touch.Api.Dtos.TiposObjetoPublicitar;

namespace Touch.Api.Dtos.Playlists
{
    public class MultimediaDto
    {
        [JsonProperty("tipo")]
        public TipoMultimediaDto Tipo { get; set; }

        [JsonProperty("objeto")]
        public ComunDto Objeto { get; set; }

        [JsonProperty("tiempo")]
        public int  Tiempo { get; set; }
    }
}
