#region licence
// The MIT License (MIT)
// 
// Filename: SecurityHelper.cs
// Date Created: 2014/08/26
// 
// Copyright (c) 2014 Jon Smith (www.selectiveanalytics.com & www.thereformedprogrammer.net)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion
using System;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GenericLibsBase;
using GenericLibsBase.Core;

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
                if (GenericServicesConfig.RealiseSingleExceptionMethod == null) throw;      //nothing to catch error

                var errMsg = GenericServicesConfig.RealiseSingleExceptionMethod(ex, methodName);
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
                var result = await request.SingleOrDefaultAsync().ConfigureAwait(false);
                if (result == null)
                    status.AddSingleError(
                        "We could not find an entry using that filter. Has it been deleted by someone else?");
                else
                    status.SetSuccessWithResult(result, "successful");
            }
            catch (Exception ex)
            {
                if (GenericServicesConfig.RealiseSingleExceptionMethod == null) throw;      //nothing to catch error

                var errMsg = GenericServicesConfig.RealiseSingleExceptionMethod(ex, methodName);
                if (errMsg != null)
                    status.AddSingleError(errMsg);
                else
                    throw; //do not understand the error so rethrow
            }
            return status;
        }

    }
}
