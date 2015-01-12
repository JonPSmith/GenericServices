#region licence
// The MIT License (MIT)
// 
// Filename: CheckDecompile.cs
// Date Created: 2015/1/12
// 
// Copyright (c) 2015 Jon Smith (www.selectiveanalytics.com & www.thereformedprogrammer.net)
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
using System.Linq;
using System.Reflection;
using DelegateDecompiler;

namespace GenericServices.Core.Internal
{
    internal static class CheckComputed
    {

        /// <summary>
        /// This is the cache used to hold info on if the type contains the DelegateDecompiler [Computed] attribute
        /// </summary>
        private static readonly ConcurrentDictionary<Type, bool> EntityHasComputed = new ConcurrentDictionary<Type, bool>();

        private static bool CheckIfClassNeedsDecompile(Type type)
        {
            return (type.GetProperties().Any(x => x.GetCustomAttribute<ComputedAttribute>() != null));
        }

        /// <summary>
        /// This returns true if the config says we are using DelegateDecompiler and the class contains the DelegateDecompiler [Computed] attribute
        /// </summary>
        /// <param name="classToCheck"></param>
        /// <returns></returns>
        public static bool ClassNeedsDecompile(Type classToCheck)
        {
            return GenericServicesConfig.UseDelegateDecompilerWhereNeeded 
                   && EntityHasComputed.GetOrAdd(classToCheck, setup => CheckIfClassNeedsDecompile(classToCheck));
        }
    }
}
