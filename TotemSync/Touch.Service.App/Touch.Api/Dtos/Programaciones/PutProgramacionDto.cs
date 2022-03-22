using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Programaciones
{
    public class PutProgramacionDto : IProgramacionDto
    {

        public ItemDeProgramacionDto Items { get; set; } = new ItemDeProgramacionDto();
        public List<PeriodoDto> Periodos { get; set; } = new List<PeriodoDto>();
        public DestinatarioDto Destinatarios { get; set; } = new DestinatarioDto();
        public string Nombre { get; set; }
        public bool Activa { get; set; }
    }
}
