using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HRManagementSystem.Repository
{
    public abstract class DataRepositoryBase<T, U> : IDataRepository<T>
         where T : class, IIdentifiableEntity, new()
         where U : DbContext, new()
    {
        protected abstract DbSet<T> DbSet(U entityContext);
        protected abstract Expression<Func<T, bool>> IdentifierPredicate(U entityContext, int id);

        T AddEntity(U entityContext, T entity)
        {
            return DbSet(entityContext).Add(entity);
        }

        IEnumerable<T> GetEntities(U entityContext)
        {
            return DbSet(entityContext).ToFullyLoaded();
        }


        IEnumerable<T> GetEntities(U entityContext, Expression<Func<T, bool>> condition)
        {
            return DbSet(entityContext).Where(condition).ToFullyLoaded();
        }

        T GetEntity(U entityContext, int id)
        {
            return DbSet(entityContext).Where(IdentifierPredicate(entityContext, id)).FirstOrDefault();
        }

        T GetEntity(U entityContext, Expression<Func<T, bool>> condition)
        {
            return DbSet(entityContext).Where(condition).SingleOrDefault();
        }

        T UpdateEntity(U entityContext, T entity)
        {
            var q = DbSet(entityContext).Where(IdentifierPredicate(entityContext, entity.EntityId));
            return q.FirstOrDefault();
        }

        void AddEntitiesList(U entityContext, List<T> entities)
        {
            DbSet(entityContext).AddRange(entities);
        }

        public virtual int AddList(List<T> list)
        {
            using (U entityContext = new U())
            {
                AddEntitiesList(entityContext, list);
                var c = entityContext.SaveChanges();
                return c;
            }
        }

        public virtual T Add(T entity)
        {
            using (U entityContext = new U())
            {
                T addedEntity = AddEntity(entityContext, entity);
                entityContext.SaveChanges();
                return addedEntity;
            }
        }

        public virtual void Remove(T entity)
        {
            using (U entityContext = new U())
            {
                entityContext.Entry<T>(entity).State = EntityState.Deleted;
                entityContext.SaveChanges();
            }
        }

        public virtual void Remove(int id)
        {
            using (U entityContext = new U())
            {
                T entity = GetEntity(entityContext, id);
                entityContext.Entry<T>(entity).State = EntityState.Deleted;
                entityContext.SaveChanges();
            }
        }

        public virtual void RemoveList(Expression<Func<T, bool>> condition)
        {
            using (U entityContext = new U())
            {
                var entites = DbSet(entityContext).RemoveRange(GetEntities(entityContext, condition));
                entityContext.SaveChanges();
            }
        }

        public virtual T Update(T entity)
        {
            using (U entityContext = new U())
            {
                T existingEntity = UpdateEntity(entityContext, entity);

                SimpleMapper.PropertyMap(entity, existingEntity);

                entityContext.SaveChanges();
                return existingEntity;
            }
        }

        //public virtual void UpdateList(Expression<Func<T, bool>> condition, string property, List<T> value)
        //{
        //    using (U entityContext = new U())
        //    {
        //        var entites = DbSet(entityContext).Where(condition).AsQueryable();
        //        //PropertyInfo propertyInfo = entites.GetType().GetProperty(property);
        //        //propertyInfo.SetValue(entites, Convert.ChangeType(value, propertyInfo.PropertyType), null);
        //        entityContext.Entry(entites).CurrentValues.SetValues(value);
        //        entityContext.SaveChanges();
        //    }
        //}
        public static async Task<T> Post(string url, T entity)
        {

            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (var content = new StringContent(JsonConvert.SerializeObject(entity), UTF8Encoding.UTF8, "application/json"))
                {
                    using (var response = await client.PostAsync(url, content))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var objsJsonString = await response.Content.ReadAsStringAsync();
                            var jsonContent = JsonConvert.DeserializeObject<T>(objsJsonString);
                            return jsonContent;
                        }
                        else
                        {
                            return default(T);
                        }
                    }
                }
            }
        }



        public virtual T Get(int id)
        {
            using (U entityContext = new U())
                return GetEntity(entityContext, id);
        }

        public virtual IEnumerable<T> Get()
        {
            using (U entityContext = new U())
                return (GetEntities(entityContext)).ToArray().ToList();
        }

        public virtual IEnumerable<T> Get(Expression<Func<T, bool>> condition)
        {
            using (U entityContext = new U())
                return (GetEntities(entityContext, condition)).ToArray().ToList();
        }

        public virtual T GetSingleValue(Expression<Func<T, bool>> condition)
        {
            using (U entityContext = new U())
            {
                return (GetEntity(entityContext, condition));
            }
        }
    }
}
