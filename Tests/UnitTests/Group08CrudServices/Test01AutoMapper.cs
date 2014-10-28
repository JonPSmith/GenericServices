#region licence
// The MIT License (MIT)
// 
// Filename: Test01AutoMapper.cs
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

using System.Linq;
using AutoMapper;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests.Group08CrudServices
{
    class Test01AutoMapper
    {
        private class Data
        {
            public int [] Ints { get; set; }

            public int GetSum { get { return Ints.Sum(); } }
        }

        private class DtoAggregate
        {
            public int IntsCount { get; set; }
            public int Sum { get; set; }
        }

        [Test]
        public void Check01MappingAggregatesOk()
        {

            //SETUP  
            var data = new Data {Ints = new[] {1, 2, 3}};

            //ATTEMPT
            Mapper.CreateMap<Data, DtoAggregate>();
            var dto = Mapper.Map<Data, DtoAggregate>(data);

            //VERIFY
            dto.IntsCount.ShouldEqual(3);
            dto.Sum.ShouldEqual(6);
        }

        //private class DtoSelectCollection
        //{
        //    public string Title { get; set; }
        //    public ICollection<string> TagsName { get; set; }
        //}

        //[Test]
        //public void Check02MappingSelectFromCollectionOk()
        //{

        //    //SETUP  
        //    var data = new Post { Title = "Hello", Tags = new Collection<Tag>{ new Tag{ Name = "Tag1"}, new Tag{ Name = "Tag2"}}  };

        //    //ATTEMPT
        //    Mapper.CreateMap<Post, DtoSelectCollection>();
        //    var dto = Mapper.Map<Post, DtoSelectCollection>(data);

        //    //VERIFY
        //    dto.Title.ShouldEqual("Hello");
        //    CollectionAssert.AreEqual(new string[] { "Tag1", "Tag2" }, dto.TagsName);     //THIS FAILS

        //}
    }
}
