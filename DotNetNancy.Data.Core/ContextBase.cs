using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace NextGear.Generic.Data.Core
{
    public class ContextBase : DbContext
    {
        protected static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ContextBase(string connectionStringName)
            : base(connectionStringName)
        {
            Configuration.UseDatabaseNullSemantics = true;
            Configuration.ProxyCreationEnabled = false;
        }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                Log.Error(ex);
                var dbentityvalidationexception = new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                Log.Error(dbentityvalidationexception);
                throw dbentityvalidationexception;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

    }

}
