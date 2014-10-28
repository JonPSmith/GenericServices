#region licence
// The MIT License (MIT)
// 
// Filename: ISimplePostTagGradeDto.cs
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

using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using GenericServices.Core;
using Tests.DataClasses.Concrete;

namespace Tests.DTOs
{
    internal interface ISimplePostTagGradeDto
    {
        [Key]
        [Column(Order = 1)]
        int PostId { get; }

        [ForeignKey("PostId")]
        Post PostPart { get; }

        [Key]
        [Column(Order = 2)]
        int TagId { get; }

        [ForeignKey("TagId")]
        Tag TagPart { get; }

        int Grade { get; set; }
        string TagPartName { get; }
        string PostPartTitle { get; }
        string FunctionsCalledCommaDelimited { get; }
        ReadOnlyCollection<InstrumentedLog> LogOfCalls { get; }
        bool WriteEvenIfWarning { get; }
        void LogSpecificName(string callPoint, CallTypes callType = CallTypes.Point);
        void LogCaller( CallTypes callType = CallTypes.Point, [CallerMemberName] string callerName = "");

        /// <summary>
        /// Optional method that will setup any mapping etc. that are cached. This will will improve speed later.
        /// The GenericDto will still work without this method being called, but the first use that needs the map will be slower. 
        /// </summary>
        void CacheSetup();
    }
}