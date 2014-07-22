using GenericServices.Core;

namespace GenericServices
{

    public interface IUpdateService
    {
        /// <summary>
        /// This updates the data in the database using the input data
        /// </summary>
        /// <typeparam name="T">The type of input data. 
        /// Type must be a type either an EF data class or one of the EfGenericDto's</typeparam>
        /// <param name="data">data to update the class. If Dto then copied over to data class</param>
        /// <returns></returns>
        ISuccessOrErrors Update<T>(T data) where T : class;
    }

}