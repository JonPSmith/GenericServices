using System;
using System.Linq;
using System.Linq.Expressions;
using GenericServices.Concrete;

namespace GenericServices
{
    [Flags]
    public enum CrudFunctions
    {
        None = 0,
        List = 1,
        Detail = 2,
        Create = 4,
        Update = 8,
        AllButCreate = List | Detail | Update,
        AllButList = Detail | Create | Update,
        All = ~None
    }

    public interface IEfGenericDto {}

}