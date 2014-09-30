#region licence
// The MIT License (MIT)
// 
// Filename: DecodeToService.cs
// Date Created: 2014/07/19
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
using GenericServices.Services.Concrete;
using GenericServices.ServicesAsync.Concrete;

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

        //public static Type[] DecodeTypes<TD>(WhatItShouldBe whatItShouldBe) where TD : class
        //{
        //    return GetTypesFromInitialType<TD>(whatItShouldBe, new SyncAsyncDefiner(whatItShouldBe));
        //}

        private static dynamic CreateService<TD>(WhatItShouldBe whatItShouldBe, params object[] ctorParams) where TD : class
        {
            var syncAsync = new SyncAsyncDefiner(whatItShouldBe);

            var dataTypes = GetTypesFromInitialType<TD>(whatItShouldBe, syncAsync);

            var genericServiceString = syncAsync.BuildTypeString(dataTypes.Length);
            var serviceGenericType = Type.GetType(genericServiceString);
            if (serviceGenericType == null)
                throw new InvalidOperationException("Failed to create the type. Is the DTO of the correct type?");
            var serviceType = serviceGenericType.MakeGenericType(dataTypes);
            return Activator.CreateInstance(serviceType, ctorParams);
        }

        /// <summary>
        /// This decodes the type and returns an array of types. If the type is based on a GenericDto 
        /// then it returns the Data type and the 
        /// </summary>
        /// <typeparam name="TD"></typeparam>
        /// <param name="whatItShouldBe"></param>
        /// <param name="syncAsync"></param>
        /// <returns></returns>
        private static Type[] GetTypesFromInitialType<TD>(WhatItShouldBe whatItShouldBe, SyncAsyncDefiner syncAsync)
            where TD : class
        {
            var classType = typeof (TD);
            Type[] dataTypes;
            if (classType.IsSubclassOf(typeof (EfGenericDtoBase)))
            {
                dataTypes = GetGenericTypesIfCorrectGeneric(classType, syncAsync.BaseGenericDtoType);
                if (dataTypes == null)
                    throw new InvalidOperationException(string.Format("This service needs a class which inherited from {0}.",
                        syncAsync.BaseGenericDtoType.Name));
            }
            else if (!whatItShouldBe.HasFlag(WhatItShouldBe.DataClass))
                throw new InvalidOperationException("This type of service only works with some form of EfGenericDto.");
            else
            {
                //Its a data class
                dataTypes = new[] {typeof (TD)};
            }
            return dataTypes;
        }


        /// <summary>
        /// This returns the two classes of used to form the EfGenericDto. Null if it doesn't match the expected type
        /// </summary>
        /// <param name="classType">class type to check</param>
        /// <param name="genericDtoClass">the type of the particular dto we are looking for, or EfGenericBase if either will do</param>
        /// <returns>array of two classes if ok. Null array if not inherited from the right sync/async GenericDtoType</returns>
        private static Type[] GetGenericTypesIfCorrectGeneric(Type classType, Type genericDtoClass)
        {
            while (classType.Name != genericDtoClass.Name && classType.BaseType != null)
                classType = classType.BaseType;

            return classType.Name != genericDtoClass.Name ? null : classType.GetGenericArguments();
        }

            
        private class SyncAsyncDefiner
        {
            private static readonly string UpdateServiceReplaceString = typeof(UpdateService<>).Name;
            private static readonly string UpdateServiceAsyncReplaceString = typeof(UpdateServiceAsync<>).Name;

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
