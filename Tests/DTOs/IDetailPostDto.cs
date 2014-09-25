#region licence
// The MIT License (MIT)
// 
// Filename: IDetailPostDto.cs
// Date Created: 2014/05/20
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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Tests.DataClasses.Concrete;
using Tests.UiHelpers;

namespace Tests.DTOs
{
    public interface IDetailPostDto
    {
        [UIHint("HiddenInput")]
        [Key]
        int PostId { get; set; }

        [MinLength(2), MaxLength(128)]
        string Title { get; set; }

        [DataType(DataType.MultilineText)]
        string Content { get; set; }

        DateTime LastUpdated { get; }

        [UIHint("HiddenInput")]
        int BlogId { get; set; }

        ICollection<Tag> Tags { get; set; }
        string BloggerName { get; }

        /// <summary>
        /// This allows a single blogger to be chosen from the list
        /// </summary>
        DropDownListType Bloggers { get; set; }

        MultiSelectListType UserChosenTags { get; set; }

        /// <summary>
        /// When it was last updated in DateTime format
        /// </summary>
        DateTime LastUpdatedUtc { get; }

        string TagNames { get; }

        //-----------------------------------------------------------------------------
        //public methods

        /// <summary>
        /// Optional method that will setup any mapping etc. that are cached. This will will improve speed later.
        /// The GenericDto will still work without this method being called, but the first use that needs the map will be slower. 
        /// </summary>
        void CacheSetup();
    }
}