#region licence
// The MIT License (MIT)
// 
// Filename: EfKeyFinder.cs
// Date Created: 2014/07/16
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
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]

namespace GenericServices.Core.Internal
{

    internal static class EfKeyFinder
    {

        private static readonly ConcurrentDictionary<Type, IReadOnlyCollection<PropertyInfo>> KeyCache = new ConcurrentDictionary<Type, IReadOnlyCollection<PropertyInfo>>();

        /// <summary>
        /// This returns PropertyInfos for all the properties in the class that are found in the entity framework metadata 
        /// </summary>
        /// <typeparam name="TClass">The class must belong to a class that entity framework has in its metadata</typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IReadOnlyCollection<PropertyInfo> GetKeyProperties<TClass>(this IGenericServicesDbContext context) where TClass : class
        {
            return KeyCache.GetOrAdd(typeof(TClass), type => FindKeys(type, context));
        }

        private static List<PropertyInfo> FindKeys(Type type, IGenericServicesDbContext context)
        {
            var metadata = ((IObjectContextAdapter)context).ObjectContext.MetadataWorkspace;

            // Get the part of the model that contains info about the actual CLR types
            var objectItemCollection = ((ObjectItemCollection)metadata.GetItemCollection(DataSpace.OSpace));

            // Get the entity type from the model that maps to the CLR type
            var entityType = metadata
                    .GetItems<EntityType>(DataSpace.OSpace)
                    .SingleOrDefault(e => objectItemCollection.GetClrType(e) == type);

            if (entityType == null)
                throw new InvalidOperationException("This method expects a entity class. Did you provide a DTO by mistake?");

            var keyProperties = entityType.KeyProperties.Select(x => type.GetProperty(x.Name)).ToList();
            if (!keyProperties.Any())
                throw new MissingPrimaryKeyException(string.Format("Failed to find a EF primary key in type {0}", type.Name));
            if (keyProperties.Any( x => x == null))
                throw new NullReferenceException(string.Format("Failed to find key property by name in type {0}", type.Name));

            return keyProperties;
        }
    }
}
