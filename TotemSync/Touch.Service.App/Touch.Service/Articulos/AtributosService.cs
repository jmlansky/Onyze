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
    public class AtributosService : BaseService, IAtributosService
    {
        private readonly IAtributosRepository atributosRepository;
        private readonly ITiposDeAtributoRepository tiposDeAtributoRepository;

        public AtributosService(IAtributosRepository atributosRepository, ITiposDeAtributoRepository tiposDeAtributoRepository)
        {
            this.atributosRepository = atributosRepository;
            this.tiposDeAtributoRepository = tiposDeAtributoRepository;
        }

        public async Task<ServiceResult> Delete(long id)
        {
            if (await atributosRepository.Delete(new Atributo() { Id = id }))
            {
                //TODO: eliminar todas las relaciones de los atributos/articulos
                return GetServiceResult(ServiceMethod.Delete, "Atributo", true);
            }
            return GetServiceResult(ServiceMethod.Delete, "Atributo", false);
        }

        public async Task<ServiceResult> DeleteAtributosDelArticulo(long id, IEnumerable<long> idsAtributos)
        {
            try
            {
                foreach (var idAtributo in idsAtributos)
                {
                    var result = await atributosRepository.DeleteAtributosDelArticulo(id, idAtributo);

                    if (!result)
                        return GetServiceResult(ServiceMethod.Delete, "Atributo", false);
                }
                return GetServiceResult(ServiceMethod.Delete, "Atributo", true);
            }
            catch (Exception ex)
            {
                return ObtenerResultadoDeError(ex, ServiceMethod.Delete);
            }
        }

        public async Task<ServiceResult> DeleteAtributosDelArticulo(long id)
        {
            try
            {
                if (await atributosRepository.DeleteAtributosDelArticulo(id))
                    return GetServiceResult(ServiceMethod.Delete, "Atributo", true);
                return GetServiceResult(ServiceMethod.Delete, "Atributo", false);
            }
            catch (Exception ex)
            {
                return ObtenerResultadoDeError(ex, ServiceMethod.Delete);
            }
        }

        public async Task<IEnumerable<Atributo>> Get()
        {
            var atributos = new List<Atributo>();
            var t = Task.Run(() =>
            {
                atributos = (List<Atributo>)atributosRepository.Get().Result;
                var tiposDeAtributos = tiposDeAtributoRepository.Get().Result;

                foreach( var atributo in atributos ) {

                    foreach( var tipoDeAtributo in tiposDeAtributos ) {

                        if( tipoDeAtributo.Id == atributo.IdTipo ) {

                            atributo.TipoAtributo = (TipoAtributo)tipoDeAtributo;

                        }

                    }

                    // atributo.TipoAtributo = (TipoAtributo)tiposDeAtributos.FirstOrDefault(x => x.Id == atributo.IdTipo);

                }

            });
            t.Wait();
            return atributos;
        }

        public async Task<Atributo> Get(long id)
        {
            var atributo = new Atributo();
            var t = Task.Run(() =>
            {
                atributo = (Atributo)atributosRepository.Get(id).Result;
                var tiposDeAtributos = tiposDeAtributoRepository.Get().Result;

                atributo.TipoAtributo = (TipoAtributo)tiposDeAtributos.FirstOrDefault(x => x.Id == atributo.IdTipo);

            });
            t.Wait();
            return atributo;
        }

        public async Task<IEnumerable<Atributo>> Get(string nombre)
        {
            var atributos = new List<Atributo>();
            var t = Task.Run(() =>
            {
                atributos = (List<Atributo>)atributosRepository.Get(nombre).Result;
                var tiposDeAtributos = tiposDeAtributoRepository.Get().Result;

                foreach (var atributo in atributos)
                    atributo.TipoAtributo = (TipoAtributo)tiposDeAtributos.FirstOrDefault(x => x.Id == atributo.IdTipo);

            });
            t.Wait();
            return atributos;
        }

        public async Task<IEnumerable<Atributo>> GetAtributosDelArticulo(long articuloId, bool incluyeTipo = true)
        {
            var atributos = await atributosRepository.GetAtributosDelArticulo(articuloId);

            if (incluyeTipo)
            {
                foreach (var atributo in atributos)
                    atributo.TipoAtributo = (TipoAtributo)await tiposDeAtributoRepository.Get(atributo.IdTipo);
            }

            return atributos;
        }

        public async Task<IEnumerable<Atributo>> GetAtributosDelTipo(long id)
        {
            var atributos = await atributosRepository.GetAtributosDelTipo(id);
            return atributos;
        }

        public async Task<ServiceResult> Insert(Atributo atributo)
        {
            var existe = (await atributosRepository.Get(atributo.Nombre)).Any(x => x.Nombre.ToUpper().Equals(atributo.Nombre.ToUpper()));
            if (existe)
                return GetServiceExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, atributo.GetType().Name);

            return GetServiceResult(ServiceMethod.Insert, atributo.GetType().Name, await atributosRepository.Insert(atributo));
        }

        public async Task<ServiceResult> InsertarAtributosDelArticulo(long articuloId, List<Atributo> atributos)
        {
            //1ro verificar si existe la relacion
            await ObtenerListaDeAtributosAInsertar(articuloId, atributos);

            //2do: si no existe el atributo y tipo de atributo, insertar
            return GetServiceResult(ServiceMethod.Insert, "Atributo", atributos.Any() && await atributosRepository.InsertarAtributosDelArticulo(articuloId, atributos));
        }

        public async Task<ServiceResult> InsertarAtributosDelArticulo(long id, IEnumerable<long> idsAtributos)
        {
            try
            {
                ServiceResult = new ServiceResult
                {
                    Message = "Se insertaron los atributos correctamente.",
                    HasErrors = false,
                    StatusCode = ServiceMethodsStatusCode.Ok,
                    Method = ServiceMethod.Insert.ToString()
                };

                var atributosExistentes = await atributosRepository.GetAtributosDelArticulo(id);
                foreach (var idAtributo in idsAtributos)
                {
                    var result = false;
                    if ((await atributosRepository.Get(idAtributo)).Id <= 0)
                        return GetServiceNonExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, "Atributo del Artículo");

                    if (!atributosExistentes.Any(x => x.Id.Equals(idAtributo)))
                        result = await atributosRepository.InsertarAtributoDelArticulo(id, idAtributo);
                    else
                        continue;

                    if (!result)
                    {
                        ServiceResult.Message = "Alguno de los atributos que intentó insertar, no pudo hacerse.";
                        ServiceResult.HasErrors = true;
                        ServiceResult.StatusCode = ServiceMethodsStatusCode.PartialContent;
                    }
                }

                return ServiceResult;
            }
            catch (Exception ex)
            {
                return ObtenerResultadoDeError(ex, ServiceMethod.Insert);
            }
        }

        public async Task<ServiceResult> Update(Atributo atributo)
        {
            var existe = (await atributosRepository.Get(atributo.Nombre)).Any(x => x.Nombre.ToUpper().Equals(atributo.Nombre.ToUpper()));
            if (existe)
                return GetServiceExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Atributo");

            var tipo = await atributosRepository.Get(atributo.Id);
            if (tipo != null && tipo.Id > 0)
                return GetServiceResult(ServiceMethod.Update, "Atributo", await atributosRepository.Update(atributo));

            return GetServiceResult(ServiceMethod.Update, "Atributo", false);
        }

        private async Task ObtenerListaDeAtributosAInsertar(long articuloId, List<Atributo> atributos)
        {
            var atributosExistentesDelArticulo = (await GetAtributosDelArticulo(articuloId));

            for (int i = 0; i < atributosExistentesDelArticulo.Count(); i++)
            {
                for (int j = 0; j < atributos.Count(); j++)
                {
                    if (atributosExistentesDelArticulo.ToArray()[i].Nombre == atributos.ToArray()[j].Nombre &&
                        atributosExistentesDelArticulo.ToArray()[i].TipoAtributo.Id == atributos.ToArray()[j].TipoAtributo.Id)
                    {
                        atributos.RemoveAt(j);
                        break;
                    }
                }
            }
        }
    }
}
