#region licence
// The MIT License (MIT)
// 
// Filename: ActionServiceHelper.cs
// Date Created: 2014/07/09
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

using System.Threading.Tasks;
using GenericServices.Core;

namespace GenericServices.Actions.Internal
{
   internal static class ActionServiceHelper
    {
       public static ISuccessOrErrors<TActionOut> SaveChangesAttempt<TActionOut>(this ISuccessOrErrors<TActionOut> status, object actionInDto, IGenericServicesDbContext db)
        {
            if (status.ShouldStopAsWarningsMatter(actionInDto))
                //There were warnings and we are asked to not write to the database
                return status.UpdateSuccessMessage("{0}... but NOT written to database as warnings.",
                    status.SuccessMessage);

            //we now need to save the changes to the database
            var dataStatus = db.SaveChangesWithChecking();
            return dataStatus.IsValid
                ? status.UpdateSuccessMessage("{0}... and written to database.", status.SuccessMessage)
                : SuccessOrErrors<TActionOut>.ConvertNonResultStatus(dataStatus);
        }

       public static async Task<ISuccessOrErrors<TActionOut>> SaveChangesAttemptAsync<TActionOut>(this ISuccessOrErrors<TActionOut> status, object actionInDto, IGenericServicesDbContext db)
        {

            if (status.ShouldStopAsWarningsMatter(actionInDto))
                //There were warnings and we are asked to not write to the database
                return status.UpdateSuccessMessage("{0}... but NOT written to database as warnings.",
                    status.SuccessMessage);

            //we now need to save the changes to the database
            var dataStatus = await db.SaveChangesWithCheckingAsync();
            return dataStatus.IsValid
                ? status.UpdateSuccessMessage("{0}... and written to database.", status.SuccessMessage)
                : SuccessOrErrors<TActionOut>.ConvertNonResultStatus(dataStatus);
        }

        public static bool AskedToSaveChanges<TActionOut>(this ISuccessOrErrors<TActionOut> status, IActionBase actionToRun)
        {
            return status.IsValid && actionToRun.SubmitChangesOnSuccess;
        }

        //------------------------------------------------
        //private helpers

        private static bool ShouldStopAsWarningsMatter<TActionOut>(this ISuccessOrErrors<TActionOut> status, object classToCheck)
        {
            if (!status.HasWarnings) return false;
            var flagClass = classToCheck as ICheckIfWarnings;
            return (flagClass != null && !flagClass.WriteEvenIfWarning);
        }
    }
}
