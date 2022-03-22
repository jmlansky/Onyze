using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Gondolas;
using Touch.Repositories.Comun;
using Touch.Repositories.Gondolas.Contracts;

namespace Touch.Repositories.Gondolas
{
    public class GondolasRepository : SingleEntityComunRepository<Gondola>, IGondolasRepository
    {
        private readonly IEstantesRepository estantesRepository;
        private readonly IArticulosPorEstanteRepository articulosPorEstanteRepository;
        private readonly IEstantesDecoracionesRepository estantesDecoracionesRepository;
        private readonly IGrillasRepository grillasRepository;

        //private string[] columnsToIgnoreGondola = { "Estantes", "Articulos", "Grilla" };

        public GondolasRepository(IConfiguration configuration,
            IEstantesRepository estantesRepository,
            IArticulosPorEstanteRepository articulosPorEstanteRepository,
            IGrillasRepository grillasRepository,
             IEstantesDecoracionesRepository estantesDecoracionesRepository) : base(configuration)
        {
            this.estantesRepository = estantesRepository;
            this.articulosPorEstanteRepository = articulosPorEstanteRepository;
            this.grillasRepository = grillasRepository;
            this.estantesDecoracionesRepository = estantesDecoracionesRepository;
        }

        public async Task<bool> AsociarArchivoAGondola(Gondola gondola)
        {
            var campo = gondola.IdEncabezado > 0 ? "id_encabezado" : "id_fondo";
            var id = gondola.IdEncabezado > 0 ? gondola.IdEncabezado : gondola.IdFondo;
            Sql = "update gondola set " + campo + " = @id_archivo where id = @id and eliminado = 0";

            Parameters = new Dictionary<string, object>()
            {
                { "id_archivo", id },
                { "id", gondola.Id }
            };

            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }

        public override async Task<long> InsertAndGetId(Gondola gondola, string[] columnsToIgnore = null)
        {
            using SqlTransaction tran = await OpenConnectionWithTransaction();
            try
            {
                long id = 0;
                var t = Task.Run(() =>
                {
                    id = base.InsertAndGetId(gondola, tran, columnsToIgnore).Result;
                    if (id <= 0)
                        throw new Exception("Error al insertar gondola");

                    gondola.Id = id;
                    InsertarGrillaDeGondola(gondola, gondola.Creado.Value, tran);
                });
                t.Wait();

                tran.Commit();
                return id;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return 0;
            }
        }

        public override Task<IEnumerable<Gondola>> Get(string[] columnsToIgnore = null)
        {
            return base.Get(columnsToIgnore);
        }

        public override Task<Gondola> Get(long id, string[] columnsToIgnore = null)
        {
            return base.Get(id,  new string[] { "Estantes", "Articulos", "Grilla",  "Image", "Categoria" });
        }

        public override Task<IEnumerable<Gondola>> Get(string nombre, string[] columnsToIgnore = null)
        {
            return base.Get(nombre, new string[] { "Estantes", "Articulos", "Grilla",  "Image", "Categoria" });
        }

        public async Task<IEnumerable<Gondola>> GetGondolasDelArticulo(long id)
        {
            Sql = @"select g.Id
                    from gondola g, estante e, articulos_estantes ae
                    where g.id = e.id_gondola
                    and e.id = ae.id_estante
                    and ae.eliminado = 0
                    and e.eliminado = 0
                    and g.eliminado = 0
                    and ae.id_articulo = @id_articulo";
            Parameters = new Dictionary<string, object>() { { "id_articulo", id } };
            return await GetListOf<Gondola>(Sql, Parameters);
        }

        public override Task<bool> Insert(Gondola gondola, string[] columnsToIgnore = null)
        {
            gondola.Creado = DateTime.Now;
            return base.Insert(gondola, columnsToIgnore);
        }

        public async Task<long> InsertGondaEstantesArticulos(Gondola gondola, string[] columnsToIgnore = null)
        {
            using SqlTransaction tran = await OpenConnectionWithTransaction();
            try
            {
                long idGondola = 0;
                var t = Task.Run(() =>
                {
                    idGondola = InsertAndGetId(gondola, tran, columnsToIgnore).Result;

                    if (idGondola <= 0)
                        throw new Exception("Hubo un error al insertar Góndola, Estante o Artúculos");

                    gondola.Id = idGondola;

                    InsertarGrillaDeGondola(gondola, gondola.Creado.Value, tran);

                    if (gondola.Estantes.Any())
                    {
                        var result = InsertarEstantesDeGondola(gondola, gondola.Creado.Value, tran);

                        if (!result)
                            throw new Exception("Hubo un error al insertar estantes de la góndola");
                    }
                });
                t.Wait();

                tran.Commit();
                return idGondola;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return 0;
            }
        }

        public override async Task<bool> Update(Gondola gondola, string[] columnsToIgnore = null)
        {
            gondola.ColorEncabezado = string.IsNullOrWhiteSpace(gondola.ColorEncabezado) ? string.Empty : gondola.ColorEncabezado;
            gondola.ColorTitulo = string.IsNullOrWhiteSpace(gondola.ColorTitulo) ? string.Empty : gondola.ColorTitulo;
            gondola.ColorFondo = string.IsNullOrWhiteSpace(gondola.ColorFondo) ? string.Empty : gondola.ColorFondo;

            using SqlTransaction tran = await OpenConnectionWithTransaction();
            try
            {
                var result = false;
                var t = Task.Run(() =>
                {
                    DeleteEstantesDeGondola(gondola, tran);
                    DeleteGrillaDeGondola(gondola.Id, tran);

                    gondola.Modificado = DateTime.Now;
                    result = Update(gondola, tran, columnsToIgnore).Result;

                    if (!result)
                        throw new Exception("No se pudo actualizar la gondola");

                    InsertarEstantesDeGondola(gondola, gondola.Modificado.Value, tran);
                    InsertarGrillaDeGondola(gondola, gondola.Modificado.Value, tran);
                });
                t.Wait();

                tran.Commit();
                return result;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return false;
            }
        }

        public async Task<bool> UpdateGondolaEstantesArticulos(Gondola gondola, string[] columnsToIgnore = null)
        {
            using SqlTransaction tran = await OpenConnectionWithTransaction();
            try
            {
                var result = false;
                var t = Task.Run(() =>
                {
                    DeleteEstantesDeGondola(gondola, tran);
                    DeleteGrillaDeGondola(gondola.Id, tran);

                    gondola.Modificado = DateTime.Now;
                    result = Update(gondola, tran, columnsToIgnore).Result;

                    if (!result)
                        throw new Exception("No se pudo actualizar la gondola");

                    InsertarEstantesDeGondola(gondola, gondola.Modificado.Value, tran);
                    //InsertarGrillaDeGondola(gondola, gondola.Modificado.Value, tran);
                });
                t.Wait();

                tran.Commit();
                return result;

            }
            catch (Exception ex)
            {

                tran.Rollback();
                return false;
            }
        }

        public override async Task<bool> Delete(Gondola gondola, string[] columnsToIgnore = null)
        {
            using SqlTransaction tran = await OpenConnectionWithTransaction();
            try
            {
                var result = false;
                var t = Task.Run(() =>
                {
                    DeleteEstantesDeGondola(gondola, tran);
                    DeleteGrillaDeGondola(gondola.Id, tran);

                    gondola.Modificado = DateTime.Now;
                    result = base.Delete(gondola, tran, columnsToIgnore).Result;

                    if (!result)
                        throw new Exception("No se pudo eliminar la gondola");
                });
                t.Wait();

                tran.Commit();
                return result;

            }
            catch (Exception ex)
            {
                tran.Rollback();
                return false;
            }
        }

        private Task DeleteGrillaDeGondola(long id, SqlTransaction tran)
        {
            var result = grillasRepository.DeleteFromGondola(id, tran);
            return Task.FromResult(result);
        }

        private Task DeleteEstantesDeGondola(Gondola gondola, SqlTransaction tran)
        {

            var idEstantes = estantesRepository.GetEstantesDeLaGondola(gondola.Id, tran).Result.Select(x => x.Id);
            var deleteResult = estantesRepository.DeleteFromGondola(gondola.Id, tran).Result;

            if (deleteResult)
            {
                foreach (var id in idEstantes)
                {
                    articulosPorEstanteRepository.DeleteAllArticulosDelEstante(id, tran);
                    estantesDecoracionesRepository.DeleteFromEstante(id, tran);
                }
            }

            return Task.FromResult(1);
        }

        public Gondola GetPorIdFondoIdEncabezado(long id)
        {
            Sql = "Select * from gondola where eliminado = 0 and (id_encabezado = @id or id_fondo = @id)";
            Parameters = new Dictionary<string, object>() { { "id", id } };

            return Get<Gondola>(Sql, Parameters).Result;
        }

        private bool InsertarEstantesDeGondola(Gondola gondola, DateTime date, SqlTransaction tran)
        {
            bool result = false;
            foreach (var estante in gondola.Estantes)
            {
                estante.IdGondola = gondola.Id;
                estante.Creado = date;
                var idEstante = estantesRepository.InsertAndGetId(estante, tran, new string[] { "Articulos", "Decoraciones" }).Result;
                estante.Id = idEstante;

                if (idEstante <= 0)
                    throw new Exception("No se pudo insertar un estante");
                else
                    result = true;

                //if (estante.Articulos.Any())
                //    result = InsertarArticulosDeEstante(estante, tran);
            }

            return result;
        }

        private bool InsertarArticulosDeEstante(Estante estante, SqlTransaction tran)
        {
            var result = false;

            //TODO: Reemplazar bloque inferior por "Insert(List<ArticuloEstante> articulos, long idEstante, DateTime dateTime, SqlTransaction tran) "
            foreach (var articulo in estante.Articulos)
            {
                articulo.IdEstante = estante.Id;
                articulo.Creado = estante.Creado;
                result = articulosPorEstanteRepository.Insert(articulo, tran, new string[] { "Nombre", "Decoraciones", "EsDestacado" }).Result;

                if (!result)
                    throw new Exception("No se pudo insertar un artículo");
            }

            return result;
        }

        private void InsertarGrillaDeGondola(Gondola gondola, DateTime date, SqlTransaction tran)
        {
            gondola.Grilla.IdGondola = gondola.Id;
            gondola.Grilla.Creado = date;
            var result = grillasRepository.Insert(gondola.Grilla, tran).Result;

            if (!result)
                throw new Exception("Error al insertar grilla");
        }

        public async Task<IEnumerable<Gondola>> GetWithDeleted(DateTime fechaSincro, string[] columnsToIgnore = null)
        {
            Sql = @"select id
      ,titulo
      ,nombre 
      ,color_titulo as ColorTitulo
      ,color_fondo as ColorFondo
      ,color_encabezado as ColorEncabezado
      ,creado
      ,modificado
      ,eliminado
      ,id_encabezado as IdEncabezado
      ,id_fondo as IdFondo
      ,activo
      , imagen
      ,id_categoria as IdCategoria
        FROM gondola   where creado > @fechasincro or (modificado is not null and modificado > @fechasincro)
        ORDER BY CASE WHEN modificado is null  THEN creado ELSE modificado END
";
            Parameters = new Dictionary<string, object>() { { "@fechasincro", fechaSincro } };
            return await GetListOf<Gondola>(Sql, Parameters);
        }

        public async Task<long> GetCurentCount()
        {
            Select = "SELECT count(*) ";
            Sql = Select + From + Where;

            return await GetValue<long>(Sql, Parameters);
        }
    }
}
