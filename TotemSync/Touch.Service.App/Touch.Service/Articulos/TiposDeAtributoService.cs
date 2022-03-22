using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Articulos;
using Touch.Core.Comun;
using Touch.Repositories.Articulos.Contracts;
using Touch.Repositories.Articulos.Contracts;
using Touch.Service.Comun;
using static Touch.Core.Invariants.InvariantObjects;

namespace Touch.Service.Articulos
{
    public class TiposDeAtributoService : BaseService, ITiposDeAtributoService
    {
        private readonly ITiposDeAtributoRepository tiposDeAtributoRepository;
        public TiposDeAtributoService(ITiposDeAtributoRepository tiposDeAtributoRepository)
        {
            this.tiposDeAtributoRepository = tiposDeAtributoRepository;
        }
        public async Task<IEnumerable<TipoAtributo>> Get()
        {
            return (IEnumerable<TipoAtributo>)await tiposDeAtributoRepository.Get();
        }

        public async Task<ServiceResult> Insert(TipoAtributo tipoDeAtributo)
        {
            tipoDeAtributo.Creado = DateTime.Now;
            var tipos = await tiposDeAtributoRepository.Get(tipoDeAtributo.Nombre);
            if (tipos.Any(x => x.Nombre.ToUpper().Equals(tipoDeAtributo.Nombre.ToUpper())))
                return GetServiceExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, "Tipo de Atributo - Nombre");

            return GetServiceResult(ServiceMethod.Insert, "Tipo de Atributo", await tiposDeAtributoRepository.Insert(tipoDeAtributo));
        }

        public async Task<ServiceResult> Update(TipoAtributo tipoDeAtributo) {

            var tipos = await tiposDeAtributoRepository.Get(tipoDeAtributo.Nombre);

            if( tipos.Any( x => x.Id != tipoDeAtributo.Id && x.Nombre.ToUpper().Equals(tipoDeAtributo.Nombre.ToUpper())) )
                return GetServiceExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Tipo de Atributo - Nombre");

            var tipo = await tiposDeAtributoRepository.Get(tipoDeAtributo.Id);
            if( tipo != null && tipo.Id > 0 )
                return GetServiceResult(ServiceMethod.Update, "Tipo de Atributo", await tiposDeAtributoRepository.Update(tipoDeAtributo));
            else
                return GetServiceNonExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Tipo de Atributo");

        }

        public async Task<ServiceResult> Delete(TipoAtributo tipoDeAtributo)
        {
            return GetServiceResult(ServiceMethod.Delete, "Tipo de Atributo", await tiposDeAtributoRepository.Delete(tipoDeAtributo));
        }

        public async Task<TipoAtributo> Get(long id)
        {
            return (TipoAtributo)await tiposDeAtributoRepository.Get(id);
        }

        public async Task<IEnumerable<TipoAtributo>> Get(string nombre)
        {
            return (IEnumerable<TipoAtributo>)await tiposDeAtributoRepository.Get(nombre);
        }
    }
}
