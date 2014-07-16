using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
        public static IReadOnlyCollection<PropertyInfo> GetKeyProperties<TClass>(this IDbContextWithValidation context) where TClass : class
        {
            return KeyCache.GetOrAdd(typeof(TClass), type => FindKeys(type, context));
        }

        private static List<PropertyInfo> FindKeys(Type type, IDbContextWithValidation context)
        {
            var metadata = ((IObjectContextAdapter)context).ObjectContext.MetadataWorkspace;

            // Get the part of the model that contains info about the actual CLR types
            var objectItemCollection = ((ObjectItemCollection)metadata.GetItemCollection(DataSpace.OSpace));

            // Get the entity type from the model that maps to the CLR type
            var entityType = metadata
                    .GetItems<EntityType>(DataSpace.OSpace)
                    .Single(e => objectItemCollection.GetClrType(e) == type);

            var keyProperties = entityType.KeyProperties.Select(x => type.GetProperty(x.Name)).ToList();
            if (!keyProperties.Any())
                throw new MissingPrimaryKeyException(string.Format("Failed to find a EF primary key in type {0}", type.Name));
            if (keyProperties.Any( x => x == null))
                throw new NullReferenceException(string.Format("Failed to find key property by name in type {0}", type.Name));

            return keyProperties;
        }
    }
}
