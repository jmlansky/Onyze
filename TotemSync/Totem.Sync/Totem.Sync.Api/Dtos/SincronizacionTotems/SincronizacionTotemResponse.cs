using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Totem.Sync.Api.Dtos
{
    public class SincronizacionTotemResponse
    {
        [JsonProperty("programaciones")]
        public List<ProgramacionDto> Programaciones{ get; set; }
    }
}
