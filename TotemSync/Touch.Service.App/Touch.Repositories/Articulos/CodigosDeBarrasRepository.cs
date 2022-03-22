using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Touch.Core.Comun;
using Touch.Repositories.Articulos.Contracts;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Articulos
{
    public class CodigosDeBarrasRepository : BaseRepository, ICodigosDeBarrasRepository
    {
        public CodigosDeBarrasRepository(IConfiguration configuration): base(configuration)
        {}

        public Task<bool> Delete(ComunEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task<ComunEntity> Get(long id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ComunEntity>> Get()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ComunEntity>> Get(string nombre)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ComunEntity>> GetAll(long id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Insert(ComunEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(ComunEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
