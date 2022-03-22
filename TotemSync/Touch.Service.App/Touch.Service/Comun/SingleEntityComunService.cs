using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Comun;
using Touch.Core.Invariants;
using Touch.Repositories.Comun;
using static Touch.Core.Invariants.InvariantObjects;

namespace Touch.Service.Comun
{
    public class SingleEntityComunService<T> : BaseService, ISingleEntityComunService<T> where T : new()
    {
        private readonly ISingleEntityComunRepository<T> repository;
        public SingleEntityComunService(ISingleEntityComunRepository<T> repository)
        {
            this.repository = repository;
        }

        public virtual async Task<ServiceResult> Delete(T entity)
        {
            var prop = typeof(T).GetProperties().FirstOrDefault(x => x.Name.ToLower().Equals("modificado"));
            if (prop.GetValue(entity) == null)
                prop.SetValue(entity, DateTime.Now);

            return GetServiceResult(InvariantObjects.ServiceMethod.Delete, entity.GetType().Name, await repository.Delete(entity));
        }

        public virtual async Task<IEnumerable<T>> Get()
        {
            return await repository.Get();
        }

        public async Task<IEnumerable<T>> GetPorIds(List<long> ids)
        {
            return await repository.Get(ids);
        }

        public async virtual Task<PagedResult> Get(int? pageNumber, int? pageSize)
        {
            var result = await Get();
            var pagedList = new PagedList<T>(result, pageNumber ?? 1, pageSize ?? 25);
            var pagedResult = new PagedResult(pageNumber ?? 1, pageSize ?? 25, result.Count())
            {
                PagedList = pagedList
            };

            return pagedResult;
        }

        public virtual async Task<IEnumerable<T>> Get(string name)
        {
            return await repository.Get(name);
        }

        public virtual async Task<PagedResult> Get(string name, int? pageNumber, int? pageSize)
        {
            var result = await Get(name);
            var pagedList = new PagedList<T>(result, pageNumber ?? 1, pageSize ?? 25);
            var pagedResult = new PagedResult(pageNumber ?? 1, pageSize ?? 25, result.Count())
            {
                PagedList = pagedList
            };

            return pagedResult;
        }

        public virtual async Task<T> Get(long id)
        {
            return await repository.Get(id);
        }

        public virtual async Task<ServiceResult> Insert(T entity)
        {
            try
            {
                var prop = typeof(T).GetProperties().FirstOrDefault(x => x.Name.ToLower().Equals("nombre"));
                var nombre = prop.GetValue(entity).ToString();
                var exists = await repository.Get(nombre, new string[] { "Tipo" });
                if (exists.Any())
                    return new ServiceResult()
                    {
                        HasErrors = true,
                        Message = "Ya existe un objeto con el mismo nombre.",
                        Method = ServiceMethod.Insert.ToString(),
                        StatusCode = ServiceMethodsStatusCode.Error
                    };

                return await OnInsert(entity);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual async Task<ServiceResult> InsertAndGetId(T entity)
        {
            return await OnInsertAndGetId(entity);
        }

        public virtual async Task<ServiceResult> InsertAndGetId(T entity, string[] columnsToIgnore = null)
        {
            return await OnInsertAndGetId(entity, columnsToIgnore);
        }

        public virtual async Task<ServiceResult> OnInsertAndGetId(T entity, string[] columnsToIgnore = null)
        {
            var prop = typeof(T).GetProperties().FirstOrDefault(x => x.Name.ToLower().Equals("creado"));
            if (prop.GetValue(entity) == null)
                prop.SetValue(entity, DateTime.Now);

            var result = await repository.InsertAndGetId(entity, columnsToIgnore);
            var serviceResult = new ServiceResult()
            {
                HasErrors = false,
                Message = entity.GetType().Name + " - Se ha insertado correctamente.",
                Method = "Insert",
                StatusCode = ServiceMethodsStatusCode.Ok
            };
            if (result > 0)
                serviceResult.IdObjeto = result;
            else
            {
                serviceResult.HasErrors = true;
                serviceResult.Message = entity.GetType().Name + " - Ha ocurrido un error.";
                serviceResult.StatusCode = ServiceMethodsStatusCode.Error;
            }

            return serviceResult;
        }

        public virtual async Task<ServiceResult> OnInsert(T entity, string[] columnsToIgnore = null)
        {
            var prop = typeof(T).GetProperties().FirstOrDefault(x => x.Name.ToLower().Equals("creado"));
            if (prop.GetValue(entity) == null)
                prop.SetValue(entity, DateTime.Now);
            
            var id = await repository.InsertAndGetId(entity, columnsToIgnore);

            var result = GetServiceResult(ServiceMethod.Insert, entity.GetType().Name, id>0);
            result.IdObjeto = id;
                        
            return result; 
        }

        public virtual async Task<ServiceResult> Update(T entity)
        {
            var idValue = (long)typeof(T).GetProperties().FirstOrDefault(x => x.Name.ToLower().Equals("id")).GetValue(entity);
            var exists = await repository.Get(idValue);

            if (exists == null || idValue <= 0)
                return new ServiceResult()
                {
                    HasErrors = true,
                    Message = entity.GetType().Name + "- No existe el id del objeto.",
                    Method = ServiceMethod.Insert.ToString(),
                    StatusCode = ServiceMethodsStatusCode.Error
                };

            var propId = (long) typeof(T).GetProperties().FirstOrDefault(x => x.Name.ToLower().Equals("id")).GetValue(entity);
            var propNombre = (string) typeof(T).GetProperties().FirstOrDefault(x => x.Name.ToLower().Equals("nombre")).GetValue(entity);

            var entityDB = await repository.Get(propNombre, new string[] { "Tipo" });
            var entityExist = false;

            foreach( var item in entityDB ) {

                var itemId = (long) typeof(T).GetProperties().FirstOrDefault(x => x.Name.ToLower().Equals("id")).GetValue(item);
                var itemNombre = (string) typeof(T).GetProperties().FirstOrDefault(x => x.Name.ToLower().Equals("nombre")).GetValue(item);

                if( propId != itemId && propNombre.ToUpper() == itemNombre.ToUpper() ) {

                    entityExist = true;

                }
                
            }

            if( entityExist )
                return new ServiceResult()
                {
                    HasErrors = true,
                    Message = "Ya existe un objeto con el mismo nombre.",
                    Method = ServiceMethod.Insert.ToString(),
                    StatusCode = ServiceMethodsStatusCode.Error,
                    Notes = "exist"
                };

            return GetServiceResult(ServiceMethod.Update, entity.GetType().Name, await repository.Update(entity));
        }

        public virtual async Task<ServiceResult> Update(T entity, string[] columnsToIgnore = null)
        {
            var idValue = (long)typeof(T).GetProperties().FirstOrDefault(x => x.Name.ToLower().Equals("id")).GetValue(entity);
            var exists = await repository.Get(idValue, columnsToIgnore);

            if (exists == null || idValue <= 0)
                return new ServiceResult()
                {
                    HasErrors = true,
                    Message = entity.GetType().Name + "- No existe el id del objeto.",
                    Method = ServiceMethod.Insert.ToString(),
                    StatusCode = ServiceMethodsStatusCode.Error
                };

            return GetServiceResult(ServiceMethod.Update, entity.GetType().Name, await repository.Update(entity, columnsToIgnore));
        }
       
    }
}
