using System;
using System.Linq;
using DelegateDecompiler;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.Helpers;
using System.Data.Entity;

namespace Tests.UnitTests.Group02DataClasses
{
    
    //Code removed from Post class to ensure unit tests pass
        ///// <summary>
        ///// This is a property to be filled in by the DelegateDecompilier
        ///// </summary>
        //[Computed]
        //public string BloggerNameAndEmail { get { return Blogger.Name + " (" + Blogger.EmailAddress + ")"; } }

        ///// <summary>
        ///// This is a property to be filled in by the DelegateDecompilier
        ///// </summary>
        //[Computed]
        //public IEnumerable<string> TagNames { get { return Tags.Select(x => x.Name); } }
        ////public IEnumerable<string> TagNames { get { return Tags == null ? new string[]{} : Tags.Select(x => x.Name); } }

    class Tests06DelegateDecompiler
    {

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            using (var db = new SampleWebAppDb())
            {
                DataLayerInitialise.InitialiseThis();
                var filepath = TestFileHelpers.GetTestFileFilePath("DbContentSimple.xml");
                DataLayerInitialise.ResetDatabaseToTestData(db, filepath);
            }
        }

        [Test]
        public void Test01NoComputedPostListThrowException()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP

                //ATTEMPT
                var ex = Assert.Throws<NotSupportedException>(() => db.Posts.Select(x => x.BloggerNameAndEmail).ToList());

                //VERIFY
                ex.Message.ShouldStartWith("The specified type member 'BloggerNameAndEmail' is not supported in LINQ to Entities.");
            }
        }

        [Test]
        public void Test02ComputedOnPostListOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP

                //ATTEMPT
                var list = db.Posts.Select(x => x.BloggerNameAndEmail).Decompile().ToList();

                //VERIFY
                list.Count.ShouldBeGreaterThan(0);
                list.All( x => x.EndsWith("nospam.com)")).ShouldEqual(true);
            }
        }

        [Test]
        public void Test03ComputedOnPostSingleNoSelectOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var firstPost = db.Posts.First();

                //ATTEMPT
                var single = db.Posts.Where(x => x.PostId == firstPost.PostId).Decompile().Single();

                //VERIFY
                single.BloggerNameAndEmail.ShouldEndWith("nospam.com)");
            }
        }

        [Test]
        public void Test04ComputedOnPostSingleOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var firstPost = db.Posts.First();

                //ATTEMPT
                var bAndE = db.Posts.Where(x => x.PostId == firstPost.PostId).Select(x => x.BloggerNameAndEmail).Decompile().Single();

                //VERIFY
                bAndE.ShouldEndWith("nospam.com)");
            }
        }

        [Test]
        public void Test05ComputedOnPostSingleCollectionOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var firstPost = db.Posts.First();

                //ATTEMPT
                var single = db.Posts.Include( x => x.Tags).Where(x => x.PostId == firstPost.PostId).Decompile().Single();

                //VERIFY
                var tagNames = single.TagNames.ToList();
                tagNames.Count.ShouldBeGreaterThan(0);
            }
        }

    }
}
