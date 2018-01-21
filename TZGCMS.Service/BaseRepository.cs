using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TZGCMS.Data.Entity;
using TZGCMS.Resources.Admin;

namespace TZGCMS.Service
{
    public partial interface IBaseRepository<T> where T : class
    {
        #region Async
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetManyAsync(Expression<Func<T, bool>> expression);
        Task<IEnumerable<T>> GetManyAsync(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetManyAsync<KProperty>(Expression<Func<T, bool>> expression, Expression<Func<T, KProperty>> orderByExpression, bool ascending);
        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default(CancellationToken), params Expression<Func<T, object>>[] includes);
        Task<bool> UpdateAsync(T entity);

        #endregion

        IEnumerable<T> GetAll();
        T GetFirstOrDefault(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes);

        T Get(Expression<Func<T, bool>> expression);
        IEnumerable<T> GetMany(Expression<Func<T, bool>> expression);
        IEnumerable<T> GetMany<KProperty>(Expression<Func<T, bool>> expression,Expression<Func<T, KProperty>> orderByExpression, bool ascending);

        IEnumerable<T> GetPagedElements<KProperty>(int pageIndex, int pageCount,
            Expression<Func<T, KProperty>> orderByExpression, bool ascending);

        IEnumerable<T> GetPagedElements<KProperty>(int pageIndex, int pageCount,
            Expression<Func<T, KProperty>> orderByExpression, bool ascending,
            params Expression<Func<T, object>>[] includes);

        IEnumerable<T> GetPagedElements<KProperty>(int pageIndex, int pageCount,
            Expression<Func<T, KProperty>> orderByExpression, Expression<Func<T, bool>> filter, bool ascending,
            params Expression<Func<T, object>>[] includes);

        T GetById(object id);
        T Insert(T entity);
        bool Insert(IEnumerable<T> entities);
        bool Update(T entity);
        bool Update(IEnumerable<T> entities);
        bool UpdateNotSave(T entity);
        bool Delete(object id);
        bool Delete(T entity);
        bool Delete(IEnumerable<T> entities);
        long Count(Expression<Func<T, bool>> expression);
        int CountInt(Expression<Func<T, bool>> expression);
        void Save();
    }

    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        internal TZGEntities Context;
        internal DbSet<T> Table;

        public BaseRepository(TZGEntities context)
        {
            this.Context = context;
            this.Table = context.Set<T>();
        }

        #region Async
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await Table.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> GetManyAsync(Expression<Func<T, bool>> expression)
        {
            return await Table.Where(expression).ToListAsync();
        }
        public virtual async Task<IEnumerable<T>> GetManyAsync(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> queryable = Table;
            if (includes.Any())
                queryable = includes.Aggregate(Table.AsQueryable(), (current, include) => current.Include(include));
            return await queryable.Where(expression).ToListAsync();
        }
        public virtual async Task<IEnumerable<T>> GetManyAsync<KProperty>(Expression<Func<T, bool>> expression, Expression<Func<T, KProperty>> orderByExpression, bool ascending)
        {
            IQueryable<T> queryable = expression == null ? Table : Table.Where(expression);

            if (orderByExpression == (Expression<Func<T, KProperty>>)null)
                throw new ArgumentNullException("orderByExpression", Messages.OrderByExpressionCannotBeNullException);

            if (ascending)
            {
                return await queryable.OrderBy(orderByExpression).ToListAsync();
            }
            else
            {
                return await Table.OrderByDescending(orderByExpression).ToListAsync();
            }

        }
       
        public Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter,
            CancellationToken cancellationToken = default(CancellationToken),
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> queryable = Table;
            if (includes.Any())
                queryable = includes.Aggregate(Table.AsQueryable(), (current, include) => current.Include(include));
            return filter == null ? queryable.AsNoTracking().FirstOrDefaultAsync(cancellationToken) : queryable.AsNoTracking().FirstOrDefaultAsync(filter, cancellationToken);
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            bool result;
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                Table.Attach(entity);
                Context.Entry(entity).State = EntityState.Modified;
                await Context.SaveChangesAsync();
                result = true;
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                var fail = new Exception(msg, dbEx);
                //throw fail;
                result = false;
            }
            return result;
        }
        #endregion

        public virtual IEnumerable<T> GetAll()
        {
            return Table;
        }

        public virtual T Get(Expression<Func<T, bool>> expression)
        {
            return Table.FirstOrDefault(expression);
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter,   params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> queryable = Table;
            if (includes.Any())
                queryable = includes.Aggregate(Table.AsQueryable(), (current, include) => current.Include(include));
            return queryable.AsNoTracking().FirstOrDefault(filter);
        }

        public virtual IEnumerable<T> GetMany(Expression<Func<T, bool>> expression)
        {
            return Table.Where(expression);
        }
        public virtual  IEnumerable<T> GetMany<KProperty>(Expression<Func<T, bool>> expression, Expression<Func<T, KProperty>> orderByExpression, bool ascending)
        {
            IQueryable<T> queryable = expression == null ? Table : Table.Where(expression);

            if (orderByExpression == (Expression<Func<T, KProperty>>)null)
                throw new ArgumentNullException("orderByExpression", Messages.OrderByExpressionCannotBeNullException);

            if (ascending)
            {
                return  queryable.OrderBy(orderByExpression).ToList();
            }
            else
            {
                return queryable.OrderByDescending(orderByExpression).ToList();
            }

        }

        /// <summary>
        /// <see cref="SIG.Core.IRepository{T}"/>
        /// </summary>
        /// <typeparam name="S"><see cref="SIG.Core.IRepository{T}"/></typeparam>
        /// <param name="pageIndex"><see cref="SIG.Core.IRepository{T}"/></param>
        /// <param name="pageCount"><see cref="SIG.Core.IRepository{T}"/></param>
        /// <param name="orderByExpression"><see cref="SIG.Core.IRepository{T}"/></param>
        /// <param name="ascending"><see cref="SIG.Core.IRepository{T}"/></param>
        /// <returns><see cref="SIG.Core.IRepository{T}"/></returns>
        public virtual IEnumerable<T> GetPagedElements<KProperty>(int pageIndex, int pageCount,
            Expression<Func<T, KProperty>> orderByExpression, bool ascending)
        {
            if (pageIndex < 0)
                throw new ArgumentException(Messages.InvalidPageIndexException, "pageIndex");

            if (pageCount <= 0)
                throw new ArgumentException(Messages.InvalidPageCountException, "pageCount");

            if (orderByExpression == (Expression<Func<T, KProperty>>)null)
                throw new ArgumentNullException("orderByExpression", Messages.OrderByExpressionCannotBeNullException);

           // var set = Table;

            if (ascending)
            {
                return Table.OrderBy(orderByExpression)
                          .Skip(pageCount * pageIndex)
                          .Take(pageCount)
                          .AsEnumerable();
            }
            else
            {
                return Table.OrderByDescending(orderByExpression)
                          .Skip(pageCount * pageIndex)
                          .Take(pageCount)
                          .AsEnumerable();
            }
        }


        /// <summary>
        /// <see cref="SIG.Core.IRepository{T}"/>
        /// </summary>
        /// <typeparam name="S"><see cref="SIG.Core.IRepository{T}"/></typeparam>
        /// <param name="pageIndex"><see cref="SIG.Core.IRepository{T}"/></param>
        /// <param name="pageCount"><see cref="SIG.Core.IRepository{T}"/></param>
        /// <param name="orderByExpression"><see cref="SIG.Core.IRepository{T}"/></param>
        /// <param name="ascending"><see cref="SIG.Core.IRepository{T}"/></param>
        /// <returns><see cref="SIG.Core.IRepository{T}"/></returns>
        public virtual IEnumerable<T> GetPagedElements<KProperty>(int pageIndex, int pageCount,
            Expression<Func<T, KProperty>> orderByExpression, bool ascending,
            params Expression<Func<T, object>>[] includes)
        {
            if (pageIndex < 0)
                throw new ArgumentException(Messages.InvalidPageIndexException, "pageIndex");

            if (pageCount <= 0)
                throw new ArgumentException(Messages.InvalidPageCountException, "pageCount");

            if (orderByExpression == (Expression<Func<T, KProperty>>)null)
                throw new ArgumentNullException("orderByExpression", Messages.OrderByExpressionCannotBeNullException);

            if (includes == (Expression<Func<T, object>>[])null)
                throw new ArgumentNullException("includes", Messages.IncludesCannotBeNullException);

           // var set = Table;

            if (ascending)
            {
                if (includes.Any())
                {
                    return includes.Aggregate(Table.OrderBy(orderByExpression).Skip(pageCount * pageIndex).Take(pageCount).AsQueryable(),
                                     (current, include) => current.Include(include));
                }

                return Table.OrderBy(orderByExpression).Skip(pageCount * pageIndex).Take(pageCount);
            }
            else
            {
                if (includes.Any())
                {
                    return includes.Aggregate(Table.OrderByDescending(orderByExpression).Skip(pageCount * pageIndex).Take(pageCount).AsQueryable(),
                                            (current, include) => current.Include(include));
                }

                return Table.OrderByDescending(orderByExpression).Skip(pageCount * pageIndex).Take(pageCount);
            }
        }



        /// <summary>
        /// <see cref="SIG.Core.IRepository{T}"/>
        /// </summary>
        /// <typeparam name="S"><see cref="SIG.Core.IRepository{T}"/></typeparam>
        /// <param name="pageIndex"><see cref="SIG.Core.IRepository{T}"/></param>
        /// <param name="pageCount"><see cref="SIG.Core.IRepository{T}"/></param>
        /// <param name="orderByExpression"><see cref="SIG.Core.IRepository{T}"/></param>
        /// <param name="ascending"><see cref="SIG.Core.IRepository{T}"/></param>
        /// <returns><see cref="SIG.Core.IRepository{T}"/></returns>
        public virtual IEnumerable<T> GetPagedElements<KProperty>(int pageIndex, int pageCount,
            Expression<Func<T, KProperty>> orderByExpression, Expression<Func<T, bool>> filter, bool ascending,
            params Expression<Func<T, object>>[] includes)
        {
            if (pageIndex < 0)
                throw new ArgumentException(Messages.InvalidPageIndexException, "pageIndex");

            if (pageCount <= 0)
                throw new ArgumentException(Messages.InvalidPageCountException, "pageCount");

            if (orderByExpression == (Expression<Func<T, KProperty>>)null)
                throw new ArgumentNullException("orderByExpression", Messages.OrderByExpressionCannotBeNullException);

            if (includes == (Expression<Func<T, object>>[])null)
                throw new ArgumentNullException("includes", Messages.IncludesCannotBeNullException);

            if (filter == (Expression<Func<T, bool>>)null)
                throw new ArgumentNullException("filter", Messages.FilterCannotBeNullException);
            var set = Table.Where(filter);

            if (ascending)
            {
                if (includes.Any())
                {
                    return includes.Aggregate(set.OrderBy(orderByExpression).Skip(pageCount * pageIndex).Take(pageCount).AsQueryable(),
                                             (current, include) => current.Include(include));
                }
                return set.OrderBy(orderByExpression).Skip(pageCount * pageIndex).Take(pageCount);

            }
            else
            {
                if (includes.Any())
                {
                    return includes.Aggregate(set.OrderByDescending(orderByExpression).Skip(pageCount * pageIndex).Take(pageCount).AsQueryable(),
                                       (current, include) => current.Include(include));
                }
                return set.OrderByDescending(orderByExpression).Skip(pageCount * pageIndex).Take(pageCount);
            }
        }

        public virtual T GetById(object id)
        {
            return Table.Find(id);
        }

        public virtual T Insert(T entity)
        {
          
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

               var result =  Table.Add(entity);
                Context.SaveChanges();
               return result;
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;

                var fail = new Exception(msg, dbEx);
                throw fail;
               // result = false;
            }
           
        }

        public virtual bool Insert(IEnumerable<T> entities)
        {
            bool result;
            try
            {
                if (entities == null)
                    throw new ArgumentNullException("entities");

                foreach (var entity in entities)
                    Table.Add(entity);

                Context.SaveChanges();
                result = true;
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;

                var fail = new Exception(msg, dbEx);
                //throw fail;
                result = false;
            }
            return result;
        }

        public virtual bool Delete(object id)
        {
            bool result;
            try
            {
                if (id == null)
                    throw new ArgumentNullException("id");

                T entityToDelete = Table.Find(id);
                Delete(entityToDelete);
                this.Context.SaveChanges();
                result = true;
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                var fail = new Exception(msg, dbEx);
                //throw fail;
                result = false;
            }
            return result;
        }

        public virtual bool Delete(T entityToDelete)
        {
            bool result;
            try
            {
                if (entityToDelete == null)
                    throw new ArgumentNullException("entityToDelete");

                if (Context.Entry(entityToDelete).State == EntityState.Detached)
                {
                    Table.Attach(entityToDelete);
                }
                Table.Remove(entityToDelete);
                this.Context.SaveChanges();
                result = true;
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                var fail = new Exception(msg, dbEx);
                //throw fail;
                result = false;
            }
            return result;
        }

        public virtual bool Delete(IEnumerable<T> entities)
        {
            bool result;
            try
            {
                if (entities == null)
                    throw new ArgumentNullException("entities");

                foreach (var entity in entities)
                    Table.Remove(entity);

                Context.SaveChanges();
                result = true;
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                var fail = new Exception(msg, dbEx);
                //throw fail;
                result = false;
            }
            return result;
        }

        public virtual bool Update(T entityToUpdate)
        {
            bool result;
            try
            {
                if (entityToUpdate == null)
                    throw new ArgumentNullException("entityToUpdate");

                Table.Attach(entityToUpdate);
                Context.Entry(entityToUpdate).State = EntityState.Modified;
                Context.SaveChanges();
                result = true;
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                var fail = new Exception(msg, dbEx);
                //throw fail;
                result = false;
            }
            return result;

        }

        public virtual bool Update(IEnumerable<T> entities)
        {
            bool result;
            try
            {
                if (entities == null)
                    throw new ArgumentNullException("entities");

                Context.SaveChanges();
                result = true;
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                var fail = new Exception(msg, dbEx);
                //throw fail;
                result = false;
            }
            return result;
        }

        public virtual bool UpdateNotSave(T entityToUpdate)
        {
            bool result;
            try
            {
                if (entityToUpdate == null)
                    throw new ArgumentNullException("entityToUpdate");

                Table.Attach(entityToUpdate);
                Context.Entry(entityToUpdate).State = EntityState.Modified;
                result = true;
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                var fail = new Exception(msg, dbEx);
                //throw fail;
                result = false;
            }
            return result;

        }

        public virtual long Count(Expression<Func<T, bool>> expression)
        {
            return Table.Count(expression);
        }

        public virtual int CountInt(Expression<Func<T, bool>> expression)
        {
            return Table.Count(expression);
        }

        public virtual void Save()
        {
            Context.SaveChanges();
        }

       
    }

}
