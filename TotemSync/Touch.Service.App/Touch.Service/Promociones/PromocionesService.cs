using Framework;
using Framework.Helpers;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Articulos;
using Touch.Core.Comun;
using Touch.Core.Promociones;
using Touch.Repositories.Articulos.Contracts;
using Touch.Repositories.Clientes.Contracts;
using Touch.Repositories.Comun;
using Touch.Repositories.Promociones;
using Touch.Service.Comun;
using static Touch.Core.Invariants.InvariantObjects;
using CategoriaDeArticulo = Touch.Core.Promociones.CategoriaDeArticulo;

namespace Touch.Service.Promociones
{
    public class PromocionesService : SingleEntityComunService<Promocion>, IPromocionesService
    {
        private readonly IPromocionesRepository promocionesRepository;
        private readonly ISingleEntityComunRepository<TipoPromocion> tipoPromocionRepo;
        private readonly ISingleEntityComunRepository<CategoriaDeArticulo> categoriasRepo;
        private readonly ISingleEntityComunRepository<Provincia> provinciasRepo;
        private readonly ISingleEntityComunRepository<Region> regionesRepo;
        private readonly IClientesRepository clientesRepo;
        private readonly ISingleEntityComunRepository<TipoItemPromocion> tiposItemRepository;

        private readonly IPromocionDeProvinciasRepository promocionesDeProvinciasRepository;
        private readonly IPromocionDeRegionesRepository promocionesDeRegionesRepository;
        private readonly IPromocionDeClientesRepository promocionesDeClientesRepository;
        private readonly IPromocionDeGruposRepository promocionesDeGruposRepository;
        private readonly IDetallesDePromocionRepository detallesDePromocionRepository;
        private readonly IPromocionDeCategoriaRepository promocionDeCategoriaRepository;
        private readonly IArticulosRepository articulosRepository;
        private readonly IPromocionDeFabricantesRepository promocionDeMarcaRepository;

        public readonly IPromociones promociones;
        private readonly string[] columnsToIgnore = new string[]
        { "Tipo", "Categorias", "Provincias", "Regiones", "Clientes", "Detalles", "Grupos", "ItemsDePromocion", "TipoItem" };

        public PromocionesService(IPromocionesRepository promocionesRepository,
            IPromociones promociones,
            ISingleEntityComunRepository<TipoPromocion> tipoPromocionRepo,
            ISingleEntityComunRepository<CategoriaDeArticulo> categoriasRepo,
            ISingleEntityComunRepository<Provincia> provinciasRepo,
            ISingleEntityComunRepository<Region> regionesRepo,
            IClientesRepository clientesRepo,
            ISingleEntityComunRepository<TipoItemPromocion> tiposItemRepository,

            IArticulosRepository articulosRepository,

            IPromocionDeProvinciasRepository promocionesDeProvinciasRepository,
            IPromocionDeRegionesRepository promocionesDeRegionesRepository,
            IPromocionDeClientesRepository promocionesDeClientesRepository,
            IPromocionDeGruposRepository promocionesDeGruposRepository,
            IDetallesDePromocionRepository detallesDePromocionRepository,
            IPromocionDeCategoriaRepository promocionDeCategoriaRepository,
            IPromocionDeFabricantesRepository promocionDeMarcaRepository) : base(promocionesRepository)
        {
            this.promocionesRepository = promocionesRepository;
            this.promociones = promociones;
            this.tipoPromocionRepo = tipoPromocionRepo;
            this.categoriasRepo = categoriasRepo;
            this.provinciasRepo = provinciasRepo;
            this.regionesRepo = regionesRepo;
            this.clientesRepo = clientesRepo;

            this.promocionesDeProvinciasRepository = promocionesDeProvinciasRepository;
            this.promocionesDeRegionesRepository = promocionesDeRegionesRepository;
            this.promocionesDeClientesRepository = promocionesDeClientesRepository;
            this.promocionesDeGruposRepository = promocionesDeGruposRepository;
            this.detallesDePromocionRepository = detallesDePromocionRepository;
            this.articulosRepository = articulosRepository;
            this.tiposItemRepository = tiposItemRepository;
            this.promocionDeCategoriaRepository = promocionDeCategoriaRepository;
            this.promocionDeMarcaRepository = promocionDeMarcaRepository;
        }

        public async override Task<Promocion> Get(long id)
        {
            var promocion = await promocionesRepository.Get(id, columnsToIgnore);

            //promocion.Grupos = (await promocionesDeGruposRepository.GetFromPromocion(id, new string[] { "Nombre", "Detalles" })).ToList();

            var t = Task.Run(() => CompletarDatosDePromocion(promocion));
            t.Wait();

            return promocion;
        }

        public async override Task<IEnumerable<Promocion>> Get()
        {
            var promociones = await promocionesRepository.Get(columnsToIgnore);

            var t = Task.Run(() =>
            {
                foreach (var promocion in promociones)
                    CompletarDatosDePromocion(promocion);
            });
            t.Wait();
            return promociones;
        }

        public async Task<IEnumerable<Promocion>> GetPorFiltro(FiltrosDePromocion filtros, int pageNumber, int pageSize)
        {
            var idsPromo = new List<long>();
            var promos = new List<Promocion>();
            if (filtros.Id <= 0)
                promos = (List<Promocion>)await promocionesRepository.GetVigentes(columnsToIgnore);
            else
                promos.Add(await promocionesRepository.Get(filtros.Id, columnsToIgnore));

            if (promos == null || !promos.Any())
                return promos;

            var t = Task.Run(() =>
            {
                if (filtros.IdTipo > 0)
                    promos = promos.Where(x => x.IdTipo == filtros.IdTipo).ToList();

                // ----  Filtrar ids de destinatarios
                var promosDeProvincias = promocionesDeProvinciasRepository.Get().Result;
                var promosDeRegiones = promocionesDeRegionesRepository.Get(new string[] { "Detalles" }).Result;
                var promosDeClientes = promocionesDeClientesRepository.Get(new string[] { "Nombre" }).Result;
                var promosDeGrupos = promocionesDeGruposRepository.Get(new string[] { "Detalles", "Nombre" }).Result;

                if ((filtros.Provincias == null || !filtros.Provincias.Any()) && (filtros.Regiones == null || !filtros.Regiones.Any())
                   && (filtros.Clientes == null || !filtros.Clientes.Any()) && (filtros.Grupos == null || !filtros.Grupos.Any()))
                {
                    idsPromo.AddRange(promosDeProvincias.Where(p => promos.Any(p2 => p2.Id == p.IdPromocion)).Select(x => x.IdPromocion));
                    idsPromo.AddRange(promosDeRegiones.Where(p => promos.Any(p2 => p2.Id == p.IdPromocion)).Select(x => x.IdPromocion));
                    idsPromo.AddRange(promosDeClientes.Where(p => promos.Any(p2 => p2.Id == p.IdPromocion)).Select(x => x.IdPromocion));
                    idsPromo.AddRange(promosDeGrupos.Where(p => promos.Any(p2 => p2.Id == p.IdPromocion)).Select(x => x.IdPromocion));
                }
                else
                {
                    if (filtros.Provincias != null && filtros.Provincias.Any())
                        idsPromo.AddRange(promosDeProvincias.Where(p => filtros.Provincias.Any(p2 => p2.IdProvincia == p.IdProvincia)).Select(x => x.IdPromocion));

                    if (filtros.Regiones != null && filtros.Regiones.Any())
                        idsPromo.AddRange(promosDeRegiones.Where(p => filtros.Regiones.Any(p2 => p2.IdRegion == p.IdRegion)).Select(x => x.IdPromocion));


                    if (filtros.Clientes != null && filtros.Clientes.Any())
                        idsPromo.AddRange(promosDeClientes.Where(p => filtros.Clientes.Any(p2 => p2.IdCliente == p.IdCliente)).Select(x => x.IdPromocion));

                    if (filtros.Grupos != null && filtros.Grupos.Any())
                        idsPromo.AddRange(promosDeGrupos.Where(p => filtros.Grupos.Any(p2 => p2.IdGrupo == p.IdGrupo)).Select(x => x.IdPromocion));
                }

                // ---- FIN Filtrar ids de destinatarios

                idsPromo = idsPromo.Distinct().ToList();
                promos = promos.Where(p => idsPromo.Any(p2 => p2 == p.Id)).ToList();
                promosDeProvincias = promosDeProvincias.Where(p => idsPromo.Any(p2 => p.IdPromocion == p2));
                promosDeRegiones = promosDeRegiones.Where(p => idsPromo.Any(p2 => p.IdPromocion == p2));
                promosDeClientes = promosDeClientes.Where(p => idsPromo.Any(p2 => p.IdPromocion == p2));
                promosDeGrupos = promosDeGrupos.Where(p => idsPromo.Any(p2 => p.IdPromocion == p2));

                // ---- Obtener items de promocion
                foreach (var promo in promos)
                {
                    AsociarTipoItemDeLaPromocion(promo);
                    AsociarProvinciasALaPromocion(promosDeProvincias, promo);
                    AsociarRegionesALaPromocion(promosDeRegiones, promo);
                    AsociarClientesALaPromocion(promosDeClientes, promo);
                    AsociarItemsDeLaPromocion(promo);
                }
                // ---- FIN Obtener items de promocion id

            });
            t.Wait();


            return promos.Any() ? promos.Where(x => x.ItemsDePromocion != null) : promos;
        }

        public override async Task<ServiceResult> Update(Promocion promocion)
        {
            //Validaciones
            var result = await Validar(promocion);

            CalcularValoresDePromociones(promocion);

            result.HasErrors = !await promocionesRepository.Delete(promocion, columnsToIgnore);
            promocion.Modificado = null;
            promocion.Creado = DateTime.Now;
            if (!result.HasErrors)
                result.IdObjeto = await promocionesRepository.InsertAndGetId(promocion, columnsToIgnore);

            return result;
        }

        public override async Task<ServiceResult> Insert(Promocion promocion)
        {
            //Validaciones
            var result = await Validar(promocion);
            if (result.HasErrors)
                return result;

            CalcularValoresDePromociones(promocion);

            promocion.Creado = DateTime.Now;
            result.IdObjeto = await promocionesRepository.InsertAndGetId(promocion, columnsToIgnore);
            return result;
        }

        public override async Task<ServiceResult> Delete(Promocion promocion)
        {
            var result = await promocionesRepository.Delete(promocion, columnsToIgnore);
            return GetServiceResult(ServiceMethod.Delete, "Promoción", result);
        }

        #region Metodos privados

        private async Task<ServiceResult> Validar(Promocion promocion)
        {
            if (promocion.FechaInicio > promocion.FechaFin)
                return GetServiceResult(ServiceMethod.Insert, "Fecha de inicio posterior al fin", false);

            //validar que exista el tipo de promocion
            var tipoDePromocion = await tipoPromocionRepo.Get(promocion.IdTipo);
            if (tipoDePromocion == null || tipoDePromocion.Id <= 0)
                return GetServiceNonExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, "Tipo de promoción");

            promocion.Tipo = tipoDePromocion;

            //validar tipo de item
            var tipoItem = await tiposItemRepository.Get(promocion.IdTipoItem);
            if (tipoItem == null || tipoItem.Id <= 0)
                return GetServiceResult(ServiceMethod.Insert, "Tipo de Item", false);
            promocion.TipoItem = tipoItem.Nombre;

            //Cada estrategia realiza sus propias validaciones      
            var estrategiaDePromocion = InstancesHelper.GetInstanciaAplicar<IPromociones>(tipoDePromocion.Nombre);
            if (estrategiaDePromocion == null)
                return GetServiceResult(ServiceMethod.Insert, "Estrategia sin implementación", false);

            //validar la promocion en base a la instancia obtenida
            var resultadoDeValidacion = await estrategiaDePromocion.ValidarPromocion(promocion);
            if (!resultadoDeValidacion)
                return GetServiceResult(ServiceMethod.Insert, "Validación de estrategia", false);

            //validar los destinatarios de la promocion
            var result = new ServiceResult() { HasErrors = false, StatusCode = ServiceMethodsStatusCode.Ok, Method = "Insert Promoción", Message = "Insert Ok" };
            await ValidarDestinatarios(promocion, result);

            if (!promocion.Clientes.Any() && !promocion.Provincias.Any() && !promocion.Regiones.Any())
                result = GetServiceNonExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, "No existen provincias, clientes o regiones");

            if (result.HasErrors && result.StatusCode == ServiceMethodsStatusCode.Error)
                return result;

            return GetServiceResult(ServiceMethod.Insert, "Promocion", true);
        }

        private void CalcularValoresDePromociones(Promocion promocion)
        {
            var estrategiaDePromocion = InstancesHelper.GetInstanciaAplicar<IPromociones>(promocion.Tipo.Nombre);
            estrategiaDePromocion.CalcularPromocion(promocion);
        }

        private async Task ValidarDestinatarios(Promocion promocion, ServiceResult result)
        {
            //await ValidarCategorias(result, promocion);
            await ValidarProvincias(result, promocion);
            await ValidarRegiones(result, promocion);
            await ValidarClientes(result, promocion);
        }

        //private async Task<ServiceResult> ValidarCategorias(ServiceResult result, Promocion promocion)
        //{
        //    if (!promocion.Categorias.Any())
        //        return result;
        //    else
        //    {
        //        if (promocion.Categorias.Count == 1 && promocion.Categorias.FirstOrDefault().Id <= 0)
        //        {
        //            promocion.Categorias.RemoveAt(0);
        //            return result;
        //        }

        //        var idsCategorias = (await categoriasRepo.Get(new string[] { "Subcategorias", "Archivo" })).Select(x => x.Id);
        //        var contador = 0;
        //        foreach (var idCategoria in promocion.Categorias.Select(x => x.Id))
        //        {
        //            if (idsCategorias.Any(x => x == idCategoria))
        //                contador++;
        //        }
        //        if (contador == 0)
        //            return GetServiceNonExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, "No existe ninguna de las categorias ingresadas");

        //        if (contador < promocion.Categorias.Count())
        //            result.StatusCode = ServiceMethodsStatusCode.PartialContent;

        //        return result;
        //    }
        //}

        private async Task<ServiceResult> ValidarProvincias(ServiceResult result, Promocion promocion)
        {
            if (!promocion.Provincias.Any())
                return result;
            else
            {
                if (promocion.Provincias.Count == 1 && promocion.Provincias.FirstOrDefault().IdProvincia <= 0)
                {
                    promocion.Provincias.RemoveAt(0);
                    return result;
                }

                var idsProvincias = (await provinciasRepo.Get()).Select(x => x.Id);
                var contador = 0;
                foreach (var idProvincia in promocion.Provincias.Select(x => x.Id))
                {
                    if (idsProvincias.Any(x => x == idProvincia))
                        contador++;
                }
                if (contador == 0)
                    return GetServiceNonExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, "No existe ninguna de las provincias ingresadas");

                if (contador < promocion.Provincias.Count())
                    result.StatusCode = ServiceMethodsStatusCode.PartialContent;

                return result;
            }
        }

        private async Task<ServiceResult> ValidarRegiones(ServiceResult result, Promocion promocion)
        {
            if (!promocion.Regiones.Any())
                return result;
            else
            {

                if (promocion.Regiones.Count == 1 && promocion.Regiones.FirstOrDefault().IdRegion <= 0)
                {
                    promocion.Regiones.RemoveAt(0);
                    return result;
                }

                var idsRegiones = (await regionesRepo.Get()).Select(x => x.Id);
                var contador = 0;
                foreach (var idRegion in promocion.Regiones.Select(x => x.Id))
                {
                    if (idsRegiones.Any(x => x == idRegion))
                        contador++;
                }
                if (contador == 0)
                    return GetServiceNonExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, "No existe ninguna de las regiones ingresadas");

                if (contador < promocion.Regiones.Count())
                    result.StatusCode = ServiceMethodsStatusCode.PartialContent;

                return result;
            }
        }

        private async Task<ServiceResult> ValidarClientes(ServiceResult result, Promocion promocion)
        {
            if (!promocion.Clientes.Any())
                return result;
            else
            {
                if (promocion.Clientes.Count == 1 && promocion.Clientes.FirstOrDefault().IdCliente <= 0)
                {
                    promocion.Clientes.RemoveAt(0);
                    return result;
                }

                var idsClientes = (await clientesRepo.Get()).Select(x => x.Id);
                var contador = 0;
                foreach (var idCliente in promocion.Clientes.Select(x => x.Id))
                {
                    if (idsClientes.Any(x => x == idCliente))
                        contador++;
                }
                if (contador == 0)
                    return GetServiceNonExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, "No existe ninguna de las regiones ingresadas");

                if (contador < promocion.Clientes.Count())
                    result.StatusCode = ServiceMethodsStatusCode.PartialContent;
                return result;
            }
        }

        private void CompletarDatosDePromocion(Promocion promocion)
        {
            promocion.Tipo = tipoPromocionRepo.Get(promocion.IdTipo).Result;

            var tipoDeItem = tiposItemRepository.Get(promocion.IdTipoItem).Result;
            promocion.TipoItem = tipoDeItem.Nombre;

            ObtenerItemsDeLaPromocion(promocion);
            ObtenerProvinciasDeLaPromocion(promocion);
            ObtenerClientesDeLaPromocion(promocion);
            ObtenerRegionesDeLaPromoion(promocion);
        }

        private void ObtenerItemsDeLaPromocion(Promocion promocion)
        {
            if (promocion.TipoItem == TiposDeItemsDePromocion.Articulos.ToString())
            {
                promocion.ItemsDePromocion = detallesDePromocionRepository.GetPorPromocion(promocion.Id, new string[] { "Articulo", "Nombre" }).Result.ToList<IItemDePromocion>();
                var articulos = articulosRepository.Get().Result;
                foreach (var detalle in promocion.ItemsDePromocion)
                {
                    var articulo = (Articulo)articulos.FirstOrDefault(x => x.Id == ((DetallePromocion)detalle).IdArticulo);
                    if (articulo != null)
                    {
                        ((DetallePromocion)detalle).Articulo = new Articulo();
                        ((DetallePromocion)detalle).Articulo = articulo;
                        ((DetallePromocion)detalle).Nombre = articulo.Nombre;
                    }
                }
            }

            if (promocion.TipoItem == TiposDeItemsDePromocion.Categorias.ToString())
            {
                promocion.ItemsDePromocion =
                    promocionDeCategoriaRepository.GetPorPromocion(promocion.Id, new string[] { "Articulo", "Nombre", "PrecioAnterior", "PrecioActual" }).Result
                    .ToList<IItemDePromocion>();
                var categorias = categoriasRepo.Get(new string[] { "Subcategorias" }).Result;
                foreach (var categoria in promocion.ItemsDePromocion)
                {
                    var cat = categorias.FirstOrDefault(x => x.Id == ((PromocionDeCategoria)categoria).IdCategoria);
                    if (cat != null)
                        ((PromocionDeCategoria)categoria).Nombre = cat.Nombre;
                }
            }

            if (promocion.TipoItem == TiposDeItemsDePromocion.Fabricantes.ToString())
                promocion.ItemsDePromocion = promocionDeMarcaRepository.GetPorPromocion(promocion.Id, new string[] { "Articulo" }).Result.ToList<IItemDePromocion>();

        }

        private void ObtenerProvinciasDeLaPromocion(Promocion promocion)
        {
            promocion.Provincias = promocionesDeProvinciasRepository.GetFromPromocion(promocion.Id).Result.ToList();
            var provincias = provinciasRepo.Get().Result;
            foreach (var provincia in promocion.Provincias)
            {
                var prov = provincias.FirstOrDefault(x => x.Id == provincia.IdProvincia);
                if (prov != null)
                    provincia.Nombre = prov.Nombre;
            } 
        }

        private void ObtenerClientesDeLaPromocion(Promocion promocion)
        {
            promocion.Clientes = promocionesDeClientesRepository.GetFromPromocion(promocion.Id, new string[] { "Nombre" }).Result.ToList();
            var clientes = clientesRepo.Get().Result;

            foreach (var cli in promocion.Clientes)
            {
                var cliente = clientes.FirstOrDefault(x => x.Id == cli.IdCliente);
                if (cliente != null)
                    cli.Nombre = cliente.Nombre;
            }
        }

        private void ObtenerRegionesDeLaPromoion(Promocion promocion)
        {
            promocion.Regiones = promocionesDeRegionesRepository.GetFromPromocion(promocion.Id, new string[] { "Nombre" }).Result.ToList();
            var regiones = regionesRepo.Get().Result;
            foreach (var region in promocion.Regiones)
                region.Nombre = regiones.FirstOrDefault(x => x.Id == region.IdRegion).Nombre;
        }

        private void AsociarTipoItemDeLaPromocion(Promocion promo)
        {
            promo.Tipo = tipoPromocionRepo.Get(promo.IdTipo).Result;
            promo.TipoItem = tiposItemRepository.Get(promo.IdTipoItem).Result.Nombre;
        }

        private void AsociarItemsDeLaPromocion(Promocion promo)
        {
            if (promo.TipoItem.Equals(TiposDeItemsDePromocion.Articulos.ToString()))
            {
                promo.ItemsDePromocion = detallesDePromocionRepository.GetPorPromocion(promo.Id, new string[] { "Articulo", "Nombre" }).Result.ToList<IItemDePromocion>();

                foreach (var detalle in promo.ItemsDePromocion)
                {
                    var articulo = articulosRepository.Get(((DetallePromocion)detalle).IdArticulo).Result;
                    detalle.Nombre = articulo.Nombre;
                    detalle.Id = articulo.Id;
                }

                return;
            }

            if (promo.TipoItem.Equals(TiposDeItemsDePromocion.Categorias.ToString()))
            {
                promo.ItemsDePromocion = promocionDeCategoriaRepository.GetPorPromocion(promo.Id, new string[] { "Nombre", "PrecioAnterior", "PrecioActual" }).Result.ToList<IItemDePromocion>();

                var categorias = categoriasRepo.Get(new string[] { "Subcategorias" }).Result;
                foreach (var detalle in promo.ItemsDePromocion)
                {
                    var categoria = categorias.FirstOrDefault(x => ((PromocionDeCategoria)detalle).IdCategoria == x.Id);
                    detalle.Nombre = categoria.Nombre;
                    detalle.Id = categoria.Id;
                    //categoria.Subcategorias = categorias.Where(x => x.IdCategoriaPadre == categoria.Id).ToList();
                }
                return;
            }

            if (promo.TipoItem.Equals(TiposDeItemsDePromocion.Fabricantes.ToString()))
            {
                //promo.ItemsDePromocion = (await promocionDeCategoriaRepository.GetPorPromocion(promo.Id, new string[] { "Nombre", "PrecioAnterior", "PrecioActual" })).ToList<IItemDePromocion>();

                //var categorias = await categoriasRepo.Get(new string[] { "Subcategorias" });
                //foreach (var detalle in promo.ItemsDePromocion)
                //{
                //    detalle.Nombre = categorias.FirstOrDefault(x => ((PromocionDeCategoria)detalle).IdCategoria == x.Id).Nombre;

                //    //categoria.Subcategorias = categorias.Where(x => x.IdCategoriaPadre == categoria.Id).ToList();
                //}
                return;
            }
        }

        private void AsociarRegionesALaPromocion(IEnumerable<PromocionDeRegiones> promosDeRegiones, Promocion promo)
        {
            var regiones = regionesRepo.Get().Result;
            foreach (var promoDeRegion in promosDeRegiones.Where(x => x.IdPromocion == promo.Id))
            {
                var reg = regiones.FirstOrDefault(x => x.Id == promoDeRegion.IdRegion && promoDeRegion.IdPromocion == promo.Id);
                if (reg != null)
                    promo.Regiones.Add(new PromocionDeRegiones() { Nombre = reg.Nombre, Id = reg.Id });
            }
        }

        private void AsociarProvinciasALaPromocion(IEnumerable<PromocionDeProvincia> promosDeProvincias, Promocion promo)
        {
            var provincias = provinciasRepo.Get().Result;
            foreach (var promosDeProvincia in promosDeProvincias.Where(x => x.IdPromocion == promo.Id))
            {
                var prov = provincias.FirstOrDefault(x => x.Id == promosDeProvincia.IdProvincia && promosDeProvincia.IdPromocion == promo.Id);
                if (prov != null)
                    promo.Provincias.Add(new PromocionDeProvincia() { Nombre = prov.Nombre, Id = prov.Id });
            }
        }

        private void AsociarClientesALaPromocion(IEnumerable<PromocionDeCliente> promosDeClientes, Promocion promo)
        {
            var clientes = clientesRepo.Get().Result;
            foreach (var promoDeCliente in promosDeClientes.Where(x => x.IdPromocion == promo.Id))
            {
                var cli = clientes.FirstOrDefault(x => x.Id == promoDeCliente.IdCliente && promoDeCliente.IdPromocion == promo.Id);
                if (cli != null)
                    promo.Clientes.Add(new PromocionDeCliente() { Nombre = cli.Nombre, Id = cli.Id });
            }
        }

        #endregion
    }
}
