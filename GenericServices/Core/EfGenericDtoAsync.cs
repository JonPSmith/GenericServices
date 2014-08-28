using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using GenericServices.Services;

[assembly: InternalsVisibleTo("Tests")]

namespace GenericServices.Core
{

    public abstract class EfGenericDtoAsync<TData, TDto> : EfGenericDtoBase<TData, TDto>
        where TData : class, new()
        where TDto : EfGenericDtoAsync<TData, TDto>, new()
    {

        
        /// <summary>
        /// This function will be called at the end of CreateSetupService and UpdateSetupService to setup any
        /// additional data in the dto used to display dropdownlists etc. 
        /// It is also called at the end of the CreateService or UpdateService if there are errors, so that
        /// the data is available if the form needs to be reshown.
        /// This function should be overridden if the dto needs additional data setup 
        /// </summary>
        /// <returns></returns>
        internal protected virtual async Task SetupSecondaryDataAsync(IDbContextWithValidation db, TDto dto)
        {
            if (!SupportedFunctions.HasFlag(ServiceFunctions.DoesNotNeedSetup))
                throw new InvalidOperationException("SupportedFunctions flags say that setup of secondary data is needed, but did not override the SetupSecondaryData method.");
        }

        /// <summary>
        /// This returns the TData item that fits the key(s) in the DTO.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        internal protected virtual async Task<TData> FindItemTrackedAsync(IDbContextWithValidation context)
        {
            return await context.Set<TData>().FindAsync(GetKeyValues(context));
        }



        /// <summary>
        /// This copies only the properties in TDto that have public setter into the TData
        /// You can override this if you need a more complex copy
        /// </summary>
        /// <param name="context"></param>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        internal protected virtual async Task<ISuccessOrErrors> CopyDtoToDataAsync(IDbContextWithValidation context, TDto source, TData destination)
        {
            CreateDtoToDataMapping();
            Mapper.Map(source, destination);

            var status = SuccessOrErrors.Success("Successful copy of data");
            if (!SupportedFunctions.HasFlag(ServiceFunctions.ValidateonCopyDtoToData)) return status;

            //we need to run a validation on the destination as it might have new or tigher validation rules
            var errors = new List<ValidationResult>();
            var vc = new ValidationContext(destination, null, null);
            var valid = Validator.TryValidateObject(destination, vc, errors, true);
            if (!valid)
                status.SetErrors(errors);

            return status;
        }

        /// <summary>
        /// This copies only the properties in TData that have public setter into the TDto
        /// You can override this if you need a more complex copy
        /// </summary>
        /// <param name="context"></param>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        internal protected virtual async Task<ISuccessOrErrors> CopyDataToDtoAsync(IDbContextWithValidation context, TData source, TDto destination)
        {
            CreateDatatoDtoMapping();
            Mapper.Map(source, destination);
            return SuccessOrErrors.Success("Successful copy of data");
        }

        //---------------------------------------------------------------
        //helper methods

        /// <summary>
        /// This copies an existing TData into a new the dto using a Lambda expression to define the where clause
        /// It copies TData properties into all TDto properties that have accessable setters, i.e. not private
        /// </summary>
        /// <returns>status. If Valid then dto, otherwise null</returns>
        internal protected virtual async Task<ISuccessOrErrors<TDto>> CreateDtoAndCopyDataInAsync(
            IDbContextWithValidation context, Expression<Func<TData, bool>> predicate)
        {
            Mapper.CreateMap<TData, TDto>();
            return
                await
                    GetDataUntracked(context)
                        .Where(predicate)
                        .Project()
                        .To<TDto>()
                        .TrySingleWithPermissionCheckingAsync();
        }
    }
}
