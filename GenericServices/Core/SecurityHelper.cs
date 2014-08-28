using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace GenericServices.Core
{
    public static class SecurityHelper
    {
        static readonly int[] PermissionErrorNumbers = { 218, 219, 229, 230, 262, 297, 300 };

        /// <summary>
        /// This command can be appended to a ListService request. It will have the effect of
        /// turning the IQueryable request into an actual access of the database, thus forcing
        /// any permission errors that SQL security might  throw up if the current user is not 
        /// allowed to access the data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request">An IQeryable request that produces a collection</param>
        /// <param name="logIt">defaults to logging any permission error. Set to false to turn off logging</param>
        /// <returns>Returns task with status. If Valid then status.Result is the collection, otherwise an empty collection</returns>
        public static ISuccessOrErrors<ICollection<T>> TryManyWithPermissionChecking<T>(
            this IQueryable<T> request, bool logIt = true, [CallerMemberName] string methodName = "") where T : class
        {
            try
            {
                return new SuccessOrErrors<ICollection<T>>(request.ToList(), "successful");
            }
            catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
            {
                if (ex.IsExceptionAboutPermissions(logIt, methodName))
                    return
                        new SuccessOrErrors<ICollection<T>>(new List<T>(), "failed")
                            .AddSingleError("This access was not allowed.");

                throw;      //do not understand the error so rethrow
            }
        }

        /// <summary>
        /// This command can be appended to a ListService request. It will have the effect of
        /// turning the IQueryable request into an actual access of the database, thus forcing
        /// any permission errors that SQL security might  throw up if the current user is not 
        /// allowed to access the data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request">An IQeryable request that produces a collection</param>
        /// <param name="logIt">defaults to logging any permission error. Set to false to turn off logging</param>
        /// <returns>Returns task with status. If Valid then status.Result is the collection, otherwise an empty collection</returns>
        public static async Task<ISuccessOrErrors<ICollection<T>>> TryManyWithPermissionCheckingAsync<T>(
            this IQueryable<T> request, bool logIt = true, [CallerMemberName] string methodName = "") where T : class
        {
            try
            {
                return new SuccessOrErrors<ICollection<T>>(await request.ToListAsync(), "successful");
            }
            catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
            {
                if (ex.IsExceptionAboutPermissions(logIt, methodName))
                    return
                        new SuccessOrErrors<ICollection<T>>(new List<T>(), "failed")
                            .AddSingleError("This access was not allowed.");

                throw;      //do not understand the error so rethrow
            }
        }

        /// <summary>
        /// This will take an IQueryable request and add single on the end to realise the request.
        /// It catches if the single didn't produce an item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request">An IQueryable request with a filter that yeilds a single item</param>
        /// <param name="logIt">default to logging if there is a permission error. Set to false to stop logging</param>
        /// <param name="methodName">Do not specify. System fills this in with the calling method</param>
        /// <returns>Returns status. If Valid then status.Result is the single item, otherwise an new, empty class</returns>
        public static ISuccessOrErrors<T> TrySingleWithPermissionChecking<T>(
            this IQueryable<T> request, bool logIt = true, [CallerMemberName] string methodName = "") where T : class, new()
        {
            var status = new SuccessOrErrors<T>(new T(), "we return empty class if it fails");
            try
            {
                var result = request.SingleOrDefault();
                if (result == null)
                    status.AddSingleError(
                        "We could not find an entry using that filter. Has it been deleted by someone else?");
                else
                    status.SetSuccessWithResult(result, "successful");
            }
            catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
            {
                if (ex.IsExceptionAboutPermissions(logIt, methodName))
                    status.AddSingleError("This access was not allowed.");
                else
                    throw; //do not understand the error so rethrow
            }
            return status;
        }

        /// <summary>
        /// This will take an IQueryable request and add single on the end to realise the request.
        /// It catches if the single didn't produce an item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request">An IQueryable request with a filter that yeilds a single item</param>
        /// <param name="logIt">default to logging if there is a permission error. Set to false to stop logging</param>
        /// <param name="methodName">Do not specify. System fills this in with the calling method</param>
        /// <returns>Returns task with status. If Valid then status.Result is the single item, otherwise an new, empty class</returns>
        public static async Task<ISuccessOrErrors<T>> TrySingleWithPermissionCheckingAsync<T>(
            this IQueryable<T> request, bool logIt = true, [CallerMemberName] string methodName = "") where T : class, new()
        {
            var status = new SuccessOrErrors<T>(new T(), "we return empty class if it fails");
            try
            {
                var result = await request.SingleOrDefaultAsync();
                if (result == null)
                    status.AddSingleError(
                        "We could not find an entry using that filter. Has it been deleted by someone else?");
                else
                    status.SetSuccessWithResult(result, "successful");
            }
            catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
            {
                if (ex.IsExceptionAboutPermissions(logIt, methodName))
                {
                    status.SetSuccessWithResult(new T(), "failed");         //we return an empty class if it fails
                    status.AddSingleError("This access was not allowed.");
                }
                else
                    throw;      //do not understand the error so rethrow
            }
            return status;
        }

        /// <summary>
        /// This will return true if the SqlException contains an error that is about permissions
        /// </summary>
        /// <param name="sqlException">sqlException to check</param>
        /// <param name="methodName">name of method that called the action that caused the exception</param>
        /// <param name="logIt">default is true which logs the permission exception. set to false to stop logging</param>
        /// <returns></returns>
        public static bool CheckSqlExceptionForPermissions(this SqlException sqlException, string methodName, bool logIt = true)
        {
            for (int i = 0; i < sqlException.Errors.Count; i++)
            {
                if (!PermissionErrorNumbers.Contains(sqlException.Errors[i].Number)) continue;

                //yes. we have found a permission exception
                if (!logIt) return true;

                var logger = GenericLoggerFactory.GetLogger("SqlSecurity");
                logger.ErrorFormat("Calling method {0} caused SqlException: {1}", methodName, sqlException.Message);
                return true;
            }
            return false;
        }


        //--------------------------------------------------
        //private helpers

        private static bool IsExceptionAboutPermissions(this Exception ex, bool logIt, string methodName)
        {

            var sqlException = ex.FindDeepestInnerException() as SqlException;
            return sqlException != null && sqlException.CheckSqlExceptionForPermissions(methodName, logIt);
        }

        private static Exception FindDeepestInnerException(this Exception ex)
        {
            while (ex.InnerException != null) { ex = ex.InnerException; }
            return ex;
        }

    }
}
