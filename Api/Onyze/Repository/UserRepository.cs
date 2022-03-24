using Core;
using Repository.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository
{
    public class UserRepository : IUserRepository
    {
        public Dictionary<string, User> Users = new Dictionary<string, User>()
        {
            {"123", new User() { Dni = "123", Name = "Name1", LastName = "LastName1", Age = 31} },
            {"456", new User (){ Dni = "456", Name = "Name2", LastName = "LastName2", Age = 32} },
            {"789", new User (){ Dni = "789", Name = "Name3", LastName = "LastName3", Age = 33} },
            {"101", new User (){ Dni = "101", Name = "Name4", LastName = "LastName4", Age = 34} },
            {"234", new User (){ Dni = "234", Name = "Name5", LastName = "LastName5", Age = 35} }
        };

        public Task<bool> Add(User user)
        {
            Users.Add(user.Dni, user);
            return Task.FromResult(true);
        }

        public Task<bool> Delete(string dni)
        {
            Users.Remove(dni);
            return Task.FromResult(true);
        }

        public Task<List<User>> GetByCriteria(Dictionary<string, object> criteria)
        {
            var list = Users.Values.ToList();
            var listReturn = new List<User>();
            foreach (var c in criteria)
            {
                listReturn.AddRange(list.Where(x=> x.GetType().GetProperty(c.Key).GetValue(x).ToString().Contains(c.Value.ToString())));
            }
            
            return Task.FromResult(listReturn.Distinct().ToList());
        }

        public async Task<User> GetByDni(string dni)
        {
            return await Task.FromResult(Users.FirstOrDefault(x => x.Key == dni).Value);
        }

        public Task<bool> Update(User user)
        {
            Users[user.Dni] = user;
            return Task.FromResult(true);
        }
    }
}
