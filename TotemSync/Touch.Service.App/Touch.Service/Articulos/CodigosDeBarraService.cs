using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Core.Articulos;
using Touch.Core.Comun;
using Touch.Repositories.Articulos.Contracts;
using Touch.Repositories.Comun;
using Touch.Service.Comun;
using static Touch.Core.Invariants.InvariantObjects;

namespace Touch.Service.Articulos
{
    public class CodigosDeBarraService : SingleEntityComunService<CodigoDeBarras>, ICodigosDeBarraService
    {
        private readonly ICodigosDeBarraRepository codigosRepository;
        private readonly IArticulosService articulosService;

        public CodigosDeBarraService(ISingleEntityComunRepository<CodigoDeBarras> comunRepository, IArticulosService articulosService, ICodigosDeBarraRepository codigosRepository) : base(comunRepository)
        {
            this.codigosRepository = codigosRepository;
            this.articulosService = articulosService;
        }

        public override async Task<IEnumerable<CodigoDeBarras>> Get(string ean)
        {
            return await codigosRepository.Get(ean);
        }

        public async Task<IEnumerable<CodigoDeBarras>> GetCodigosDelArticulo(long id)
        {
            return await codigosRepository.GetCodigosDelArticulo(id);
        }

        public override async Task<ServiceResult> Insert(CodigoDeBarras entity)
        {
            var existentes = await Get(entity.EAN);
            if (existentes != null && existentes.Any())
                return GetServiceExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, "Código de barras -  EAN");

            var articulo = (await articulosService.GetArticulo(entity.IdArticulo));
            if (articulo == null || articulo.Id == 0)
                return GetServiceNonExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, "Artículo");

            return await base.OnInsert(entity);
        }

        public override async Task<ServiceResult> Update(CodigoDeBarras entity)
        {
            // verifica que exista el EAN --> para actualizarlo
            var existentes = await Get(entity.EAN);
            if (existentes == null || !existentes.Any())
                return GetServiceNonExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Código de barras -  EAN");

            // verifica que el articulo tenga codigos de barra para actualizar
            var codigosDelArticulo = await codigosRepository.GetCodigosDelArticulo(entity.IdArticulo);
            if (codigosDelArticulo == null || !codigosDelArticulo.Any())
                return GetServiceNonExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Código de barras del artículo");

            // verifica que el codigo exista dentro de los codigos del articulo
            if (!codigosDelArticulo.Any(x => x.Id == entity.Id))
                return new ServiceResult()
                {
                    Message = "El artículo no tiene el código de barras que desea actualizar",
                    HasErrors = true,
                    StatusCode = ServiceMethodsStatusCode.Error,
                    Method = ServiceMethod.Update.ToString()
                };

            return await base.Update(entity);
        }
    }
}
