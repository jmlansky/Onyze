using Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface IUserService
    {
        Task<User> GetByDni(string dni);
        Task<List<User>> Get(Dictionary<string, object> criteria);
        Task<bool> Update(User user);
        Task<bool> Delete(string dni);
        Task<bool> Add(User user);        
    }
}
