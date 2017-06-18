using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;
using EntityState = System.Data.Entity.EntityState;

namespace DotNetNancy.Generic.Data.Core
{
    /*
     note that all methods have been marked as virtual. This allows you to override
     * any method in the generic repository by adding a derived class in cases where
     * you need some specific logic to apply only to a certain type of entity. To be
     * able to extend the generic implementation with methods that are specific only
     * to a certain type of entity, whether it’s an initial requirement or a possible
     * future one, it’s considered a good practice to define a repository per entity
     * type from the beginning. You can simply inherit these repositories from the
     * generic one as shown below and add methods to extend the common functionality based on your needs.
     */

    public class GenericDataRepository<T, TContext> : IGenericDataRepository<T, TContext>
        where T : class, IEntity
        where TContext : class, IDisposable, new()
    {
        protected static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private TContext context = null;

        public GenericDataRepository()
        {
            context = new TContext();
        }

        public GenericDataRepository(TContext instance)
        {
            context = instance;
        }

        ~GenericDataRepository()
        {
            context.Dispose();
        }

        public virtual IQueryable<T> GetAllDefaultContext(params Expression<Func<T, object>>[] navigationProperties)
        {
            IQueryable<T> query = null;

            //using (var context = new TContext())
            //{

            DbContext db = context as DbContext;
            if (db != null)
            {
                IQueryable<T> dbQuery = (db).Set<T>();

                //Apply eager loading
                foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                    dbQuery = dbQuery.Include<T, object>(navigationProperty);

                query = dbQuery
                    .AsNoTracking();
            }
            //}
            return query;
        }

        public virtual IQueryable<T> GetAll(params Expression<Func<T, object>>[] navigationProperties)
        {

            IQueryable<T> query = null;

            using (var context = new TContext())
            {

                DbContext db = context as DbContext;
                if (db != null)
                {
                    IQueryable<T> dbQuery = (db).Set<T>();

                    //Apply eager loading
                    foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                        dbQuery = dbQuery.Include<T, object>(navigationProperty);

                    query = dbQuery
                        .AsNoTracking();
                }
            }
            return query;

        }

        public virtual IQueryable<T> GetAll(TContext passContext,
            params Expression<Func<T, object>>[] navigationProperties)
        {

            IQueryable<T> query = null;

            context = passContext;

            DbContext db = context as DbContext;
            if (db != null)
            {
                IQueryable<T> dbQuery = (db).Set<T>();

                //Apply eager loading
                foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                    dbQuery = dbQuery.Include<T, object>(navigationProperty);

                query = dbQuery
                    .AsNoTracking();
            }
            return query;

        }


        public virtual IQueryable<T> GetList(TContext passContext, Expression<Func<T, bool>> where,
            params Expression<Func<T, object>>[] navigationProperties)
        {

            IQueryable<T> query = null;
            //using (var context = new TContext())
            //{
            context = passContext;
            DbContext db = context as DbContext;
            if (db != null)
            {
                IQueryable<T> dbQuery = db.Set<T>().AsNoTracking().Where(where).AsQueryable();

                //Apply eager loading
                if (navigationProperties != null)
                {
                    foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                        dbQuery.Include<T, object>(navigationProperty);
                }
                query = dbQuery.AsQueryable();
                //}
            }
            return query;

        }

        public virtual T GetSingle(TContext passContext, Expression<Func<T, bool>> where,
            params Expression<Func<T, object>>[] navigationProperties)
        {

            T item = null;
            context = passContext;
            //using (var context = new TContext())
            //{
            DbContext db = context as DbContext;
            if (db != null)
            {
                IQueryable<T> dbQuery = db.Set<T>();

                //Apply eager loading
                if (navigationProperties != null)
                {
                    foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                        dbQuery = dbQuery.Include<T, object>(navigationProperty);
                }

                item = dbQuery
                    .AsNoTracking() //Don't track any changes for the selected item
                    .FirstOrDefault(where); //Apply where clause
            }
            //}
            return item;

        }


        public virtual IQueryable<T> GetList(Expression<Func<T, bool>> where,
            params Expression<Func<T, object>>[] navigationProperties)
        {

            IQueryable<T> query = null;
            using (var context = new TContext())
            {
                DbContext db = context as DbContext;
                if (db != null)
                {
                    IQueryable<T> dbQuery = db.Set<T>().AsNoTracking().Where(where).AsQueryable();

                    //Apply eager loading
                    if (navigationProperties != null)
                    {
                        foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                            dbQuery.Include<T, object>(navigationProperty);
                    }
                    query = dbQuery.AsQueryable();
                }
            }
            return query;

        }


        public virtual T GetSingle(Expression<Func<T, bool>> where,
            params Expression<Func<T, object>>[] navigationProperties)
        {

            T item = null;
            using (var context = new TContext())
            {
                DbContext db = context as DbContext;
                if (db != null)
                {
                    IQueryable<T> dbQuery = db.Set<T>();

                    //Apply eager loading
                    if (navigationProperties != null)
                    {
                        foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                            dbQuery = dbQuery.Include<T, object>(navigationProperty);
                    }

                    item = dbQuery
                        .AsNoTracking() //Don't track any changes for the selected item
                        .FirstOrDefault(where); //Apply where clause
                }
            }
            return item;

        }
        private Expression<Func<TInput, TOutput>> CreateSelectStatementWithNewReturnObject<TInput, TOutput>(string field)
        {
            //bulid a expression tree to create a paramter
            ParameterExpression param = Expression.Parameter(typeof(TInput), "d");
            //bulid expression tree:data.Field1
            Expression selector = Expression.Property(param, typeof(TInput).GetProperty(field));
            return Expression.Lambda<Func<TInput, TOutput>>(selector, param);

        }

        public virtual MultipleResultSets ExecuteStoredProcedureMultipleResultSets(
            string sprocName,
            Dictionary<string, object> parameterNamesToValues,
            List<Type> types)
        {

            MultipleResultSets returnSets = new MultipleResultSets();

            using (var context = new TContext())
            {
                DbContext db = context as DbContext;
                if (db != null)
                {
                    db.Database.Initialize(force: false);

                    var sprocExpression = GetSprocExpression(sprocName, parameterNamesToValues);
                    var parameterValues = GetParameterValues(parameterNamesToValues);

                    var cmd = db.Database.Connection.CreateCommand();

                    //just a heads up if you don't say that this is a sproc it will not work with parameters
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = sprocName;

                    if (parameterNamesToValues != null && parameterNamesToValues.Count > 0)
                    {
                        foreach (KeyValuePair<string, object> kvp in parameterNamesToValues)
                        {
                            SqlParameter param = new SqlParameter(kvp.Key, kvp.Value);
                            cmd.Parameters.Add(param);
                        }
                    }

                    try
                    {
                        db.Database.Connection.Open();
                        // Run the sproc  
                        var reader = cmd.ExecuteReader();

                        returnSets.ResultSets = new List<object>();
                        ObjectContext objectContext = ((IObjectContextAdapter)db).ObjectContext;

                        int counter = 0;

                        Console.WriteLine("\r\n--- Examine a generic method.");

                        returnSets.ResultSets.Add(ProcessResultSet(reader, types[counter], objectContext));


                        counter++;

                        while (reader.NextResult())
                        // Move to second result set and read 
                        {
                            returnSets.ResultSets.Add(ProcessResultSet(reader, types[counter], objectContext));
                            counter++;
                        }
                    }
                    finally
                    {
                        db.Database.Connection.Close();
                    }
                }
            }
            return returnSets;

        }

        private object ProcessResultSet(DbDataReader reader, Type type, ObjectContext objectContext)
        {

            Type objContext = typeof(ObjectContext);
            MethodInfo mi = GetTranslateOverload(objContext);
            MethodInfo miConstructed = mi.MakeGenericMethod(type);

            // string entityTypeName = types[counter].GenericTypeArguments[0].Name;
            string entityTypeName = type.Name;

            var container = objectContext.MetadataWorkspace.GetEntityContainer(objectContext.DefaultContainerName,
                DataSpace.CSpace);
            var entitySetNameQuery = (from meta in container.BaseEntitySets
                                      where meta.ElementType.Name == entityTypeName
                                      select meta.Name);
            string nameiffound = entitySetNameQuery.Any() ? entitySetNameQuery.First() : null;

            // Invoke the method. 
            object[] args = { reader, nameiffound, MergeOption.AppendOnly };
            // object[] args = new object[ ]{reader};

            var firstResults = miConstructed.Invoke(objectContext, args);

            var toList = typeof(Enumerable).GetMethod("ToList");
            var constructedToList = toList.MakeGenericMethod(type);
            var castList = constructedToList.Invoke(null, new[] { firstResults });
            return castList;

        }

        private static MethodInfo GetTranslateOverload(Type aType)
        {

            MethodInfo myMethod = aType
                .GetMethods()
                .Where(m => m.Name == "Translate")
                .Select(m => new
                {
                    Method = m,
                    Params = m.GetParameters(),
                    Args = m.GetGenericArguments()
                })
                .Where(x => x.Params.Length == 3
                            //&& x.Args.Length == 3
                            && x.Params[0].ParameterType == typeof(DbDataReader)
                //&& x.Params[1].ParameterType == typeof(string)
                //&& x.Params[2].ParameterType == typeof(MergeOption)
                //            && x.Params[0].ParameterType == x.Args[0]
                )
                .Select(x => x.Method)
                .First();
            return myMethod;

        }

        public virtual TwoResultSets<TResult1, TResult2> ExecuteStoredProcedureTwoResultSets<TResult1, TResult2>(
            string sprocName,
            Dictionary<string, object> parameterNamesToValues)
            where TResult1 : new()
            where TResult2 : new()
        {

            TwoResultSets<TResult1, TResult2> returnSets = new TwoResultSets<TResult1, TResult2>();

            using (var context = new TContext())
            {
                DbContext db = context as DbContext;
                if (db != null)
                {
                    db.Database.Initialize(force: false);

                    var sprocExpression = GetSprocExpression(sprocName, parameterNamesToValues);
                    var parameterValues = GetParameterValues(parameterNamesToValues);

                    var cmd = db.Database.Connection.CreateCommand();

                    //just a heads up if you don't say that this is a sproc it will not work with parameters
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = sprocName;

                    if (parameterNamesToValues != null && parameterNamesToValues.Count > 0)
                    {
                        foreach (KeyValuePair<string, object> kvp in parameterNamesToValues)
                        {
                            SqlParameter param = new SqlParameter(kvp.Key, kvp.Value);
                            cmd.Parameters.Add(param);
                        }
                    }

                    try
                    {
                        db.Database.Connection.Open();
                        // Run the sproc  
                        var reader = cmd.ExecuteReader();

                        var firstResults = ((IObjectContextAdapter)db)
                            .ObjectContext
                            .Translate<TResult1>(reader, typeof(TResult1).Name, MergeOption.AppendOnly).ToList();

                        returnSets.ResultSet1 = firstResults;

                        // Move to second result set and read Posts 
                        if (reader.NextResult())
                        {
                            var secondResults = ((IObjectContextAdapter)db)
                                .ObjectContext
                                .Translate<TResult2>(reader, typeof(TResult2).Name, MergeOption.AppendOnly).ToList();
                            returnSets.ResultSet2 = secondResults;
                        }
                    }
                    finally
                    {
                        db.Database.Connection.Close();
                    }
                }
            }
            return returnSets;

        }

        public virtual TReturn ExecuteStoredProcedureFirstOrDefault<TReturn>(string sprocName,
            Dictionary<string, object> parameterNamesToValues) where TReturn : new()
        {
            TReturn itemToReturn = new TReturn();

            using (var context = new TContext())
            {
                DbContext db = context as DbContext;
                if (db != null)
                {
                    var sprocExpression = GetSprocExpression(sprocName, parameterNamesToValues);
                    var parameterValues = GetParameterValues(parameterNamesToValues);
                    var result =
                        db.Database.SqlQuery<TReturn>(
                            @sprocExpression, parameterValues);
                    if (result != null)
                    {
                        itemToReturn = result.FirstOrDefault();
                    }
                }
            }
            return itemToReturn;

        }

        public virtual IList<TReturn> ExecuteStoredProcedureToList<TReturn>(string sprocName) where TReturn : new()
        {

            List<TReturn> itemToReturn = new List<TReturn>();
            using (var context = new TContext())
            {
                DbContext db = context as DbContext;
                if (db != null)
                {
                    var sprocExpression = sprocName;

                    var result =
                        db.Database.SqlQuery<TReturn>(
                            @sprocExpression);
                    if (result != null && result.Any())
                    {
                        itemToReturn = result.ToList();
                    }
                }
            }
            return itemToReturn;

        }

        public virtual IList<TReturn> ExecuteTableValuedFunctionToList<TReturn>(string functionName,
            Dictionary<string, object> parameterNamesToValues)
        {

            List<TReturn> itemsToReturn = new List<TReturn>();

            //ObjectParameter [] parameters = GetFunctionParameters(parameterNamesToValues);
            using (var context = new TContext())
            {
                DbContext db = context as DbContext;
                if (db != null)
                {

                    string query = GetFunctionCall(parameterNamesToValues, functionName);
                    var sqlParameterArray = GetSqlParameters(parameterNamesToValues);
                    var result = db.Database.SqlQuery<TReturn>(query, sqlParameterArray.ToArray()).ToList();
                    if (result != null && result.Any())
                    {
                        itemsToReturn = result.ToList();
                    }
                }

            }
            return itemsToReturn;

            //List<MyModel> data =
            //    db.Database.SqlQuery<MyModel>(
            //    "select * from dbo.my_function(@p1, @p2, @p3)",
            //    new SqlParameter("@p1", new System.DateTime(2015, 1, 1)),
            //    new SqlParameter("@p2", new System.DateTime(2015, 8, 1)),
            //    new SqlParameter("@p3", 12))
            //.ToList();

        }

        private string GetFunctionCall(Dictionary<string, object> parameterNamesToValues, string functionName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select * from ");
            sb.Append(functionName);
            sb.Append("(");

            int counter = 1;
            var paramNames = parameterNamesToValues.Keys.ToList();
            var paramCount = paramNames.Count();
            foreach (string paramName in paramNames)
            {
                sb.Append(paramName);
                if (counter < paramCount)
                {
                    sb.Append(", ");
                }
                else
                {
                    sb.Append(")");
                }
                counter++;
            }
            return sb.ToString();
        }

        private ObjectParameter[] GetFunctionParameters(Dictionary<string, object> parameterNamesToValues)
        {
            List<ObjectParameter> objectParameterList = new List<ObjectParameter>();

            foreach (KeyValuePair<string, object> kvp in parameterNamesToValues)
            {
                objectParameterList.Add(new ObjectParameter(kvp.Key, kvp.Value));
            }
            return objectParameterList.ToArray();
        }

        private List<SqlParameter> GetSqlParameters(Dictionary<string, object> parameterNamesToValues)
        {
            var sqlParameterList = new List<SqlParameter>();

            foreach (KeyValuePair<string, object> kvp in parameterNamesToValues)
            {
                sqlParameterList.Add(new SqlParameter(kvp.Key, kvp.Value));
            }
            return sqlParameterList.ToList();
        }

        public virtual IList<TReturn> ExecuteStoredProcedureToList<TReturn>(string sprocName,
                Dictionary<string, object> parameterNamesToValues)
        {

            List<TReturn> itemToReturn = new List<TReturn>();
            using (var context = new TContext())
            {
                DbContext db = context as DbContext;
                if (db != null)
                {
                    var sprocExpression = GetSprocExpression(sprocName, parameterNamesToValues);
                    var parameterValues = GetParameterValues(parameterNamesToValues);
                    var result =
                        db.Database.SqlQuery<TReturn>(
                            @sprocExpression, parameterValues);
                    if (result != null && result.Any())
                    {
                        itemToReturn = result.ToList();
                    }
                }
            }
            return itemToReturn;

        }

        public virtual int ExecuteSqlCommand(string sprocName,
    Dictionary<string, object> parameterNamesToValues)
        {

            using (var context = new TContext())
            {
                DbContext db = context as DbContext;
                if (db != null)
                {
                    var sprocExpression = GetSprocExpression(sprocName, parameterNamesToValues);
                    var parameterValues = GetParameterValues(parameterNamesToValues);
                    var result =
                        db.Database.ExecuteSqlCommand(
                            sprocExpression, parameterValues);
                    return result;

                }
            }
            var outparam = parameterNamesToValues.Values.FirstOrDefault();
            return 0;

        }

        public virtual int ExecuteSqlCommand(string sprocName, Dictionary<string, object> parameterNamesToValues,
            DbContext context)
        {

            DbContext db = context as DbContext;
            if (db != null)
            {
                var sprocExpression = GetSprocExpression(sprocName, parameterNamesToValues);
                var parameterValues = GetParameterValues(parameterNamesToValues);
                var result =
                    db.Database.ExecuteSqlCommand(
                        sprocExpression, parameterValues);
                return result;
            }
            return 0;

        }


        private object[] GetParameterValues(Dictionary<string, object> parameterNamesToValues)
        {
            var parameterValues = parameterNamesToValues.Values.Any()
                        ? parameterNamesToValues.Values.ToArray()
                        : null;
            return parameterValues;
        }

        private string GetSprocExpression(string sprocName, Dictionary<string, object> parameterNamesToValues)
        {
            string sprocExpression = null;

            if (parameterNamesToValues != null && parameterNamesToValues.Any())
            {
                StringBuilder sb = new StringBuilder();
                int counter = 0;
                int length = parameterNamesToValues.Count();
                foreach (string name in parameterNamesToValues.Keys.ToList())
                {
                    sb.Append(" " + name + " = " + " {" + counter.ToString() + "}");

                    if (counter < (length - 1))
                    {
                        sb.Append(", ");
                    }
                    else
                        sb.Append(" ");
                    counter++;
                }
                var parameterizedValues = sb.ToString();
                sprocExpression += sprocName;
                sprocExpression += parameterizedValues;
            }
            else
            {
                sprocExpression += sprocName;
            }
            return sprocExpression;
        }


        public virtual void Add(TContext passContext, params T[] items)
        {
            Update(passContext, items);
        }

        public virtual void Update(TContext passContext, params T[] items)
        {

            context = passContext;
            DbContext db = context as DbContext;
            if (db != null)
            {
                DbSet<T> dbSet = db.Set<T>();
                foreach (T item in items)
                {
                    dbSet.Add(item);

                    foreach (DbEntityEntry<T> entry in db.ChangeTracker.Entries<T>())
                    {
                        IEntity entity = entry.Entity;
                        entry.State = GetEntityState(entity.EntityState);
                    }
                }

                db.SaveChanges();

            }

        }

        public virtual void Remove(TContext passContext, params T[] items)
        {
            Update(passContext, items);
        }

        public virtual List<T> UpdateAndReturnManipulated(TContext passContext, params T[] items)
        {

            context = passContext;
            //using (var context = new TContext())
            //{
            List<T> itemsManipulated = new List<T>();
            DbContext db = context as DbContext;
            if (db != null)
            {
                DbSet<T> dbSet = db.Set<T>();
                foreach (T item in items)
                {
                    itemsManipulated.Add(dbSet.Add(item));

                    foreach (DbEntityEntry<T> entry in db.ChangeTracker.Entries<T>())
                    {
                        IEntity entity = entry.Entity;
                        entry.State = GetEntityState(entity.EntityState);
                    }
                }
                db.SaveChanges();

            }
            // }
            return itemsManipulated;

        }

        public virtual void Add(params T[] items)
        {
            Update(items);
        }

        public virtual void Update(params T[] items)
        {
            using (var context = new TContext())
            {
                DbContext db = context as DbContext;

                if (db != null)
                {
                    DbSet<T> dbSet = db.Set<T>();

                    foreach (T item in items)
                    {
                        dbSet.Add(item);

                        foreach (DbEntityEntry<T> entry in db.ChangeTracker.Entries<T>())
                        {
                            IEntity entity = entry.Entity;
                            entry.State = GetEntityState(entity.EntityState);
                        }
                    }

                    db.SaveChanges();
                }
            }
        }

        public virtual void Remove(params T[] items)
        {
            Update(items);
        }

        public virtual List<T> UpdateAndReturnManipulated(params T[] items)
        {

            List<T> itemsManipulated = new List<T>();

            using (var context = new TContext())
            {
                DbContext db = context as DbContext;
                if (db != null)
                {
                    DbSet<T> dbSet = db.Set<T>();
                    foreach (T item in items)
                    {
                        itemsManipulated.Add(dbSet.Add(item));

                        foreach (DbEntityEntry<T> entry in db.ChangeTracker.Entries<T>())
                        {
                            IEntity entity = entry.Entity;
                            entry.State = GetEntityState(entity.EntityState);
                        }
                    }
                    db.SaveChanges();

                }
            }
            return itemsManipulated;

        }

        //note that the using block is assumed to be outside of this method and all the operations must call this method to be a part of transaction
        //and then pass readyToSave as true to attempt commit
        public virtual bool ProcessItemsInTransaction(TContext passedInContext, DbContextTransaction begunTransaction, bool readyToSave, params T[] items)
        {
            DbContext db = context as DbContext;

            if (db != null && begunTransaction != null)
            {

                //this handles the added/updated/deleted state of individual entities
                UpdateWithoutSave(db, items);



                if (readyToSave)
                {
                    db.SaveChanges();

                }

            }
            else
            {
                string error = "context could not be cast as dbcontext, it may be null," +
                          "or the user has passed a transaction that has not yet had BeginTransaction called in Assembly:,  "
                          + Assembly.GetExecutingAssembly().FullName + ", In Method Name:  " + System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new ApplicationException(error);
            }

            return true;

        }

        public virtual void UpdateWithoutSave(DbContext db, params T[] items)
        {

            if (db != null)
            {
                DbSet<T> dbSet = db.Set<T>();
                foreach (T item in items)
                {
                    foreach (DbEntityEntry<T> entry in db.ChangeTracker.Entries<T>())
                    {
                        IEntity entity = entry.Entity;
                        entry.State = GetEntityState(entity.EntityState);
                    }
                }
            }

        }



        protected static System.Data.Entity.EntityState GetEntityState(DotNetNancy.Generic.Data.Core.EntityState entityState)
        {
            switch (entityState)
            {
                case Core.EntityState.Unchanged:
                    return System.Data.Entity.EntityState.Unchanged;
                case Core.EntityState.Added:
                    return System.Data.Entity.EntityState.Added;
                case Core.EntityState.Modified:
                    return System.Data.Entity.EntityState.Modified;
                case Core.EntityState.Deleted:
                    return System.Data.Entity.EntityState.Deleted;
                default:
                    return System.Data.Entity.EntityState.Detached;
            }
        }

    }
}
