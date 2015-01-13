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

        /// <summary>
        /// This sets all the AutoMapper mapping that this dto needs. It is called from the base constructor
        /// It also makes sure that any associated dto mappings are set up as the order of creation is not fixed
        /// </summary>
        private void MapperSetup()
        {
            CreateReadFromDatabaseMapping();
            CreatWriteToDatabaseMapping();

            //now set up NeedsDecompile any associated mappings. See comments on AssociatedDtoMapping for why these are needed
            NeedsDecompile = ForceNeedDecompile || CheckComputed.ClassNeedsDecompile(typeof(TEntity));
            NeedsDecompile |= SetupAllAssociatedMappings();
        }

        //---------------------------------------------------------------------
        //private methods

        /// <summary>
        /// This sets up the AutoMapper mapping for a copy from the TEntity to the TDto.
        /// It applies any extra mapping provided by AddedDatabaseToDtoMapping if not null
        /// </summary>
        private void CreateReadFromDatabaseMapping()
        {
            if (DoesAutoMapperMapAlreadyExist<TEntity, TDto>()) return;

            var map = Mapper.CreateMap<TEntity, TDto>();
            if (AddedDatabaseToDtoMapping != null)
                AddedDatabaseToDtoMapping(map);
        }

        /// <summary>
        /// This sets up the AutoMapper mapping for a copy from the TDto to the TEntity.
        /// Note that properties which have the [DoNotCopyBackToDatabase] attribute will not be copied
        /// </summary>
        private static void CreatWriteToDatabaseMapping()
        {
            if (DoesAutoMapperMapAlreadyExist<TDto, TEntity>()) return;

            Mapper.CreateMap<TDto, TEntity>()
                .IgnoreMarkedProperties();
        }

        /// <summary>
        /// This stops us setting up the mapping multiple times. Don't think it is problem, but mainly a performance issue.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <returns></returns>
        private static bool DoesAutoMapperMapAlreadyExist<TSource, TDestination>()
        {
            return (Mapper.FindTypeMapFor<TSource, TDestination>() != null);
        }

        /// <summary>
        /// Set up any requested assocaiated mappings
        /// </summary>
        private bool SetupAllAssociatedMappings()
        {

            var shouldDecompile = false;
            if (AssociatedDtoMapping != null)
                shouldDecompile |= CheckAndSetupAssociatedMapping(AssociatedDtoMapping);

            if (AssociatedDtoMappings == null) return shouldDecompile;

            foreach (var associatedDtoMapping in AssociatedDtoMappings)
                shouldDecompile |= CheckAndSetupAssociatedMapping(associatedDtoMapping);

            return shouldDecompile;
        }

        private static bool CheckAndSetupAssociatedMapping(Type associatedDtoMapping)
        {
            if (!associatedDtoMapping.IsSubclassOf(typeof(EfGenericDtoBase)))
                throw new InvalidOperationException("You have not supplied a class based on EfGenericDto to set up the mapping.");

            //create the acssociated dto to get the NeedsDecompile flag. Also makes sure the mapping is set
            var associatedDto = Activator.CreateInstance(associatedDtoMapping, new object[] { });       
            return ((EfGenericDtoBase)associatedDto).NeedsDecompile;
        }


    }
}
