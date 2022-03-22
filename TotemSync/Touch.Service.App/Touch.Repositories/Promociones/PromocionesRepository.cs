using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Touch.Core.Promociones;
using Touch.Repositories.Comun;
using static Touch.Core.Invariants.InvariantObjects;

namespace Touch.Repositories.Promociones
{
    public class PromocionesRepository : SingleEntityComunRepository<Promocion>, IPromocionesRepository
    {
        private readonly IDetallesDePromocionRepository detallesRepository;
        private readonly IPromocionDeProvinciasRepository promocionDeProvinciasRepository;
        private readonly IPromocionDeClientesRepository promocionDeClientesRepository;
        private readonly IPromocionDeRegionesRepository promocionDeRegionesRepository;
        private readonly ISingleEntityComunRepository<PromocionDeGrupos> promocionDeGruposRepository;
        private readonly IPromocionDeCategoriaRepository promocionDeCategoriasRepository;
        private readonly IPromocionDeFabricantesRepository promocionDeFabricantesRepository;

        public PromocionesRepository(IConfiguration configuration,
            IDetallesDePromocionRepository detallesRepository,
            IPromocionDeProvinciasRepository promocionDeProvinciasRepository,
            IPromocionDeClientesRepository promocionDeClientesRepository,
            IPromocionDeRegionesRepository promocionDeRegionesRepository,
            ISingleEntityComunRepository<PromocionDeGrupos> promocionDeGruposRepository,
            IPromocionDeCategoriaRepository promocionDeCategoriasRepository,
            IPromocionDeFabricantesRepository promocionDeFabricantesRepository) : base(configuration)
        {
            this.detallesRepository = detallesRepository;
            this.promocionDeProvinciasRepository = promocionDeProvinciasRepository;
            this.promocionDeClientesRepository = promocionDeClientesRepository;
            this.promocionDeGruposRepository = promocionDeGruposRepository;
            this.promocionDeRegionesRepository = promocionDeRegionesRepository;
            this.promocionDeCategoriasRepository = promocionDeCategoriasRepository;
            this.promocionDeFabricantesRepository = promocionDeFabricantesRepository;
        }

        public override async Task<long> InsertAndGetId(Promocion promo, string[] columnsToIgnore = null)
        {
            using SqlTransaction tran = await OpenConnectionWithTransaction();
            try
            {
                columnsToIgnore = GetColumnsToIgnoreForInsert(columnsToIgnore);

                Sql = "Insert into " + GetTableName() + " (" + GetColumnsForInsert(columnsToIgnore) + ") values (" + GetParametersString(columnsToIgnore) + ");" +
                    " Select SCOPE_IDENTITY()";

                var idPromocion = Convert.ToInt64(await ExecuteScalarQuery(Sql, GetParameters(promo, columnsToIgnore), false, tran.Connection, tran));
                promo.Id = idPromocion;

                var tasks = InsertarDestinatarios(promo, tran);
                var t = Task.Run(() =>
                {
                    foreach (var detalle in promo.ItemsDePromocion)
                        InsertarTipoDeItem(promo, tran, detalle);
                });

                tasks.ToList().Add(t);
                Task.WaitAll(tasks.ToArray());

                tran.Commit();
                return idPromocion;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return 0;
            }
        }

        public override async Task<bool> Delete(Promocion promo, string[] columnsToIgnore = null)
        {
            using SqlTransaction tran = await OpenConnectionWithTransaction();
            try
            {
                columnsToIgnore = GetColumnsToIgnoreForUpdate(columnsToIgnore);
                promo.Modificado = DateTime.Now;

                var tasks = EliminarDestinatarios(promo, tran);
                if (promo.ItemsDePromocion != null && promo.ItemsDePromocion.Any())
                {
                    var t = Task.Run(() =>
                    {
                        foreach (var detalle in promo.ItemsDePromocion)
                            EliminarTipoDeItem(promo, tran, detalle);
                    });
                    tasks.ToList().Add(t);
                }

                var t2 = base.Delete(promo, tran, columnsToIgnore);
                tasks.Add(t2);

                Task.WaitAll(tasks.ToArray());

                tran.Commit();
                return true;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return false;
            }
        }

        public async Task<IEnumerable<Promocion>> GetVigentes(string[] columnsToIgnore = null)
        {
            if (columnsToIgnore != null && columnsToIgnore.Any())
                Select = "SELECT " + GetColumnsForSelect(Alias, columnsToIgnore) + " ";

            Sql = Select + From + Where + " and fin >= getDate()";
            return await GetListOf<Promocion>(Sql, new Dictionary<string, object>() { { "fechaDeHoy", DateTime.Now } });
        }

        private static string[] GetColumnsToIgnoreForInsert(string[] columnsToIgnore)
        {
            if (columnsToIgnore == null)
                columnsToIgnore = new string[] { "Modificado", "Id" };
            else
            {
                columnsToIgnore = columnsToIgnore.Append("Modificado").ToArray();
                columnsToIgnore = columnsToIgnore.Append("Id").ToArray();
            }

            return columnsToIgnore;
        }

        private static string[] GetColumnsToIgnoreForUpdate(string[] columnsToIgnore)
        {
            if (columnsToIgnore == null)
                columnsToIgnore = new string[] { "Creado" };
            else
                columnsToIgnore = columnsToIgnore.Append("Creado").ToArray();

            return columnsToIgnore;
        }

        private void InsertarTipoDeItem(Promocion promo, SqlTransaction tran, IItemDePromocion detalle)
        {
            detalle.IdPromocion = promo.Id;
            detalle.Creado = promo.Creado;

            if (promo.TipoItem == TiposDeItemsDePromocion.Articulos.ToString())
            {
                var item = (DetallePromocion)detalle;
                item.IdArticulo = item.Id;
                detallesRepository.Insert(item, tran, new string[] { "Articulo", "Nombre" });
            }

            if (promo.TipoItem == TiposDeItemsDePromocion.Categorias.ToString())
            {
                var item = (PromocionDeCategoria)detalle;
                item.IdCategoria = item.Id;
                promocionDeCategoriasRepository.Insert(item, tran, new string[] { "Articulo", "Nombre", "PrecioAnterior", "PrecioActual" });
            }

            if (promo.TipoItem == TiposDeItemsDePromocion.Fabricantes.ToString())
            {
                var item = (PromocionDeFabricantes)detalle;
                item.IdFabricante = item.Id;
                promocionDeFabricantesRepository.Insert(item, tran, new string[] { "Articulo", "Nombre" });
            }
        }

        private void EliminarTipoDeItem(Promocion promo, SqlTransaction tran, IItemDePromocion detalle)
        {
            //detallesRepository.DeleteFromPromocion(promo.Id, tran);
            //promocionDeCategoriasRepository.DeleteFromPromocion(promo.Id, tran);
            //promocionDeFabricantesRepository.DeleteFromPromocion(promo.Id, tran);

            //detalle.IdPromocion = promo.Id;
            //detalle.Modificado = promo.Modificado;

            if (promo.TipoItem == TiposDeItemsDePromocion.Articulos.ToString())
            {
                var item = (DetallePromocion)detalle;
                item.IdArticulo = item.Id;
                detallesRepository.DeleteFromPromocion(promo.Id, tran);
            }

            if (promo.TipoItem == TiposDeItemsDePromocion.Categorias.ToString())
            {
                var item = (PromocionDeCategoria)detalle;
                item.IdCategoria = item.Id;
                promocionDeCategoriasRepository.DeleteFromPromocion(promo.Id, tran);
            }

            if (promo.TipoItem == TiposDeItemsDePromocion.Fabricantes.ToString())
            {
                var item = (PromocionDeFabricantes)detalle;
                item.IdFabricante = item.Id;
                promocionDeFabricantesRepository.DeleteFromPromocion(promo.Id, tran);
            }

        }

        private List<Task> InsertarDestinatarios(Promocion promo, SqlTransaction tran)
        {
            var tasks = new List<Task>();
            foreach (var provincia in promo.Provincias)
            {
                provincia.IdPromocion = promo.Id;
                provincia.Creado = promo.Creado;
                tasks.Add(Task.Run(() => promocionDeProvinciasRepository.Insert(provincia, tran, new string[] { "Nombre" })));
            }

            foreach (var cliente in promo.Clientes)
            {
                cliente.IdPromocion = promo.Id;
                cliente.Creado = promo.Creado;
                tasks.Add(Task.Run(() => promocionDeClientesRepository.Insert(cliente, tran, new string[] { "Nombre" })));
            }

            foreach (var region in promo.Regiones)
            {
                region.IdPromocion = promo.Id;
                region.Creado = promo.Creado;
                tasks.Add(Task.Run(() => promocionDeRegionesRepository.Insert(region, tran, new string[] { "Nombre" })));
            }
            return tasks;
        }

        private List<Task> EliminarDestinatarios(Promocion promo, SqlTransaction tran)
        {
            var tasks = new List<Task>();
            if (promo.Provincias.Any())
                tasks.Add(Task.Run(() => promocionDeProvinciasRepository.DeleteFromPromocion(promo.Id, tran)));

            if (promo.Clientes.Any())
                tasks.Add(Task.Run(() => promocionDeClientesRepository.DeleteFromPromocion(promo.Id, tran)));

            if (promo.Regiones.Any())
                tasks.Add(Task.Run(() => promocionDeRegionesRepository.DeleteFromPromocion(promo.Id, tran)));

            return tasks;
        }
    }
}
