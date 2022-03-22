using Framework.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Touch.Core.Totems;
using CoreTotem = Touch.Core.Totems;

namespace Totem.Sync.Services.Contracts
{
    public interface ITotemsService: ISingleEntityComunService<CoreTotem.Totem>
    {
        Task<List<Programacion>>Syncronizar(long id, DateTime fechaNueva);
    }
}
