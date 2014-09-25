using System.Collections.Generic;
using System.Linq;
using GenericServices;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests.Group01Configuration
{
    class Test02SqlDict
    {

        private List<KeyValuePair<int, string>> _rememberDefaultSqlErrorTextDict ;
            
            
        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            //This remembers the SqlErrorTextDict
            _rememberDefaultSqlErrorTextDict =
                ServicesConfiguration.SqlErrorDict.Select(x => new KeyValuePair<int, string>(x.Key, x.Value))
                    .ToList();

        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            //This resets the SqlErrorTextDict
            RestoreSqlErrorTextDict();
        }

        private void RestoreSqlErrorTextDict()
        {
            _rememberDefaultSqlErrorTextDict.ForEach(x => ServicesConfiguration.AddToSqlErrorDict(x.Key, x.Value));
        }

        //---------------------

        [Test]
        public void Test01ClearSqlErrorDictOk()
        {

            //SETUP  

            //ATTEMPT
            ServicesConfiguration.ClearSqlErrorDict();

            //VERIFY
            ServicesConfiguration.SqlErrorDict.Count.ShouldEqual(0);
        }

        [Test]
        public void Test02DefaultSqlErrorDictOk()
        {

            //SETUP  

            //ATTEMPT
            ServicesConfiguration.ClearSqlErrorDict();
            RestoreSqlErrorTextDict();

            //VERIFY
            CollectionAssert.AreEquivalent(new[] {547, 2601}, ServicesConfiguration.SqlErrorDict.Keys);
        }

        [Test]
        public void Test05AddNewSqlErrorDictItemOk()
        {

            //SETUP  

            //ATTEMPT
            ServicesConfiguration.ClearSqlErrorDict();
            ServicesConfiguration.AddToSqlErrorDict(-1, "A test");

            //VERIFY
            ServicesConfiguration.SqlErrorDict.Count.ShouldEqual(1);
            ServicesConfiguration.SqlErrorDict[-1].ShouldEqual("A test");
        }

        [Test]
        public void Test05UpdateSqlErrorDictItemOk()
        {

            //SETUP  

            //ATTEMPT
            ServicesConfiguration.ClearSqlErrorDict();
            ServicesConfiguration.AddToSqlErrorDict(-1, "A test");
            ServicesConfiguration.AddToSqlErrorDict(-1, "Another test");

            //VERIFY
            ServicesConfiguration.SqlErrorDict.Count.ShouldEqual(1);
            ServicesConfiguration.SqlErrorDict[-1].ShouldEqual("Another test");
        }

    }
}
