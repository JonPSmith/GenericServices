using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using GenericServices.Services;

[assembly: InternalsVisibleTo("Tests")]

namespace GenericServices.ServicesAsync
{

    public abstract class EfGenericDtoAsync<TData, TDto> : EfGenericDto<TData, TDto>
        where TData : class
        where TDto : EfGenericDtoAsync<TData, TDto>
    {

        protected internal override TData FindItemTracked(IDbContextWithValidation context)
        {
            throw new InvalidOperationException("This should not be called when in Async mode.");
        }

        /// <summary>
        /// This returns the TData item that fits the key(s) in the DTO.
        /// Override if you want to include other relationships for deep level updates
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        internal protected virtual async Task<TData> FindItemTrackedAsync(IDbContextWithValidation context)
        {
            return await context.Set<TData>().FindAsync(GetKeyValues());
        }

        //---------------------------------------------------------------
        //helper methods

        internal protected override TDto CreateDtoAndCopyDataIn(IDbContextWithValidation context, Expression<Func<TData, bool>> predicate)
        {
            throw new InvalidOperationException("This should not be called when in Async mode.");
        }

        /// <summary>
        /// This copies an existing TData into a new the dto using a Lambda expression to define the where clause
        /// It copies TData properties into all TDto properties that have accessable setters, i.e. not private
        /// </summary>
        /// <returns>dto, or null if not found</returns>
        internal protected virtual async Task<TDto> CreateDtoAndCopyDataInAsync(IDbContextWithValidation context, Expression<Func<TData, bool>> predicate)
        {
            Mapper.CreateMap<TData, TDto>();
            var dto = await GetDataUntracked(context).Where(predicate).Project().To<TDto>().SingleOrDefaultAsync();
            if (dto == null)
                throw new ArgumentException("We could not find an entry using the given predicate");

            return dto;
        }
    }
}
