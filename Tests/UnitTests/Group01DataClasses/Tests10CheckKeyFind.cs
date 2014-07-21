using System;
using System.Diagnostics;
using System.Linq;
using GenericServices.Core.Internal;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group01DataClasses
{
    class Tests10CheckKeyFind
    {

        [Test]
        public void Check01FindPostKeyOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var timer = new Stopwatch();
                DataLayerInitialise.InitialiseThis();
                var x = db.Posts.Count();

                //ATTEMPT
                timer.Start();
                var keys = db.GetKeyProperties<Post>();
                timer.Stop();

                Console.WriteLine("took {0:f3} ms", 1000.0 * timer.ElapsedTicks / Stopwatch.Frequency);
                //VERIFY
                keys.Count.ShouldEqual(1);
                keys.First().Name.ShouldEqual("PostId");
            }
        }

        [Test]
        public void Check02FindPostKeyAgainOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var timer = new Stopwatch();
                DataLayerInitialise.InitialiseThis();
                var x = db.Posts.Count();

                //ATTEMPT
                timer.Start();
                var keys = db.GetKeyProperties<Post>();
                timer.Stop();

                Console.WriteLine("took {0:f3} ms", 1000.0 * timer.ElapsedTicks / Stopwatch.Frequency);
                //VERIFY
                keys.Count.ShouldEqual(1);
                keys.First().Name.ShouldEqual("PostId");
            }
        }

        [Test]
        public void Check05FindPostTagGradeKeyOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                DataLayerInitialise.InitialiseThis();

                //ATTEMPT
                var keys = db.GetKeyProperties<PostTagGrade>();

                //VERIFY
                keys.Count.ShouldEqual(2);
                keys.First().Name.ShouldEqual("PostId");
                keys.Last().Name.ShouldEqual("TagId");
            }
        }


    }
}
