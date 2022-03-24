using Core;
using Repository.Contracts;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class UserService: IUserService
    {
        private readonly IUserRepository userRepository;
        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<bool> Add(User user)
        {
            return await userRepository.Add(user);
        }

        public async Task<bool> Delete(string dni)
        {
            return await userRepository.Delete(dni);
        }

        public async Task<List<User>> Get(Dictionary<string, object> criteria)
        {
            return await userRepository.GetByCriteria(criteria);
        }

        public async Task<User> GetByDni(string dni)
        {
           return await userRepository.GetByDni(dni);
        }

        public async Task<bool> Update(User user)
        {
            var oldUser = await userRepository.GetByDni(user.Dni);
            if (oldUser == null)
                return false;

            var result = await userRepository.Update(user);
            return result;
        }
    }
}
