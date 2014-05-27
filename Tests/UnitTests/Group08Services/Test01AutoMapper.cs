using System.Linq;
using AutoMapper;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests.Group08Services
{
    class Test01AutoMapper
    {
        private class Data
        {
            public int [] Ints { get; set; }

            public int GetSum { get { return Ints.Sum(); } }
        }

        private class Dto
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
            Mapper.CreateMap<Data, Dto>();
            var dto = Mapper.Map<Data, Dto>(data);

            //VERIFY
            dto.IntsCount.ShouldEqual(3);
            dto.Sum.ShouldEqual(6);
        }
    }
}
