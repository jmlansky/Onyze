using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Core.Usuarios;
using Touch.Repositories.Comun;
using Touch.Repositories.Usuarios.Contracts;

namespace Touch.Repositories.Usuarios
{
    public class UsuariosRepository : SingleEntityComunRepository<Usuario>, IUsuariosRepository
    {
        public UsuariosRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public override async Task<IEnumerable<Usuario>> Get(string nombre, string[] columnsToIgnore = null)
        {
            if (columnsToIgnore != null && columnsToIgnore.Any())
                Select = "SELECT " + GetColumnsForSelect(Alias, columnsToIgnore) + " ";

            Sql = Select + From + Where + "and upper(" + Alias + ".nombre) like upper(@nombre) or upper(" + Alias + ".nombre_usuario) like upper(@nombre) and eliminado = 0";
            Parameters = new Dictionary<string, object>() { { "nombre", "%" + nombre + "%" } };
            return await GetListOf<Usuario>(Sql, Parameters);
        }

        public async Task<Usuario> Get(string nombreUsuario, string password, string[] columnsToIgnore = null)
        {
            if (columnsToIgnore != null && columnsToIgnore.Any())
                Select = "SELECT " + GetColumnsForSelect(Alias, columnsToIgnore) + " ";

            Sql = Select + From + Where + "and upper(" + Alias + ".nombre_usuario) = upper(@nombreUsuario) and password = @password and eliminado = 0";
            Parameters = new Dictionary<string, object>() {
                { "nombreUsuario", nombreUsuario },
                { "password", password }
            };
            return await Get<Usuario>(Sql, Parameters);
        }


        public async Task<ICollection<Usuario>> GetFromCliente(long id, string[] columnsToIgnore = null)
        {
            if (columnsToIgnore != null && columnsToIgnore.Any())
                Select = "SELECT " + GetColumnsForSelect(Alias, columnsToIgnore) + " ";

            Sql = Select + From + Where + "and id_cliente = @id_cliente and eliminado = 0";
            Parameters = new Dictionary<string, object>() {
                { "id_cliente", id },
            };
            return await GetListOf<Usuario>(Sql, Parameters);
        }

        public async Task<ICollection<Usuario>> GetFromCliente(long id, string nombre, string[] columnsToIgnore = null)
        {
            if (columnsToIgnore != null && columnsToIgnore.Any())
                Select = "SELECT " + GetColumnsForSelect(Alias, columnsToIgnore) + " ";

            Sql = Select + From + Where + "and id_cliente = @id_cliente and eliminado = 0 and (" + Alias + ".nombre_usuario  like  '%' + @nombreUsuario + '%' OR " + Alias + ".mail  like  '%' + @nombreUsuario + '%' OR " + Alias + ".nombre  like  '%' + @nombreUsuario + '%') " +
              "ORDER BY nombre ";
            Parameters = new Dictionary<string, object>() { { "nombreUsuario", nombre ?? "" } };
            Parameters.Add("id_cliente", id);

            return await GetListOf<Usuario>(Sql, Parameters);
        }

        public async Task<Usuario> GetPorNombreUsuario(string nombreUsuario, string[] columnsToIgnore, bool useLique)
        {
            if (columnsToIgnore != null && columnsToIgnore.Any())
                Select = "SELECT " + GetColumnsForSelect(Alias, columnsToIgnore) + " ";

            Sql = Select + From + Where + "and eliminado = 0 and upper(" + Alias + ".nombre_usuario) ";

            if (useLique)
            {
                Sql += " like upper(@nombreUsuario) ";
                Parameters = new Dictionary<string, object>() { { "nombreUsuario", "%" + nombreUsuario + "%" } };
            }
            else
            {
                Sql += " = upper(@nombreUsuario) ";
                Parameters = new Dictionary<string, object>() { { "nombreUsuario", nombreUsuario } };
            }

            return await Get<Usuario>(Sql, Parameters);
        }

        public override async Task<bool> Update(Usuario usuario, string[] columnsToIgnore = null)
        {
            Sql = "UPDATE " + GetTableName() + " SET id_rol = @id_rol, nombre = @nombre, modificado = @modificado, telefono = @telefono, mail = @mail WHERE eliminado = 0 and id = @id ";
            Parameters = new Dictionary<string, object>()
            {
                { "id_rol", usuario.IdRol },
                { "nombre", usuario.Nombre },
                { "id", usuario.Id },
                { "modificado", DateTime.Now },
                { "telefono", usuario.Telefono},
                { "mail", usuario.Mail }
            };
            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }

        public async Task<bool> UpdatePassword(Usuario usuario)
        {
            Sql = "UPDATE " + GetTableName() + " SET password = @password, modificado = @modificado, requiere_cambiar_password = 0 WHERE eliminado = 0 and nombre_usuario = @nombre_usuario ";
            Parameters = new Dictionary<string, object>()
            {
                { "password", usuario.Password },
                { "nombre_usuario", usuario.NombreUsuario },
                { "modificado", DateTime.Now }
            };
            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }
    }
}
