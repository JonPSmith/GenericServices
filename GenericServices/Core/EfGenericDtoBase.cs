#region licence
// The MIT License (MIT)
// 
// Filename: EfGenericDtoBase.cs
// Date Created: 2014/06/24
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
