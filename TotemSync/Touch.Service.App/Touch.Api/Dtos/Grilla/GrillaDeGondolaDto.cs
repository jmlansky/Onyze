using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Grilla
{
    public class GrillaDeGondolaDto
    {
        [JsonProperty("grillaX")]
        public int GrillaX { get; set; }

        [JsonProperty("grillaY")]
        public int GrillaY { get; set; }

        [JsonProperty("esPlantilla")]
        public bool EsPlantilla { get; set; }
    }
}
