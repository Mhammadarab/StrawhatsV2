using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cargohub.models;

namespace Cargohub.interfaces
{
    public interface ICrudService<TEntity, TKey>
    {
        List<TEntity> GetAll();
        TEntity GetById(TKey id);
        Task Create(TEntity entity);
        Task Update(TEntity entity);
        Task Delete(TKey id);
    }
    public interface IItemService : ICrudService<Item, string>
    {
        InventoryTotals GetItemInventoryTotals(string itemId);
    }
}