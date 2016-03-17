#region licence
// The MIT License (MIT)
// 
// Filename: EfGenericDtoAsync.cs
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
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using GenericLibsBase;
using GenericLibsBase.Core;

[assembly: InternalsVisibleTo("Tests")]

namespace GenericServices.Core
{

    public abstract class EfGenericDtoAsync<TEntity, TDto> : EfGenericDtoBase<TEntity, TDto>
        where TEntity : class, new()
        where TDto : EfGenericDtoAsync<TEntity, TDto>, new()
    {
      
        /// <summary>
        /// This function will be called at the end of CreateSetupService and UpdateSetupService to setup any
        /// additional data in the dto used to display dropdownlists etc. 
        /// It is also called at the end of the CreateService or UpdateService if there are errors, so that
        /// the data is available if the form needs to be reshown.
        /// This function should be overridden if the dto needs additional data setup 
        /// </summary>
        /// <returns></returns>
        protected internal virtual async Task SetupSecondaryDataAsync(IGenericServicesDbContext db, TDto dto)
        {
            if (!SupportedFunctions.HasFlag(CrudFunctions.DoesNotNeedSetup))
                throw new InvalidOperationException("SupportedFunctions flags say that setup of secondary data is needed, but did not override the SetupSecondaryData method.");
        }

        /// <summary>
        /// This is used for update. This returns the TEntity item that fits the key(s) in the DTO.
        /// Override this if you need to include any related entries when doing a complex update.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected internal virtual async Task<TEntity> FindItemTrackedForUpdateAsync(IGenericServicesDbContext context)
        {
            return await context.Set<TEntity>().FindAsync(GetKeyValues(context)).ConfigureAwait(false);
        }

        /// <summary>
        /// This is used in a create. It copies only the properties in TDto that have public setter into the TEntity.
        /// You can override this if you need a more complex copy
        /// </summary>
        /// <param name="context"></param>
        /// <param name="source"></param>
        /// <returns>Task containing status which, if Valid, has new TEntity with data from DTO copied in</returns>
        protected internal virtual async Task<ISuccessOrErrors<TEntity>> CreateDataFromDtoAsync(IGenericServicesDbContext context, TDto source)
        {
            var result = new TEntity();
            Mapper.Map(source, result);
            return new SuccessOrErrors<TEntity>(result, "Successful copy of data");
        }

        /// <summary>
        /// This is used in an update. It copies only the properties in TDto that do not have the [DoNotCopyBackToDatabase] on them.
        /// You can override this if you need a more complex copy
        /// </summary>
        /// <param name="context"></param>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <return>Task containing status. destination is only valid if status.IsValid</return>
        protected internal virtual async Task<ISuccessOrErrors> UpdateDataFromDtoAsync(IGenericServicesDbContext context, TDto source, TEntity destination)
        {
            Mapper.Map(source, destination);
            return SuccessOrErrors.Success("Successful copy of data");
        }

        /// <summary>
        /// This copies an existing TEntity into a new the dto using a Lambda expression to define the where clause
        /// It copies TEntity properties into all TDto properties that have accessable setters, i.e. not private
        /// </summary>
        /// <returns>status. If Valid then dto, otherwise null</returns>
        protected internal virtual async Task<ISuccessOrErrors<TDto>> DetailDtoFromDataInAsync(
            IGenericServicesDbContext context, Expression<Func<TEntity, bool>> predicate)
        {
            var query = GetDataUntracked(context).Where(predicate).ProjectTo<TDto>(AutoMapperConfigs[CreateDictionaryKey<TEntity, TDto>()]);

            //We check if we need to decompile the LINQ expression so that any computed properties in the class are filled in properly
            return await ApplyDecompileIfNeeded(query).RealiseSingleWithErrorCheckingAsync().ConfigureAwait(false);
        }
    }
}
