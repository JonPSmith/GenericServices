using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;

namespace GenericServices
{
    public interface ISuccessOrErrors
    {
        /// <summary>
        /// Holds the list of errors. Empty list means no errors. Null means validation has not been done
        /// </summary>
        IReadOnlyList<ValidationResult> Errors { get; }

        /// <summary>
        /// Returns true if not errors or not validated yet, else false. 
        /// </summary>
        bool IsValid { get; }

        string SuccessMessage { get; }

        /// <summary>
        /// This converts the Class level errors to ValidationResults and sets the error list to them.
        /// </summary>
        /// <param name="errors">list of IValidationErrorsPerClass to add. If null then no errors</param>
        ISuccessOrErrors SetErrors(IEnumerable<DbEntityValidationResult> errors);

        /// <summary>
        /// This sets the error list to a series of non property specific error messages
        /// </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
        ISuccessOrErrors SetErrors(IEnumerable<string> errors);

        /// <summary>
        /// Allows a single error to be set.
        /// </summary>
        /// <param name="errorformat"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        ISuccessOrErrors AddSingleError(string errorformat, params object[] args);

        /// <summary>
        /// This adds an error for a specific, named parameter
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="errorformat"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        ISuccessOrErrors AddNamedParameterError(string parameterName, string errorformat, params object[] args);

        /// <summary>
        /// This sets a success message and sets the IsValid flag to true
        /// </summary>
        /// <param name="successformat"></param>
        /// <param name="args"></param>
        ISuccessOrErrors SetSuccessMessage(string successformat, params object[] args);
    }
}
