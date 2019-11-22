using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace HRManagementSystem.Repository
{
    public interface IDataRepository
    {

    }

    public interface IDataRepository<T> : IDataRepository
        where T : class, IIdentifiableEntity, new()
    {
        T Add(T entity);
        int AddList(List<T> list);

        void Remove(T entity);
        void Remove(int id);
        void RemoveList(Expression<Func<T, bool>> condition);

        T Update(T entity);
        //void UpdateList(Expression<Func<T, bool>> condition, string property, List<T> value);

        IEnumerable<T> Get();
        T GetSingleValue(Expression<Func<T, bool>> condition);
        IEnumerable<T> Get(Expression<Func<T, bool>> condition);
        T Get(int id);

    }
}
