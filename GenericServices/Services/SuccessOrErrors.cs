using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;
using System.Linq;

namespace GenericServices.Services
{

    public class SuccessOrErrors : ISuccessOrErrors
    {

        private readonly List<string> _warnings = new List<string>();

        private List<ValidationResult> _errors;
        private string _successMessage;

        /// <summary>
        /// Holds the list of errors. Empty list means no errors.
        /// </summary>
        public IReadOnlyList<ValidationResult> Errors
        {
            get
            {
                if (_errors == null)
                    throw new NullReferenceException("The status must have an error set or the success message set before you can access errors.");
                return _errors;
            }
        }

        /// <summary>
        /// This returns any warning messages
        /// </summary>
        public IReadOnlyList<string> Warnings { get { return _warnings; }}

        /// <summary>
        /// Returns true if not errors or not validated yet, else false. 
        /// </summary>
        public bool IsValid { get { return (_errors != null && Errors.Count == 0); }}

        /// <summary>
        /// Returns true if not errors or not validated yet, else false. 
        /// </summary>
        public bool HasWarnings { get { return (_warnings.Count > 0); } }

        /// <summary>
        /// This returns the success message with suffix is nay warning messages
        /// </summary>
        public string SuccessMessage
        {
            get { return HasWarnings ? string.Format("{0} (has {1} warnings)",_successMessage,_warnings.Count  ) : _successMessage; }
        }

        //---------------------------------------------------
        //public methods


        /// <summary>
        /// Adds a warning message. It places the test 'Warning: ' before the message
        /// </summary>
        /// <param name="warningformat"></param>
        /// <param name="args"></param>
        public void AddWarning(string warningformat, params object[] args)
        {
            _warnings.Add("Warning: " + string.Format(warningformat, args));
        }

        /// <summary>
        /// This converts the Entity framework errors into Validation errors
        /// </summary>
        public ISuccessOrErrors SetErrors(IEnumerable<DbEntityValidationResult> errors)
        {
            _errors = new List<ValidationResult>();

            foreach (var errorsPerThisClass in errors)
                _errors.AddRange(errorsPerThisClass.ValidationErrors.Select(y => new ValidationResult(y.ErrorMessage, new[] { y.PropertyName })));

            _successMessage = string.Empty;
            return this;
        }

        /// <summary>
        /// This sets the error list to a series of non property specific error messages
        /// </summary>
        /// <param name="errors"></param>
        public ISuccessOrErrors SetErrors(IEnumerable<string> errors)
        {
            _errors = errors.Where(x => !string.IsNullOrEmpty(x)).Select(x => new ValidationResult(x)).ToList();
            _successMessage = string.Empty;
            return this;
        }

        /// <summary>
        /// Allows a single error to be added
        /// </summary>
        /// <param name="errorformat"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public ISuccessOrErrors AddSingleError(string errorformat, params object[] args)
        {
            if (_errors == null)
                _errors = new List<ValidationResult>();
            _errors.Add(new ValidationResult(string.Format(errorformat, args)));
            _successMessage = string.Empty;
            return this;
        }

        /// <summary>
        /// This adds an error for a specific, named parameter
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="errorformat"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public ISuccessOrErrors AddNamedParameterError(string parameterName, string errorformat, params object[] args)
        {
            if (_errors == null)
                _errors = new List<ValidationResult>();
            _errors.Add(new ValidationResult(string.Format(errorformat, args), new[] { parameterName }));
            _successMessage = string.Empty;
            return this;
        }

        /// <summary>
        /// This sets a success message and sets the IsValid flag to true
        /// </summary>
        /// <param name="successformat"></param>
        /// <param name="args"></param>
        public ISuccessOrErrors SetSuccessMessage(string successformat, params object [] args)
        {
            _errors = new List<ValidationResult>();         //empty list means its been validated and its Valid
            _successMessage = string.Format(successformat, args);
            return this;
        }

        /// <summary>
        /// This is a quick way to create an ISuccessOrErrors with a success message
        /// </summary>
        /// <param name="formattedSuccessMessage"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static ISuccessOrErrors Success(string formattedSuccessMessage, params object[] args)
        {
            return new SuccessOrErrors().SetSuccessMessage(string.Format(formattedSuccessMessage, args));
        }

        /// <summary>
        /// Useful one line error statement where brevity is needed
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (IsValid)
                return _successMessage ?? "The task completed successfully";

            return _errors == null 
                ? "Not currently setup" 
                : string.Format("Failed with {0} errors", _errors.Count);
        }

    }
}
