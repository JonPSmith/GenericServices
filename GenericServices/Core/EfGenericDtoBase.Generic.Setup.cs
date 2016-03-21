#region licence
// The MIT License (MIT)
// 
// Filename: EfGenericDtoBase.Generic.Setupcs
// Date Created: 2014/11/11
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
using System.Reflection;
using System.Runtime.CompilerServices;
using AutoMapper;
using GenericServices.Core.Internal;

[assembly: InternalsVisibleTo("Tests")]

namespace GenericServices.Core
{
    /// <summary>
    /// This should not be used. It is used as the base for EfGenericDto and EfGenericDtoAsync
    /// This partial class contains all the code to setup the DTO.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TDto"></typeparam>
    public abstract partial class EfGenericDtoBase<TEntity, TDto> : EfGenericDtoBase
        where TEntity : class
        where TDto : EfGenericDtoBase<TEntity, TDto>
    {


        /// <summary>
        /// Constructor. This ensures that the mappings are set up on creation of the class
        /// and sets the NeedsDecompile property based on checking 
        /// </summary>
        protected EfGenericDtoBase()
        {
            MapperSetup();
        }


        //---------------------------------------------------------------------
        //private methods

        /// <summary>
        /// This is used to set up the mapping of any associated EfGenericDto 
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="readFromDatabase"></param>
        /// <returns></returns>
        protected void AssociatedMapperSetup(IMapperConfiguration cfg, bool readFromDatabase)
        {
            var needsDecompile = false;
            if (readFromDatabase)
                CreateReadFromDatabaseMapping(cfg, ref needsDecompile);
            else
                CreateWriteToDatabaseMapping(cfg);
        }

        /// <summary>
        /// This sets all the AutoMapper mapping that this dto needs. It is called from the base constructor
        /// It also makes sure that any associated dto mappings are set up as the order of creation is not fixed
        /// </summary>
        private void MapperSetup()
        {
            var needsDecompile = false;
            GenericServicesConfig.AutoMapperConfigs.GetOrAdd(CreateDictionaryKey<TEntity, TDto>(),
                config => new MapperConfiguration(cfg => CreateReadFromDatabaseMapping(cfg, ref needsDecompile)));

            //now set up NeedsDecompile any associated mappings. See comments on AssociatedDtoMapping for why these are needed
            NeedsDecompile = ForceNeedDecompile || CheckComputed.ClassNeedsDecompile(typeof(TEntity));
            NeedsDecompile |= needsDecompile;

            if (SupportedFunctions.HasFlag(CrudFunctions.Update) | SupportedFunctions.HasFlag(CrudFunctions.Create))
                //Only setup TDto->TEntity mapping if needed
                GenericServicesConfig.AutoMapperConfigs.GetOrAdd(CreateDictionaryKey<TDto, TEntity>(), 
                    config => new MapperConfiguration(CreateWriteToDatabaseMapping));
        }

        /// <summary>
        /// This sets up the AutoMapper mapping for a copy from the TEntity to the TDto.
        /// It applies any extra mapping provided by AddedDatabaseToDtoMapping if not null
        /// </summary>
        private void CreateReadFromDatabaseMapping(IMapperConfiguration cfg, ref bool needsDecompile)
        {
            if (AddedDatabaseToDtoMapping == null)
                cfg.CreateMap<TEntity, TDto>();
            else
                AddedDatabaseToDtoMapping(cfg.CreateMap<TEntity, TDto>());

            needsDecompile = needsDecompile | SetupAllAssociatedMappings(cfg, true);
        }

        /// <summary>
        /// This sets up the AutoMapper mapping for a copy from the TDto to the TEntity.
        /// Note that properties which have the [DoNotCopyBackToDatabase] attribute will not be copied
        /// </summary>
        private void CreateWriteToDatabaseMapping(IMapperConfiguration cfg)
        {
            cfg.CreateMap<TDto, TEntity>().IgnoreMarkedProperties();
            SetupAllAssociatedMappings(cfg, false);
        }

        protected static string CreateDictionaryKey<TFrom, TTo>()
        {
            return typeof (TFrom).FullName + "=" + typeof (TTo).FullName;
        }

        /// <summary>
        /// Set up any requested assocaiated mappings
        /// </summary>
        private bool SetupAllAssociatedMappings(IMapperConfiguration cfg, bool readFromDatabase)
        {
            var shouldDecompile = false;
            if (AssociatedDtoMapping != null)
                shouldDecompile |= CheckAndSetupAssociatedMapping(AssociatedDtoMapping, cfg, readFromDatabase);

            if (AssociatedDtoMappings == null) return shouldDecompile;

            foreach (var associatedDtoMapping in AssociatedDtoMappings)
                shouldDecompile |= CheckAndSetupAssociatedMapping(associatedDtoMapping, cfg, readFromDatabase);

            return shouldDecompile;
        }

        private static bool CheckAndSetupAssociatedMapping(Type associatedDtoMapping, IMapperConfiguration cfg, bool readFromDatabase)
        {
            if (!associatedDtoMapping.IsSubclassOf(typeof(EfGenericDtoBase)))
                throw new InvalidOperationException("You have not supplied a class based on EfGenericDto to set up the mapping.");

            //create the acssociated dto to get the AssociatedMapperSetup method
            var associatedDto = Activator.CreateInstance(associatedDtoMapping, new object[] { });
            var method = associatedDtoMapping.GetMethod(nameof(AssociatedMapperSetup),
                BindingFlags.NonPublic | BindingFlags.Instance);

            method.Invoke(associatedDto, new object[] { cfg, readFromDatabase});
            return ((EfGenericDtoBase)associatedDto).NeedsDecompile;
        }


    }
}
