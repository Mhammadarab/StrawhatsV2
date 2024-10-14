using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cargohub.interfaces
{
    public class IWarehouseService
    {
        public interface ICrudService<TEntity, TKey>
        {
            IEnumerable<TEntity> GetAll();
            TEntity GetById(TKey id);
            void Create(TEntity entity);
            void Update(TEntity entity);
            void Delete(TKey id);
        }
    }
}