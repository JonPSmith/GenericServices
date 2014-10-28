#region licence
// The MIT License (MIT)
// 
// Filename: SuccessOrErrors.cs
// Date Created: 2014/06/24
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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;

namespace GenericLibsBase.Core
{

    public class SuccessOrErrors<T> : SuccessOrErrors, ISuccessOrErrors<T>
    {
        /// <summary>
        /// Holds the value set using SetSuccessWithResult
        /// </summary>
        public T Result { get; private set; }


        //ctors
        public SuccessOrErrors() :base() {}

        public SuccessOrErrors(T result, string successformat, params object[] args) : base()
        {
            Result = result;
            base.SetSuccessMessage(successformat, args);
        }

        private SuccessOrErrors(ISuccessOrErrors nonResultStatus)
            : base(nonResultStatus) {}


        /// <summary>
        /// This sets a successful end by setting the Result and supplying a success message 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="successformat"></param>
        /// <param name="args"></param>
        public ISuccessOrErrors<T> SetSuccessWithResult(T result, string successformat, params object[] args)
        {
            Result = result;
            base.SetSuccessMessage(successformat, args);
            return this;
        }

        public new ISuccessOrErrors<T> SetErrors(IEnumerable<DbEntityValidationResult> errors)
        {
            base.SetErrors(errors);
            return this;
        }

        public new ISuccessOrErrors<T> SetErrors(IEnumerable<string> errors)
        {
            base.SetErrors(errors);
            return this;
        }

        public new ISuccessOrErrors<T> AddSingleError(string errorformat, params object[] args)
        {
            base.AddSingleError(errorformat, args);
            return this;
        }

        public new ISuccessOrErrors<T> AddNamedParameterError(string parameterName, string errorformat, params object[] args)
        {
            base.AddNamedParameterError(parameterName, errorformat, args);
            return this;
        }

        /// <summary>
        /// This allows the current success message to be updated
        /// </summary>
        /// <param name="successformat"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public ISuccessOrErrors<T> UpdateSuccessMessage(string successformat, params object[] args)
        {
            base.SetSuccessMessage(successformat, args);
            return this;
        }

        /// <summary>
        /// Turns the non result status into a result status by copying any errors or warnings
        /// </summary>
        /// <param name="nonResultStatus"></param>
        /// <returns></returns>
        public static ISuccessOrErrors<T> ConvertNonResultStatus(ISuccessOrErrors nonResultStatus)
        {
            return new SuccessOrErrors<T>(nonResultStatus);
        }

        /// <summary>
        /// This is a quick way to create an ISuccessOrErrors(T) with a success message and result
        /// </summary>
        /// <param name="result"></param>
        /// <param name="formattedSuccessMessage"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static ISuccessOrErrors<T> SuccessWithResult(T result, string formattedSuccessMessage, params object[] args)
        {
            return new SuccessOrErrors<T>().SetSuccessWithResult(result, string.Format(formattedSuccessMessage, args));
        }

    }



    public class SuccessOrErrors : ISuccessOrErrors
    {
        private readonly List<string> _localWarnings = new List<string>();
        private List<ValidationResult> _localErrors;
        private string _localSuccessMessage;

        public SuccessOrErrors() { }

        protected SuccessOrErrors(ISuccessOrErrors nonResultStatus)
        {
            var status = (SuccessOrErrors)nonResultStatus;
            _localWarnings = status._localWarnings;
            _localErrors = status._localErrors;
        }

        /// <summary>
        /// Holds the list of errors. Empty list means no errors.
        /// </summary>
        public IReadOnlyList<ValidationResult> Errors
        {
            get
            {
                if (_localErrors == null)
                    throw new InvalidOperationException("The status must have an error set or the success message set before you can access errors.");
                return _localErrors;
            }
        }

        /// <summary>
        /// This returns any warning messages
        /// </summary>
        public IReadOnlyList<string> Warnings { get { return _localWarnings; }}

        /// <summary>
        /// Returns true if not errors or not validated yet, else false. 
        /// </summary>
        public bool IsValid { get { return (_localErrors != null && Errors.Count == 0); }}

        /// <summary>
        /// Returns true if not errors or not validated yet, else false. 
        /// </summary>
        public bool HasWarnings { get { return (_localWarnings.Count > 0); } }

        /// <summary>
        /// This returns the success message with suffix is nay warning messages
        /// </summary>
        public string SuccessMessage
        {
            get { return HasWarnings ? string.Format("{0} (has {1} warnings)",_localSuccessMessage,_localWarnings.Count  ) : _localSuccessMessage; }
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
            _localWarnings.Add("Warning: " + string.Format(warningformat, args));
        }

        /// <summary>
        /// This converts the Entity framework errors into Validation errors
        /// </summary>
        public ISuccessOrErrors SetErrors(IEnumerable<DbEntityValidationResult> errors)
        {
            _localErrors = errors.SelectMany(
                    x => x.ValidationErrors.Select(y => new ValidationResult(y.ErrorMessage, new[] { y.PropertyName })))
                    .ToList();

            _localSuccessMessage = string.Empty;
            return this;
        }

        /// <summary>
        /// Copies in validation errors found outside into the status
        /// </summary>
        public ISuccessOrErrors SetErrors(IEnumerable<ValidationResult> errors)
        {
            _localErrors = errors.ToList();
            _localSuccessMessage = string.Empty;
            return this;
        }

        /// <summary>
        /// This sets the error list to a series of non property specific error messages
        /// </summary>
        /// <param name="errors"></param>
        public ISuccessOrErrors SetErrors(IEnumerable<string> errors)
        {
            _localErrors = errors.Where(x => !string.IsNullOrEmpty(x)).Select(x => new ValidationResult(x)).ToList();
            _localSuccessMessage = string.Empty;
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
            if (_localErrors == null)
                _localErrors = new List<ValidationResult>();
            _localErrors.Add(new ValidationResult(string.Format(errorformat, args)));
            _localSuccessMessage = string.Empty;
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
            if (_localErrors == null)
                _localErrors = new List<ValidationResult>();
            _localErrors.Add(new ValidationResult(string.Format(errorformat, args), new[] { parameterName }));
            _localSuccessMessage = string.Empty;
            return this;
        }

        /// <summary>
        /// This sets a success message and sets the IsValid flag to true
        /// </summary>
        /// <param name="successformat"></param>
        /// <param name="args"></param>
        public virtual ISuccessOrErrors SetSuccessMessage(string successformat, params object [] args)
        {
            _localErrors = new List<ValidationResult>();         //empty list means its been validated and its Valid
            _localSuccessMessage = string.Format(successformat, args);
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
                return _localSuccessMessage ?? "The task completed successfully";

            return _localErrors == null 
                ? "Not currently setup"
                : string.Format("Failed with {0} error{1}", _localErrors.Count, _localErrors.Count > 1 ? "s" : string.Empty);
        }

        /// <summary>
        /// This returns the errors as:
        /// If only one error then as a html p 
        /// If multiple errors then as an unordered list
        /// </summary>
        /// <returns>simple html data without any classes</returns>
        public string ErrorsAsHtml()
        {
            if (IsValid)
                throw new InvalidOperationException("You should not call this if there are no errors.");

            if (Errors.Count == 1)
                return string.Format("<p>{0}{1}</p>", FormatParamNames(Errors[0]), Errors[0].ErrorMessage);

            var sb = new StringBuilder("<ul>");
            foreach (var validationResult in Errors)
            {
                sb.AppendFormat("<li>{0}{1}</li>", FormatParamNames(validationResult), validationResult.ErrorMessage);
            }
            sb.Append("</ul>");

            return sb.ToString();
        }

        private string FormatParamNames(ValidationResult validationResult)
        {
            if (validationResult.MemberNames.Any())
                return string.Join(",", validationResult.MemberNames) + ": ";

            return string.Empty;
        }

    }
}
