#region licence
// The MIT License (MIT)
// 
// Filename: SimplePostTagGradeDto.cs
// Date Created: 2014/05/27
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

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GenericServices;
using GenericServices.Core;
using Tests.DataClasses.Concrete;

namespace Tests.DTOs.Concrete
{
    class SimpleTagPostGradeDto : InstrumentedEfGenericDto<PostTagGrade, SimpleTagPostGradeDto>
    {
        [DoNotCopyBackToDatabase]
        public int TagId { get; set; }
        [ForeignKey("TagId")]
        [DoNotCopyBackToDatabase]
        public Tag TagPart { get; set; }

        [DoNotCopyBackToDatabase]
        public int PostId { get; set; }
        [DoNotCopyBackToDatabase]
        [ForeignKey("PostId")]
        public Post PostPart { get; set; }

        public int Grade { get; set; }

        //--------------------------------
        //now the extra bits

        public string TagPartName { get; set; }

        public string PostPartTitle { get; set; }

        protected internal override CrudFunctions SupportedFunctions
        {
            get { return CrudFunctions.AllCrudButCreate | CrudFunctions.DoesNotNeedSetup; }
        }
    }
}
