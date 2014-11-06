using System.Reflection;
using AutoMapper;

namespace GenericServices.Core.Internal
{
    internal static class MappingHelper
    {
        /// <summary>
        /// This is used to filter out all properties that have a [DoNotCopyBackToDatabase] attribute.
        /// </summary>
        /// <param name="mappingExpression"></param>
        /// <returns></returns>
        public static IMappingExpression<TDto, TEntity> IgnoreMarkedProperties<TDto, TEntity>
            (this IMappingExpression<TDto, TEntity> mappingExpression)
        {
            mappingExpression.ForAllMembers(x => x.Condition(mapContext => mapContext.PropertyMap.SourceMember != null &&
                   mapContext.PropertyMap.SourceMember.GetCustomAttribute<DoNotCopyBackToDatabaseAttribute>() == null));
            return mappingExpression;
        }

    }
}
