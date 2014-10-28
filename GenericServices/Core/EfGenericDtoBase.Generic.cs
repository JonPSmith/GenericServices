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

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using GenericLibsBase;
using GenericLibsBase.Core;
using GenericServices.Core.Internal;

[assembly: InternalsVisibleTo("Tests")]

namespace GenericServices.Core
{
    
    public abstract class EfGenericDtoBase<TData, TDto> :EfGenericDtoBase
        where TData : class
        where TDto : EfGenericDtoBase<TData, TDto>
    {
        /// <summary>
        /// Optional method that will setup any mapping etc. that are cached. This will will improve speed later.
        /// The GenericDto will still work without this method being called, but the first use that needs the map will be slower. 
        /// </summary>
        public void CacheSetup()
        {
            CreateDatatoDtoMapping();
            CreateDtoToDataMapping();
        }

        /// <summary>
        /// This provides the name of the name of the data item to display in success or error messages.
        /// Override if you want a more user friendly name
        /// </summary>
        internal protected virtual string DataItemName { get { return typeof (TData).Name; }}
        
        /// <summary>
        /// This method is called to get the data table. Can be overridden if include statements are needed.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>returns an IQueryable of the table TData as Untracked</returns>
        protected virtual IQueryable<TData> GetDataUntracked(IGenericServicesDbContext context)
        {
            return context.Set<TData>().AsNoTracking();
        }

        /// <summary>
        /// This provides the IQueryable command to get a list of TData, but projected to TDto.
        /// Can be overridden if standard AutoMapping isn't good enough, or return null if not supported
        /// </summary>
        /// <returns></returns>
        internal protected virtual IQueryable<TDto> ListQueryUntracked(IGenericServicesDbContext context)
        {
            CreateDatatoDtoMapping();
            return GetDataUntracked(context).Project().To<TDto>();
        }

        //---------------------------------------------------------------
        //protected methods

        protected object[] GetKeyValues(IGenericServicesDbContext context)
        {
            var efkeyPropertyNames = context.GetKeyProperties<TData>().Select(x => x.Name).ToArray();

            var dtoKeyProperies = typeof(TDto).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => efkeyPropertyNames.Any( y => y == x.Name)).ToArray();

            if (efkeyPropertyNames.Length != dtoKeyProperies.Length)
                throw new MissingPrimaryKeyException("The dto must contain the key(s) properties from the data class.");

            return dtoKeyProperies.Select(x => x.GetValue(this)).ToArray();
        }

        protected static void CreateDatatoDtoMapping()
        {
            Mapper.CreateMap<TData, TDto>();
        }

        protected static void CreateDtoToDataMapping()
        {
            Mapper.CreateMap<TDto, TData>()
                .ForAllMembers(opt => opt.Condition(CheckIfSourceSetterIsPublic));
        }

        //----------------------------------------------------------------
        //protected/private methods

        /// <summary>
        /// This copies only the properties in TDto that have public setter into the TData.
        /// You can override this if you need a more complex copy
        /// </summary>
        /// <param name="context"></param>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        protected ISuccessOrErrors CreateUpdateDataFromDto(IGenericServicesDbContext context, TDto source, TData destination)
        {
            CreateDtoToDataMapping();
            Mapper.Map(source, destination);
            return SuccessOrErrors.Success("Successful copy of data"); ;
        }

        private static bool CheckIfSourceSetterIsPublic(ResolutionContext mapContext)
        {
            return mapContext.PropertyMap.SourceMember != null 
                   && ((PropertyInfo)mapContext.PropertyMap.SourceMember).SetMethod != null
                   && ((PropertyInfo)mapContext.PropertyMap.SourceMember).SetMethod.IsPublic;
        }

    }
}
