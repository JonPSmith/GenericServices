#region licence
// The MIT License (MIT)
// 
// Filename: ServicesConfiguration.cs
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
using System;
using System.Collections.Generic;
using GenericServices.Logger;

namespace GenericServices
{
    /// <summary>
    /// This is the signiture of the method called if an exception is found in the RealiseSingleWithErrorChecking
    /// </summary>
    /// <param name="ex">the exception thrown</param>
    /// <param name="callingMethodName">the name of the calling method (can be used for logging)</param>
    /// <returns>error message, or null if no error</returns>
    public delegate string RealiseSingleException(Exception ex, string callingMethodName);

    /// <summary>
    /// This static class holds the GenericService configuration parts
    /// </summary>
    public class ServicesConfiguration
    {

        private static readonly Dictionary<int, string> PrivateSqlErrorDict = new Dictionary<int, string>
        {
            {547, "This operation failed because another data entry uses this entry."},         //constraint
            {2601, "One of the properties is marked as Unique index and there is already an entry with that value."} //cannot insert dup key in index
        };

        /// <summary>
        /// This is a constant noLogger. Used when no logging is needed.
        /// </summary>
        public static readonly IGenericLogger NoLoggerInstance = new NoLoggingGenericLogger();

        /// <summary>
        /// This contains the SqlErrorNumbers that will be caught by SaveChangesWithErrorChecking (sync and Async)
        /// </summary>
        public static IReadOnlyDictionary<int, string> SqlErrorDict { get { return PrivateSqlErrorDict; } }        
        
        /// <summary>
        /// This should be given a method that takes a string, to have the logger, and returns an IGenericLogger instance. 
        /// </summary>
        public static Func<string, IGenericLogger> SetLoggerMethod { private get; set; }

        /// <summary>
        /// This can be set to a method that is called in RealiseSingleWithErrorChecking when an exception occurs
        /// It should return a error string if it can decode the error for the user, otherwise should return null
        /// </summary>
        public static RealiseSingleException RealiseSingleExceptionMethod { internal get; set; }

        /// <summary>
        /// This returns the logger set in the Generic Service package
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IGenericLogger GetLogger( string name)
        {
            return SetLoggerMethod(name);
        }

        static ServicesConfiguration()
        {
            SetLoggerMethod = name => NoLoggerInstance;
        }

        //--------------------------------------------------
        //public methods

        /// <summary>
        /// This clears the SqlErrorDict of all entries
        /// </summary>
        public static void ClearSqlErrorDict()
        {
            PrivateSqlErrorDict.Clear();
        }

        /// <summary>
        /// This adds an entry to the SqlErrorDict
        /// </summary>
        /// <param name="sqlErrorNumber"></param>
        /// <param name="errorText"></param>
        public static void AddToSqlErrorDict(int sqlErrorNumber, string errorText)
        {
            if (PrivateSqlErrorDict.ContainsKey(sqlErrorNumber))
                PrivateSqlErrorDict[sqlErrorNumber] = errorText;
            else
                PrivateSqlErrorDict.Add(sqlErrorNumber, errorText);
        }

    }
}
