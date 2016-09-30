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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
using AutoMapper;

namespace GenericServices
{
    /// <summary>
    /// This is the signiture of the method called on a SqlException happening in SaveChangesWithChecking (sync and async)
    /// </summary>
    /// <param name="exception">This is the Sql Exception that occured</param>
    /// <param name="entitiesThatErrored">DbEntityEntry objects that represents the entities that could not be saved to the database</param>
    /// <returns>return ValidationResult with error, or null if cannot handle this error</returns>
    public delegate ValidationResult HandleSqlException(
        System.Data.SqlClient.SqlException exception, IEnumerable<DbEntityEntry> entitiesThatErrored);

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
    public static class GenericServicesConfig
    {

        private static readonly Dictionary<int, HandleSqlException> PrivateSqlHandlerDict = new Dictionary<int, HandleSqlException>();

        private static readonly Dictionary<int, string> PrivateSqlErrorDict = new Dictionary<int, string>
        {
            {547, "This operation failed because another data entry uses this entry."},         //constraint
            {2601, "One of the properties is marked as Unique index and there is already an entry with that value."} //cannot insert dup key in index
        };

        private static bool _useDelegateDecompilerWhereNeeded = true;

        //This holds all the AutoMapper configs
        internal static readonly ConcurrentDictionary<string, MapperConfiguration> AutoMapperConfigs =
            new ConcurrentDictionary<string, MapperConfiguration>();

        /// <summary>
        /// This contains the SqlErrorNumbers that will be caught by SaveChangesWithChecking (sync and Async)
        /// </summary>
        public static IReadOnlyDictionary<int, string> SqlErrorDict { get { return PrivateSqlErrorDict; } }

        /// <summary>
        /// This contains the HandleSqlException methods by Sql Error number that will be caught by SaveChangesWithChecking (sync and Async)
        /// </summary>
        public static IReadOnlyDictionary<int, HandleSqlException> SqlHandlerDict { get { return PrivateSqlHandlerDict; } }

        /// <summary>
        /// This can be set to a method that is called in RealiseSingleWithErrorChecking when an exception occurs.
        /// RealiseSingleWithErrorChecking is used when a single DTO/Enity is asked for inside DetailService and
        /// UpdateSetupService, plus RealiseSingleWithErrorChecking can be used as an extension. 
        /// The method you provide should return a error string if it can decode the error for the user, otherwise should return null
        /// </summary>
        public static RealiseSingleException RealiseSingleExceptionMethod { internal get; set; }

        /// <summary>
        /// Set this if you want Generic Services to use the DelegateDecompiler. See documentation for more information
        /// </summary>
        public static bool UseDelegateDecompilerWhereNeeded
        {
            get { return _useDelegateDecompilerWhereNeeded; }
            set { _useDelegateDecompilerWhereNeeded = value; }
        }

        //--------------------------------------------------
        //public methods

        /// <summary>
        /// This clears any AutoMapper mappings. Used when Unit Testing to ensure the mappings are newly set up.
        /// </summary>
        public static void ClearAutoMapperCache()
        {
           AutoMapperConfigs.Clear();
        }

        /// <summary>
        /// This clears the SqlErrorDict of all entries
        /// </summary>
        public static void ClearSqlErrorDict()
        {
            PrivateSqlErrorDict.Clear();
        }

        /// <summary>
        /// This clears the SqlHandlerDict of all entries
        /// </summary>
        public static void ClearSqlHandlerDict()
        {
            PrivateSqlHandlerDict.Clear();
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

        /// <summary>
        /// This adds an ErrorHandler to the SqlHandlerDict
        /// The ErrorHandler will be called if the specified sql error happens.
        /// Note: will throw an exception if an error handler already exists for that sql error number unless 
        /// the checkNotAlreadySet is set to false
        /// </summary>
        /// <param name="sqlErrorNumber"></param>
        /// <param name="errorHandler">Called when given sql error number happens with sql error and entities. 
        /// Should return ValidationError or null if cannot handle the error</param>
        /// <param name="checkNotAlreadySet"></param>
        public static void AddToSqlHandlerDict(int sqlErrorNumber, HandleSqlException errorHandler, bool checkNotAlreadySet = true)
        {
            if (PrivateSqlHandlerDict.ContainsKey(sqlErrorNumber))
            {
                if (checkNotAlreadySet)
                    throw new InvalidOperationException(
                        string.Format("You tried to add an exception handler for sql error {0} but a handler called {1} was already there.",
                        sqlErrorNumber, PrivateSqlHandlerDict[sqlErrorNumber].Method.Name));
                PrivateSqlHandlerDict[sqlErrorNumber] = errorHandler;
            }
            else
                PrivateSqlHandlerDict.Add(sqlErrorNumber, errorHandler);
        }

    }
}
