using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cargohub.interfaces
{
        public interface ICrudService<TEntity, TKey>
        {
            IEnumerable<TEntity> GetAll();
            TEntity GetById(TKey id);
            Task Create(TEntity entity);
            Task Update(TEntity entity);
            Task Delete(TKey id);
        }
}