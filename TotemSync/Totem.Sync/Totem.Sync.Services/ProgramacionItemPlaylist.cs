using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Totem.Sync.Services.Contracts;
using Touch.Core.Totems;
using Touch.Core.Totems.Contracts;

namespace Totem.Sync.Services
{
    public class ProgramacionItemPlaylist : IProgramacionItemService<IProgramacionItem>
    {      
        public List<Programacion> AddProgramaciones(List<IProgramacionItem> itemsDeProgramaciones, IEnumerable<ProgramacionItem> items)
        {
            var lista = new List<Programacion>();
            if (itemsDeProgramaciones != null && itemsDeProgramaciones.Any())
            {
                var itemsPlaylist = items.Where(x => x.Tipo == "playlist");
                lista.AddRange(from item in itemsPlaylist
                            join playlist in itemsDeProgramaciones on item.IdItem equals playlist.Id
                            select new Programacion() { Id = item.IdProgramacion });
            }

            return lista;
        }
    }
}
