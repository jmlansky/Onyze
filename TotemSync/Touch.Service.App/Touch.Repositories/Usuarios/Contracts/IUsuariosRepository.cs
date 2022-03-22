using System.Collections.Generic;
using System.Threading.Tasks;
using Touch.Core.Usuarios;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Usuarios.Contracts
{
    public interface IUsuariosRepository : ISingleEntityComunRepository<Usuario>
    {
        Task<bool> UpdatePassword(Usuario usuario);
        Task<Usuario> GetPorNombreUsuario(string nombre, string[] columnsToIgnore, bool useLique);
        Task<Usuario> Get(string nombreUsuario, string password, string[] columnsToIgnore = null);
        Task<ICollection<Usuario>> GetFromCliente(long id, string[] columnsToIgnore = null);
        Task<ICollection<Usuario>> GetFromCliente(long idCliente, string name, string[] columnsToIgnore = null);
    }
}
