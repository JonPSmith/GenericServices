using System;
using System.Dynamic;
using System.Runtime.CompilerServices;
using GenericServices.Services;
using GenericServices.ServicesAsync;

[assembly: InternalsVisibleTo("Tests")]

namespace GenericServices.Core.Internal
{
    [Flags]
    internal enum WhatItShouldBe
    {
        DataClass = 1,
        AnyDto = 2,
        SpecificDto = 4,
        IsSync = 8,
        IsAsync = 16,
        //sync versions
        SyncAnything = DataClass | AnyDto | IsSync,
        SyncAnyDto = AnyDto | IsSync,
        SyncClassOrSpecificDto = DataClass | SpecificDto | IsSync,
        SyncSpecificDto = IsSync | SpecificDto,
        //Async versions
        AsyncAnything = DataClass | AnyDto | IsAsync,
        AsyncAnyDto = AnyDto | IsAsync,
        AsyncClassOrSpecificDto = DataClass | SpecificDto | IsAsync,
        AsyncSpecificDto = IsAsync | SpecificDto
    }

    internal static class DecodeToService<TEfService>
    {
        public static dynamic CreateCorrectService<TD>(WhatItShouldBe whatItShouldBe, params object[] ctorParams) where TD : class
        {
            return CreateService<TD>(whatItShouldBe, ctorParams);
        }

        private static dynamic CreateService<TD>(WhatItShouldBe whatItShouldBe, params object[] ctorParams) where TD : class
        {
            var syncAsync = new SyncAsyncDefiner(whatItShouldBe);

            var classType = typeof (TD);
            Type[] dataTypes = null;
            if (classType.IsSubclassOf(typeof (EfGenericDtoBase)))
            {
                dataTypes = GetGenericTypesIfCorrectGeneric(classType, syncAsync.BaseGenericDtoType);
                if (dataTypes == null)
                    throw new InvalidOperationException(string.Format("This service needs a class which inherited from {0}.", syncAsync.BaseGenericDtoType.Name));
            } 
            else if (!whatItShouldBe.HasFlag(WhatItShouldBe.DataClass))
                throw new InvalidOperationException("This type of service only works with some form of EfGenericDto.");
            else
            {
                //Its a data class
                dataTypes = new[] {typeof (TD)};
            }

            var genericServiceString = syncAsync.BuildTypeString(dataTypes.Length);
            var serviceGenericType = Type.GetType(genericServiceString);
            if (serviceGenericType == null)
                throw new InvalidOperationException("Failed to create the type. Is the DTO of the correct type?");
            var serviceType = serviceGenericType.MakeGenericType(dataTypes);
            return Activator.CreateInstance(serviceType, ctorParams);
        }


        /// <summary>
        /// This checks whether the class is inherited from the correct version of EfGenericDto
        /// </summary>
        /// <param name="classType">class type to check</param>
        /// <param name="genericDtoClass">the type of the particular dto we are looking for, or EfGenericBase if either will do</param>
        /// <returns>null class is not inherited from genericDto. True if inherited for the dto we expected, else false </returns>
        private static Type[] GetGenericTypesIfCorrectGeneric(Type classType, Type genericDtoClass)
        {
            if (!classType.IsSubclassOf(typeof(EfGenericDtoBase)))
                return null;

            while (classType.Name != genericDtoClass.Name && classType.BaseType != null)
                classType = classType.BaseType;

            return classType.Name != genericDtoClass.Name ? null : classType.GetGenericArguments();
        }

            
        class SyncAsyncDefiner
        {
            private const string UpdateServiceReplaceString = "UpdateService`1";
            private const string UpdateServiceAsyncReplaceString = "UpdateServiceAsync`1";

            private static readonly string UpdateServiceAsyncAssemblyQualifiedName =
                typeof(UpdateServiceAsync<>).AssemblyQualifiedName;

            private static readonly string UpdateServiceAssemblyQualifiedName =
                typeof(UpdateService<>).AssemblyQualifiedName;

            private readonly string _serviceReplaceString;
            private readonly string _serviceAssemblyQualifiedName;

            public Type BaseGenericDtoType { get; private set; }

            public SyncAsyncDefiner(WhatItShouldBe whatItShouldBe)
            {
                if (whatItShouldBe.HasFlag(WhatItShouldBe.IsSync))
                {
                    BaseGenericDtoType = typeof(EfGenericDto<,>);
                    _serviceReplaceString = UpdateServiceReplaceString;
                    _serviceAssemblyQualifiedName = UpdateServiceAssemblyQualifiedName;
                }
                else if (whatItShouldBe.HasFlag(WhatItShouldBe.IsAsync))
                {
                    BaseGenericDtoType = typeof(EfGenericDtoAsync<,>);
                    _serviceReplaceString = UpdateServiceAsyncReplaceString;
                    _serviceAssemblyQualifiedName = UpdateServiceAsyncAssemblyQualifiedName;
                }
                else
                {
                    throw new InvalidOperationException("Neither the IsSync or the IsAsync flags were set.");
                }

                //If any type allowed we test against base
                if (!whatItShouldBe.HasFlag(WhatItShouldBe.SpecificDto))
                    BaseGenericDtoType = typeof(EfGenericDtoBase<,>);
            }

            public string BuildTypeString(int numDataTypes)
            {
                return _serviceAssemblyQualifiedName.Replace(_serviceReplaceString,
                    string.Format("{0}`{1}", typeof(TEfService).Name, numDataTypes));
            }
        }
    }

    
}
