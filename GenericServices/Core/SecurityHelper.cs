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
        /// <param name="request"></param>
        /// <param name="logIt">defaults to logging any permission error. Set to false to turn off logging</param>
        /// <returns></returns>
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
        /// <param name="request"></param>
        /// <param name="logIt">defaults to logging any permission error. Set to false to turn off logging</param>
        /// <returns></returns>
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
        /// <param name="request"></param>
        /// <param name="logIt"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public static ISuccessOrErrors<T> TrySingleWithPermissionChecking<T>(
            this IQueryable<T> request, bool logIt = true, [CallerMemberName] string methodName = "") where T : class
        {
            var status = new SuccessOrErrors<T>();
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
                    throw;      //do not understand the error so rethrow
            }
            return status;
        }

        public static async Task<ISuccessOrErrors<T>> TrySingleWithPermissionCheckingAsync<T>(
            this IQueryable<T> request, bool logIt = true, [CallerMemberName] string methodName = "") where T : class
        {
            var status = new SuccessOrErrors<T>();
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
                    status.AddSingleError("This access was not allowed.");
                else
                    throw;      //do not understand the error so rethrow
            }
            return status;
        }


        //--------------------------------------------------


        private static bool IsExceptionAboutPermissions(this Exception ex, bool logIt, string methodName)
        {

            var sqlException = ex.FindDeepestInnerException() as SqlException;
            if (sqlException == null) return false;

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

        private static Exception FindDeepestInnerException(this Exception ex)
        {
            while (ex.InnerException != null) { ex = ex.InnerException; }
            return ex;
        }

    }
}
