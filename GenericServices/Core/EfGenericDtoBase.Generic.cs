#region licence
// The MIT License (MIT)
// 
// Filename: EfGenericDtoBase.Generic.cs
// Date Created: 2014/07/21
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
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DelegateDecompiler.EntityFramework;
using GenericServices.Core.Internal;

[assembly: InternalsVisibleTo("Tests")]

namespace GenericServices.Core
{
    /// <summary>
    /// This should not be used. It is used as the base for EfGenericDto and EfGenericDtoAsync
    /// and contained all the common methods/properties
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TDto"></typeparam>
    public abstract partial class EfGenericDtoBase<TEntity, TDto>
        where TEntity : class
        where TDto : EfGenericDtoBase<TEntity, TDto>
    {
        
        /// <summary>
        /// This provides the name of the name of the data item to display in success or error messages.
        /// Override if you want a more user friendly name
        /// </summary>
        internal protected virtual string DataItemName { get { return typeof (TEntity).Name; }}

        /// <summary>
        /// Override this to add .ForEach mappings that will be applied to the TEntity to TDto conversion
        /// See 'DTO data copying, Using AutoMapper for calculated properties' in the documentation 
        /// </summary>
        protected virtual Action<IMappingExpression<TEntity, TDto>> AddedDatabaseToDtoMapping { get { return null; } }

        /// <summary>
        /// Override this if your dto relies on another dto in its mapping
        /// For instance if you are mapping a property that is a type and you want that to map to a dto then call this
        /// </summary>
        protected virtual Type AssociatedDtoMapping { get { return null; } }

        /// <summary>
        /// Override this if your dto relies on multiple other dtos in its mapping
        /// For instance if you are mapping a property that is a type and you want that to map to a dto then call this
        /// </summary>
        protected virtual Type[] AssociatedDtoMappings { get { return null; } }
        
        /// <summary>
        /// This method is called to get the data table. Can be overridden if include statements are needed.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>returns an IQueryable of the table TEntity as Untracked</returns>
        protected virtual IQueryable<TEntity> GetDataUntracked(IGenericServicesDbContext context)
        {
            return context.Set<TEntity>().AsNoTracking();
        }

        /// <summary>
        /// This provides the IQueryable command to get a list of TEntity, but projected to TDto.
        /// Can be overridden if standard AutoMapping isn't good enough, or return null if not supported
        /// </summary>
        /// <returns></returns>
        internal protected virtual IQueryable<TDto> ListQueryUntracked(IGenericServicesDbContext context)
        {
            var query = GetDataUntracked(context).ProjectTo<TDto>(AutoMapperConfigs[CreateDictionaryKey<TEntity, TDto>()]);

            //We check if we need to decompile the LINQ expression so that any computed properties in the class are filled in properly
            return ApplyDecompileIfNeeded(query);
        }

        //----------------------------------------------------------------------
        //non-overridable internal methods

        /// <summary>
        /// This copies back the keys from a newly created entity into the dto as long as there are matching properties in the Dto
        /// </summary>
        /// <param name="context"></param>
        /// <param name="newEntity"></param>
        internal protected void AfterCreateCopyBackKeysToDtoIfPresent(IGenericServicesDbContext context, TEntity newEntity)
        {
            var dtoKeyProperies = typeof (TDto).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var entityKeys in context.GetKeyProperties<TEntity>())
            {
                var dtoMatchingProperty =
                    dtoKeyProperies.SingleOrDefault(
                        x => x.Name == entityKeys.Name && x.PropertyType == entityKeys.PropertyType);
                if (dtoMatchingProperty == null) continue;

                dtoMatchingProperty.SetValue(this, entityKeys.GetValue(newEntity));
            }
        }

        //---------------------------------------------------------------
        //protected methods

        /// <summary>
        /// This gets the key values from this DTO in the correct order. Used in FindItemTrackedForUpdate sync/async
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected object[] GetKeyValues(IGenericServicesDbContext context)
        {
            var efkeyProperties = context.GetKeyProperties<TEntity>().ToArray();
            var dtoProperties = typeof(TDto).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var keysInOrder = efkeyProperties.Select(x => dtoProperties.SingleOrDefault(y => y.Name == x.Name && y.PropertyType == x.PropertyType)).ToArray();

            if (keysInOrder.Any(x => x == null))
                throw new MissingPrimaryKeyException("The dto must contain all the key(s) properties from the data class.");

            return keysInOrder.Select(x => x.GetValue(this)).ToArray();
        }

        /// <summary>
        /// This checks if the DelegateDecompiler is needed. If so it applies it to the query
        /// </summary>
        /// <returns>original query, but with Decompile applied if needed</returns>
        protected IQueryable<TDto> ApplyDecompileIfNeeded(IQueryable<TDto> query)
        {
            return NeedsDecompile ? query.DecompileAsync() : query;
        }

    }
}
