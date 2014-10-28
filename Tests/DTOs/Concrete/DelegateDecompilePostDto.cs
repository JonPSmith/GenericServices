#region licence
// The MIT License (MIT)
// 
// Filename: DetailPostDto.cs
// Date Created: 2014/05/19
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

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using GenericServices.Core;
using Tests.DataClasses.Concrete;

[assembly: InternalsVisibleTo("Tests")]

namespace Tests.DTOs.Concrete
{
    public class DelegateDecompilePostDto : InstrumentedEfGenericDto<Post, DelegateDecompilePostDto>
    {

        [UIHint("HiddenInput")]
        [Key]
        public int PostId { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        //public ICollection<Tag> Tags { get; set; }

        //------------------------------------------------------
        //now the computed properties to read

        public string BloggerNameAndEmail { get; set; }

        public IEnumerable<string> TagNames { get; set; }

        protected internal override CrudFunctions SupportedFunctions
        {
            get { return CrudFunctions.AllCrud | CrudFunctions.DoesNotNeedSetup ; }
        }

    }
}
