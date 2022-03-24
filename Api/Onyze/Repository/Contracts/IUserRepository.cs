using Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Contracts
{
    public interface IUserRepository
    {
        Task<User> GetByDni(string dni);
        Task<bool> Add(User user);
        Task<bool> Update(User user);
        Task<bool> Delete(string dni);
        Task<List<User>> GetByCriteria(Dictionary<string, object> criteria);
    }
}
