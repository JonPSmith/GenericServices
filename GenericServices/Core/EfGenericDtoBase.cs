using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]

namespace GenericServices.Core
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
        
        //Now Action parts
        DoActionWithoutValidate = 32,
        //This is the default. It validates the destination before calling the action
        DoAction = DoActionWithoutValidate | ValidateonCopyDtoToData,
        //This causes the destination data is validated after a CopyDtoToData. 
        //(Not really necessary when doing a DB action as SaveChanges does a validation)
        ValidateonCopyDtoToData = 64,
        //DoesNotNeedSetup refers the need to call the SetupSecondaryData method
        //if this flag is NOT set then expects dto to override SetupSecondaryData method
        DoesNotNeedSetup = 256,
        AllCrudButCreate = List | Detail | Update,
        AllCrudButList = Detail | Create | Update,
        AllCrud = List | Detail | Create | Update
    }

    public abstract class EfGenericDtoBase 
    {

        /// <summary>
        /// This must be overridden to say that the dto supports the create function
        /// </summary>
        internal protected abstract ServiceFunctions SupportedFunctions { get; }     

    }
}
