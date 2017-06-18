using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DotNetNancy.Generic.Data.Core
{
    public interface IGenericDataRepository<T, TContext>
        where T : class, IEntity
        where TContext : class, IDisposable, new()
    {
        IQueryable<T> GetAll(params Expression<Func<T, object>>[] navigationProperties);

        //it is so important to use Expression<Func<T,bool>> for the where or else the where does not get honored at execution time
        IQueryable<T> GetList(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] navigationProperties);
        T GetSingle(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] navigationProperties);
        void Add(params T[] items);
        void Update(params T[] items);
        List<T> UpdateAndReturnManipulated(params T[] items);
        void Remove(params T[] items);

        TReturn ExecuteStoredProcedureFirstOrDefault<TReturn>(string sprocName,
            Dictionary<string, object> parameterNamesToValues) where TReturn : new();

        IList<TReturn> ExecuteStoredProcedureToList<TReturn>(string sprocName,
            Dictionary<string, object> parameterNamesToValues);

        IList<TReturn> ExecuteTableValuedFunctionToList<TReturn>(string functionName,
            Dictionary<string, object> parameterNamesToValues);

        IQueryable<T> GetList(TContext passContext, Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] navigationProperties);
        T GetSingle(TContext passContext, Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] navigationProperties);
        IQueryable<T> GetAll(TContext passContext, params Expression<Func<T, object>>[] navigationProperties);
        IQueryable<T> GetAllDefaultContext(params Expression<Func<T, object>>[] navigationProperties);

    }
}
