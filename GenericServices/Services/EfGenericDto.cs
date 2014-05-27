using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using AutoMapper;
using AutoMapper.QueryableExtensions;

[assembly: InternalsVisibleTo("Tests")]

namespace GenericServices.Services
{
    [Flags]
    public enum ServiceFunctions
    {
        None = 0,
        List = 1,
        Detail = 2,
        Create = 4,
        Update = 8,
        //note: no delete as delete does not need a dto
        DoAction = 32,
        DoDbAction = 32,
        //DoesNotNeedSetup refers the need to call the SetupSecondaryData method
        //if this flag is NOT set then expects dto to override SetupSecondaryData method
        DoesNotNeedSetup = 128,
        AllCrudButCreate = List | Detail | Update,
        AllCrudButList = Detail | Create | Update,
        AllCrud = List | Detail | Create | Update
    }

    public abstract class EfGenericDto<TData, TDto> where TData : class
        where TDto : EfGenericDto<TData,TDto>
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
        /// This must be overridden to say that the dto supports the create function
        /// </summary>
        internal protected abstract ServiceFunctions SupportedFunctions { get; }

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
        protected virtual IQueryable<TData> GetDataUntracked(IDbContextWithValidation context)
        {
            return context.Set<TData>().AsNoTracking();
        }

        /// <summary>
        /// This function will be called at the end of CreateSetupService and UpdateSetupService to setup any
        /// additional data in the dto used to display dropdownlists etc. 
        /// It is also called at the end of the CreateService or UpdateService if there are errors, so that
        /// the data is available if the form needs to be reshown.
        /// This function should be overridden if the dto needs additional data setup 
        /// </summary>
        /// <returns></returns>
        internal protected virtual void SetupSecondaryData(IDbContextWithValidation db, TDto dto)
        {
            if (!SupportedFunctions.HasFlag(ServiceFunctions.DoesNotNeedSetup))
                throw new InvalidOperationException("SupportedFunctions flags say that setup of secondary data is needed, but did not override the SetupSecondaryData method.");
        }

        /// <summary>
        /// This returns the TData item that fits the key(s) in the DTO.
        /// Override if you want to include other relationships for deep level updates
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        internal protected virtual TData FindItemTracked(IDbContextWithValidation context)
        {
            return context.Set<TData>().Find(GetKeyValues());
        }

        /// <summary>
        /// This provides the IQueryable command to get a list of TData, but projected to TDto.
        /// Can be overridden if standard AutoMapping isn't good enough, or return null if not supported
        /// </summary>
        /// <returns></returns>
        internal protected virtual IQueryable<TDto> BuildListQueryUntracked(IDbContextWithValidation context)
        {
            CreateDatatoDtoMapping();
            return GetDataUntracked(context).Project().To<TDto>();
        }

        /// <summary>
        /// This copies only the properties in TDto that have public setter into the TData
        /// You can override this if you need a more complex copy
        /// </summary>
        /// <param name="context"></param>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        internal protected virtual ISuccessOrErrors CopyDtoToData(IDbContextWithValidation context, TDto source, TData destination)
        {
            CreateDtoToDataMapping();
            Mapper.Map(source, destination);
            return SuccessOrErrors.Success("Successfull copy of data");
        }


        /// <summary>
        /// This copies only the properties in TData that have public setter into the TDto
        /// You can override this if you need a more complex copy
        /// </summary>
        /// <param name="context"></param>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        internal protected virtual ISuccessOrErrors CopyDataToDto(IDbContextWithValidation context, TData source, TDto destination)
        {
            CreateDatatoDtoMapping();
            Mapper.Map(source, destination);
            return SuccessOrErrors.Success("Successfull copy of data");
        }

        //---------------------------------------------------------------
        //helper methods

        /// <summary>
        /// This copies an existing TData into a new the dto using a Lambda expression to define the where clause
        /// It copies TData properties into all TDto properties that have accessable setters, i.e. not private
        /// </summary>
        /// <returns>dto, or null if not found</returns>
        internal protected virtual TDto CreateDtoAndCopyDataIn(IDbContextWithValidation context, Expression<Func<TData, bool>> predicate)
        {
            Mapper.CreateMap<TData, TDto>();
            var dto = GetDataUntracked(context).Where(predicate).Project().To<TDto>().SingleOrDefault();
            if (dto == null)
                throw new ArgumentException("We could not find an entry using the given predicate");

            return dto;
        }

        //---------------------------------------------------------------
        //private methods


        private static void CreateDatatoDtoMapping()
        {
            Mapper.CreateMap<TData, TDto>();
        }

        private static void CreateDtoToDataMapping()
        {
            Mapper.CreateMap<TDto, TData>()
                .ForAllMembers(opt => opt.Condition(CheckIfSourceSetterIsPublic));
        }

        private static bool CheckIfSourceSetterIsPublic(ResolutionContext mapContext)
        {
            return mapContext.PropertyMap.SourceMember != null 
                   && ((PropertyInfo)mapContext.PropertyMap.SourceMember).SetMethod != null
                   && ((PropertyInfo)mapContext.PropertyMap.SourceMember).SetMethod.IsPublic;
        }

        private object[] GetKeyValues()
        {
            var keyProperies = typeof(TDto).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.GetCustomAttribute<KeyAttribute>() != null).ToArray();
            if (!keyProperies.Any())
                throw new MissingPrimaryKeyException("You must mark the primary key(s) in the DTO with the [Key] attribute");

            return keyProperies.Select(x => x.GetValue(this)).ToArray();
        }
    }
}
