using System.Collections.Generic;
using System.Threading.Tasks;
using Cargohub.models;

namespace Cargohub.interfaces
{
    public interface ICrudService<TEntity, TKey>
    {
        List<TEntity> GetAll(int? pageNumber = null, int? pageSize = null); // Updated to include pagination
        TEntity GetById(TKey id);
        Task Create(TEntity entity);
        Task Update(TEntity entity);
        Task Delete(TKey id);
    }

}
