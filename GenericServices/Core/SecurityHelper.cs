using System;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace GenericServices.Core
{
    public static class SecurityHelper
    {

        /// <summary>
        /// This will take an IQueryable request and add single on the end to realise the request.
        /// It catches if the single didn't produce an item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request">An IQueryable request with a filter that yeilds a single item</param>
        /// <param name="methodName">Do not specify. System fills this in with the calling method</param>
        /// <returns>Returns status. If Valid then status.Result is the single item, otherwise an new, empty class</returns>
        public static ISuccessOrErrors<T> RealiseSingleWithErrorChecking<T>(this IQueryable<T> request, [CallerMemberName] string methodName = "") where T : class, new()
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
            catch (Exception ex)
            {
                if (ServicesConfiguration.RealiseSingleExceptionMethod == null) throw;      //nothing to catch error

                var errMsg = ServicesConfiguration.RealiseSingleExceptionMethod(ex, methodName);
                if (errMsg != null)
                    status.AddSingleError(errMsg);
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
        /// <param name="methodName">Do not specify. System fills this in with the calling method</param>
        /// <returns>Returns task with status. If Valid then status.Result is the single item, otherwise an new, empty class</returns>
        public static async Task<ISuccessOrErrors<T>> RealiseSingleWithErrorCheckingAsync<T>(this IQueryable<T> request, [CallerMemberName] string methodName = "") where T : class, new()
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
            catch (Exception ex)
            {
                if (ServicesConfiguration.RealiseSingleExceptionMethod == null) throw;      //nothing to catch error

                var errMsg = ServicesConfiguration.RealiseSingleExceptionMethod(ex, methodName);
                if (errMsg != null)
                    status.AddSingleError(errMsg);
                else
                    throw; //do not understand the error so rethrow
            }
            return status;
        }

    }
}
