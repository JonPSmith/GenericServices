using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AutoMapper;
using NUnit.Framework;
using Tests.DataClasses.Concrete;
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
