using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericServices;
using GenericServices.Services.Concrete;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.DTOs.Concrete;

namespace ProfileApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new SampleWebAppDb())
            {
                var firstId = db.Posts.First().PostId;
                RunCommand1(db);
                RunCommand2(db);
            }

        }

        private static void RunCommand1(IGenericServicesDbContext db)
        {
            var service = new ListService(db);
            var list = service.GetAll<SimplePostDto>().ToList();
        }

        private static void RunCommand2(IGenericServicesDbContext db)
        {
            var service = new ListService(db);
            var list = service.GetAll<SimplePostDto>().ToList();
        }
    }
}
