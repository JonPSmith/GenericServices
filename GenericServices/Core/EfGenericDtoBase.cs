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
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]

namespace GenericServices.Core
{
    [Flags]
    public enum CrudFunctions
    {
        None = 0,
        List = 1,
        Detail = 2,
        Create = 4,
        Update = 8,
        //note: no delete as delete does not need a dto
        
        DoesNotNeedSetup = 256,
        AllCrudButCreate = List | Detail | Update,
        AllCrudButList = Detail | Create | Update,
        AllCrud = List | Detail | Create | Update
    }

    public abstract class EfGenericDtoBase 
    {
        /// <summary>
        /// This class hold any information that can be found at setup
        /// </summary>
        protected class GenericDtoSetupInfo
        {

            /// <summary>
            /// Set to true if UseDelegateDecompilerWhereNeeded is valid and TEntity class contains a [Computed] attribute
            /// </summary>
            public bool NeedsDecompile { get; private set; }

            public GenericDtoSetupInfo(bool needsDecompile)
            {
                NeedsDecompile = needsDecompile;
            }
        }

        /// <summary>
        /// This is the cache used to hold info on setup. If an entry is in the directory then the class has been setup
        /// </summary>
        protected static readonly ConcurrentDictionary<Type, GenericDtoSetupInfo> SetupCache = new ConcurrentDictionary<Type, GenericDtoSetupInfo>();

        /// <summary>
        /// If this flag is set then .Decompile needs to be added to any query
        /// The flag is set on creation based on whether config UseDelegateDecompilerWhereNeeded flas is true
        /// and class's TEntity class, or  any of the associatedDTO TEntity classes ,
        /// has properties with the [Computed] attribute on them.
        /// Can be overriden by the developer.
        /// </summary>
        public bool NeedsDecompile { get; set; }

        /// <summary>
        /// Override and set to true if you wish to force NeedDecompile as always on in this DTO.
        /// Needed if accessing a calculated field in a related class
        /// </summary>
        public virtual bool ForceNeedDecompile { get {  return false;} }

        /// <summary>
        /// This must be overridden to say that the dto supports the create function
        /// </summary>
        internal protected abstract CrudFunctions SupportedFunctions { get; }

    }
}
