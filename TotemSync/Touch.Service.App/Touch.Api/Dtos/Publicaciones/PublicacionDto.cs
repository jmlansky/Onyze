using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Api.Dtos.Archivos;
using Touch.Api.Dtos.Cliente;

namespace Touch.Api.Dtos.Publicaciones
{
    public class PublicacionDto: ComunDto
    {
        [JsonProperty("archivo")]
        public ArchivoDto Archivo { get; set; }

        [JsonProperty("cliente")]
        public ClienteDto Cliente { get; set; }

        [JsonProperty("activo")]
        public bool Activo{ get; set; }

        [JsonProperty("objetosPublicitado")]
        public List<ObjetoPublicitadoDto> ObjetosPublicitados { get; set; } = new List<ObjetoPublicitadoDto>();
    }
}
