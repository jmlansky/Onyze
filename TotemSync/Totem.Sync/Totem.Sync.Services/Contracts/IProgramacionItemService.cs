using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Totems;
using Touch.Core.Totems.Contracts;

namespace Totem.Sync.Services.Contracts
{
    public interface IProgramacionItemService<TContext>
    {
        List<Programacion> AddProgramaciones(List<IProgramacionItem> itemsDeProgramaciones, IEnumerable<ProgramacionItem> items);
    }
}
