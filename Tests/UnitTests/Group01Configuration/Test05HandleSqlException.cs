#region licence
// The MIT License (MIT)
// 
// Filename: Test02SqlDict.cs
// Date Created: 2014/09/25
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
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using GenericServices;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group01Configuration
{
    class Test05HandleSqlException
    {
            
        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            GenericServicesConfig.HandleSqlExceptionOnSave = null;
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            GenericServicesConfig.HandleSqlExceptionOnSave = null;
        }

        private ValidationResult TestExceptionCatch(SqlException ex, IEnumerable<DbEntityEntry> entitiesNotSaved)
        {         
            var message = string.Format("SQL error {0}. Following class types had errors: {1}.", ex.Number, 
                string.Join(",", entitiesNotSaved.Select(x => x.Entity.GetType().Name)));
            return new ValidationResult(message);
        }

        private ValidationResult TestExceptionNoCatch(SqlException ex, IEnumerable<DbEntityEntry> entitiesNotSaved)
        {
            return null;
        }

        //-------------------------------
        //Tests

        [Test]
        public void Test01ValidateTagError()
        {
            //SETUP
            GenericServicesConfig.HandleSqlExceptionOnSave = TestExceptionCatch;
            using (var db = new SampleWebAppDb())
            {
                var existingTag = db.Tags.First();

                //ATTEMPT
                var dupTag = new Tag { Name = "duplicate slug", Slug = existingTag.Slug };
                db.Tags.Add(dupTag);
                var status = db.SaveChangesWithChecking();

                //VERIFY
                status.IsValid.ShouldEqual(false);
                status.Errors.Count.ShouldEqual(1);
                status.Errors[0].ErrorMessage.ShouldEqual("SQL error 2601. Following class types had errors: Tag.");
            }
        }

        [Test]
        public void Test02ValidateTagErrorWithOtherData()
        {
            //SETUP
            GenericServicesConfig.HandleSqlExceptionOnSave = TestExceptionCatch;
            using (var db = new SampleWebAppDb())
            {
                var existingTag = db.Tags.First();

                //ATTEMPT
                var dupTag = new Tag { Name = "duplicate slug", Slug = existingTag.Slug };
                db.Tags.Add(dupTag);
                db.Blogs.Add(new Blog {Name = Guid.NewGuid().ToString(), EmailAddress = "nospam@nospam.com"});
                var status = db.SaveChangesWithChecking();

                //VERIFY
                status.IsValid.ShouldEqual(false);
                status.Errors.Count.ShouldEqual(1);
                status.Errors[0].ErrorMessage.ShouldEqual("SQL error 2601. Following class types had errors: Tag.");
            }
        }

        [Test]
        public void Test10ValidateTagCaughtBySqlDict()
        {
            //SETUP
            GenericServicesConfig.HandleSqlExceptionOnSave = TestExceptionNoCatch;
            using (var db = new SampleWebAppDb())
            {
                var existingTag = db.Tags.First();

                //ATTEMPT
                var dupTag = new Tag { Name = "duplicate slug", Slug = existingTag.Slug };
                db.Tags.Add(dupTag);
                var status = db.SaveChangesWithChecking();

                //VERIFY
                status.IsValid.ShouldEqual(false);
                status.Errors.Count.ShouldEqual(1);
                status.Errors[0].ErrorMessage.ShouldEqual("One of the properties is marked as Unique index and there is already an entry with that value.");
            }
        }
    }
}
