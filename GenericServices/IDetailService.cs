using System;
using System.Linq.Expressions;
using GenericServices.Core;
using GenericServices.Services;

namespace GenericServices
{
    public interface IDetailService
    {
        /// <summary>
        /// This returns a status which, if Valid, contains a single entry found using its primary keys.
        /// </summary>
        /// <typeparam name="T">The type of the data to output. 
        /// Type must be a type either an EF data class or one of the EfGenericDto's</typeparam>
        /// <param name="keys">The keys must be given in the same order as entity framework has them</param>
        /// <returns>Status. If valid Result holds data (not tracked), otherwise null</returns>
        ISuccessOrErrors<T> GetDetail<T>(params object[] keys) where T : class, new();
    }

}