#region licence
// The MIT License (MIT)
// 
// Filename: EfGenericDto.cs
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
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using GenericLibsBase;
using GenericLibsBase.Core;

[assembly: InternalsVisibleTo("Tests")]

namespace GenericServices.Core
{
    public abstract class EfGenericDto<TData, TDto> : EfGenericDtoBase<TData, TDto>
        where TData : class, new()
        where TDto : EfGenericDto<TData,TDto>, new()
    {

        /// <summary>
        /// This function will be called at the end of CreateSetupService and UpdateSetupService to setup any
        /// additional data in the dto used to display dropdownlists etc. 
        /// It is also called at the end of the CreateService or UpdateService if there are errors, so that
        /// the data is available if the form needs to be reshown.
        /// This function should be overridden if the dto needs additional data setup 
        /// </summary>
        /// <returns></returns>
        internal protected virtual void SetupSecondaryData(IGenericServicesDbContext db, TDto dto)
        {
            if (!SupportedFunctions.HasFlag(ServiceFunctions.DoesNotNeedSetup))
                throw new InvalidOperationException("SupportedFunctions flags say that setup of secondary data is needed, but did not override the SetupSecondaryData method.");
        }

        /// <summary>
        /// Used only by Update. This returns the TData item that fits the key(s) in the DTO.
        /// Override this if you need to include any related entries when doing a complex update.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        internal protected virtual TData FindItemTrackedForUpdate(IGenericServicesDbContext context)
        {
            return context.Set<TData>().Find(GetKeyValues(context));
        }

        /// <summary>
        /// This is used in a create. It copies only the properties in TDto that have public setter into the TData.
        /// You can override this if you need a more complex copy
        /// Note: If SupportedFunctions has the flag ValidateonCopyDtoToData then it validates the data (used by Action methods)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="source"></param>
        /// <returns>status which, if Valid, has new TData with data from DTO copied in</returns>
        internal protected virtual ISuccessOrErrors<TData> CreateDataFromDto(IGenericServicesDbContext context, TDto source)
        {
            var result = new TData();
            var status = CreateUpdateDataFromDto(context, source, result);

            return status.IsValid
                ? new SuccessOrErrors<TData>(result, status.SuccessMessage)
                : SuccessOrErrors<TData>.ConvertNonResultStatus(status);
        }

        /// <summary>
        /// This is used in an update. It copies only the properties in TDto that have public setter into the TData.
        /// You can override this if you need a more complex copy
        /// </summary>
        /// <param name="context"></param>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <return>status. destination is only valid if status.IsValid</return>
        internal protected virtual ISuccessOrErrors UpdateDataFromDto(IGenericServicesDbContext context, TDto source, TData destination)
        {
            CreateDtoToDataMapping();
            Mapper.Map(source, destination);
            return SuccessOrErrors.Success("Successful copy of data");
        }

        /// <summary>
        /// This copies an existing TData into a new the dto using a Lambda expression to define the where clause
        /// It copies TData properties into all TDto properties that have accessable setters, i.e. not private
        /// </summary>
        /// <returns>status. If valid result is dto. Otherwise null</returns>
        internal protected virtual ISuccessOrErrors<TDto> DetailDtoFromDataIn(IGenericServicesDbContext context, 
            Expression<Func<TData, bool>> predicate)
        {
            CreateDatatoDtoMapping();
            return GetDataUntracked(context).Where(predicate).Project().To<TDto>().RealiseSingleWithErrorChecking();
        }

    }
}
