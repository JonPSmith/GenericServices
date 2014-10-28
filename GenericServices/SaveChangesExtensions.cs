#region licence
// The MIT License (MIT)
// 
// Filename: SaveChangesExtensions.cs
// Date Created: 2014/09/25
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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using GenericLibsBase;
using GenericLibsBase.Core;
using GenericServices.Core;

namespace GenericServices
{
    /// <summary>
    /// This contains extention errors that change SaveChanges/SvaeChangesAsync into returning error messages
    /// rather than an exception on certain types of errors.
    /// </summary>
    public static class SaveChangesExtensions
    {
        /// <summary>
        /// This extension will undertake a SaveChanges but will catch any validation errors 
        /// or specific SqlException specified in ServicesConfiguration.SqlErrorDict and return
        /// them as errors rather than throw an exception
        /// </summary>
        /// <param name="db"></param>
        /// <returns>a status saying whether SaveChanges was successful or not. If not then holds errors</returns>
        public static ISuccessOrErrors SaveChangesWithChecking(this IGenericServiceSaveChanges db)
        {
            var result = new SuccessOrErrors();
            var numChanges = 0;
            try
            {
                numChanges = db.SaveChanges(); //then update it
            }
            catch (DbEntityValidationException ex)
            {
                return result.SetErrors(ex.EntityValidationErrors);
            }
            catch (DbUpdateException ex)
            {
                var decodedErrors = TryDecodeDbUpdateException(ex);
                if (decodedErrors == null)
                    throw; //it isn't something we understand

                return result.SetErrors(decodedErrors);
            }

            return result.SetSuccessMessage("Successfully added or updated {0} items", numChanges);
        }

        /// <summary>
        /// This extension will undertake a SaveChangesAsync but will catch any validation errors 
        /// or specific SqlException specified in ServicesConfiguration.SqlErrorDict and return
        /// them as errors
        /// </summary>
        /// <param name="db"></param>
        /// <returns>Task containing status saying whether SaveChanges was successful or not. If not then holds errors</returns>
        public static async Task<ISuccessOrErrors> SaveChangesWithCheckingAsync(this IGenericServiceSaveChanges db)
        {
            var result = new SuccessOrErrors();
            var numChanges = 0;
            try
            {
                numChanges = await db.SaveChangesAsync(); //then update it
            }
            catch (DbEntityValidationException ex)
            {
                return result.SetErrors(ex.EntityValidationErrors);
            }
            catch (DbUpdateException ex)
            {
                var decodedErrors = TryDecodeDbUpdateException(ex);
                if (decodedErrors == null)
                    throw; //it isn't something we understand

                return result.SetErrors(decodedErrors);
            }

            return result.SetSuccessMessage("Successfully added or updated {0} items", numChanges);
        }

        /// <summary>
        /// This decodes the DbUpdateException. If there are any errors it can
        /// handle then it returns a list of errors. Otherwise it returns null
        /// which means rethrow the error as it has not been handled
        /// </summary>
        /// <param name="ex"></param>
        /// <returns>null if cannot handle errors, otherwise a list of errors</returns>
        private static IEnumerable<ValidationResult> TryDecodeDbUpdateException(DbUpdateException ex)
        {
            if (!(ex.InnerException is System.Data.Entity.Core.UpdateException) ||
                !(ex.InnerException.InnerException is System.Data.SqlClient.SqlException))
                return null;

            var sqlException = (System.Data.SqlClient.SqlException)ex.InnerException.InnerException;
            var result = new List<ValidationResult>();
            for (int i = 0; i < sqlException.Errors.Count; i++)
            {
                var errorNum = sqlException.Errors[i].Number;
                string errorText;
                if (GenericServicesConfig.SqlErrorDict.TryGetValue(errorNum, out errorText))
                    result.Add(new ValidationResult(errorText));
            }
            return result.Any() ? result : null;
        }

    }
}
