using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericServices.Core.Internal;
using GenericServices.Services.Concrete;
using NUnit.Framework;
using Tests.DataClasses.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group15CrudServiceFinder
{
    class Test01DecodeToTypes
    {

        [Test]
        public void Test01DecodeListOk()
        {

            //SETUP
            var timer = new Stopwatch();

            //ATTEMPT
            timer.Start();
            var service = DecodeToService<ListService>.CreateCorrectService<Post>(WhatItShouldBe.SyncAnything, new object[] { null });
            timer.Stop();

            //VERIFY
            ExtendAsserts.ShouldNotEqualNull(service);
            ExtendAsserts.IsA<ListService<Post>>(service);
            Console.WriteLine("took {0:f3} ms", 1000.0 * timer.ElapsedTicks / Stopwatch.Frequency);
        }

    }
}
