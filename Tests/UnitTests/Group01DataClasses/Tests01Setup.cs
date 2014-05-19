using System.Linq;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.Helpers;

namespace Tests.UnitTests.Group01DataClasses
{
    class Tests01Setup
    {
        [Test]
        public void Check01DatabaseResetOk()
        {
            using (var db = new TemplateWebAppDb())
            {
                //SETUP
                DataLayerInitialise.InitialiseThis();

                //ATTEMPT
                DataLayerInitialise.ResetDatabaseToTestData(db);

                //VERIFY
                db.Blogs.Count().ShouldEqual(2);
                db.Posts.Count().ShouldEqual(3);
                db.Tags.Count().ShouldEqual(3);
            }
        }

    }
}
