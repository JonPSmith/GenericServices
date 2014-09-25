using System.Threading.Tasks;
using GenericServices.Core;

namespace GenericServices.Actions.Internal
{
   internal static class ActionServiceHelper
    {
        public static ISuccessOrErrors<TActionOut> SaveChangesAttempt<TActionOut>(this ISuccessOrErrors<TActionOut> status, object actionData, IGenericServicesDbContext db)
        {
            if (status.ShouldStopAsWarningsMatter(actionData))
                //There were warnings and we are asked to not write to the database
                return status.UpdateSuccessMessage("{0}... but NOT written to database as warnings.",
                    status.SuccessMessage);

            //we now need to save the changes to the database
            var dataStatus = db.SaveChangesWithChecking();
            return dataStatus.IsValid
                ? status.UpdateSuccessMessage("{0}... and written to database.", status.SuccessMessage)
                : SuccessOrErrors<TActionOut>.ConvertNonResultStatus(dataStatus);
        }

        public static async Task<ISuccessOrErrors<TActionOut>> SaveChangesAttemptAsync<TActionOut>(this ISuccessOrErrors<TActionOut> status, object actionData, IGenericServicesDbContext db)
        {

            if (status.ShouldStopAsWarningsMatter(actionData))
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
