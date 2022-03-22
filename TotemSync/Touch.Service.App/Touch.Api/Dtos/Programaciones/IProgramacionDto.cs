using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Touch.Api.Dtos.Programaciones
{
    public interface IProgramacionDto
    {

        public ItemDeProgramacionDto Items { get; set; }
        public List<PeriodoDto> Periodos { get; set; }
        public DestinatarioDto Destinatarios { get; set; }
        public string Nombre { get; set; }
        public bool Activa { get; set; }
    }
}