using System;
using GenericServices.Concrete;

namespace GenericServices
{
    [Flags]
    public enum ServiceFunctions
    {
        None = 0,
        List = 1,
        Detail = 2,
        Create = 4,
        Update = 8,
        //DoesNotNeedSetup refers the need to call the SetupSecondaryData method
        //if this flag is NOT set then expects dto to override SetupForeignKeys method
        DoesNotNeedSetup = 128,
        AllButCreate = List | Detail | Update,
        AllButList = Detail | Create | Update,
        All = List | Detail | Create | Update
    }

    //public interface IEfGenericDto {}

}