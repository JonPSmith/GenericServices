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
                GenericServicesConfig.SqlErrorDict.Select(x => new KeyValuePair<int, string>(x.Key, x.Value))
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
            _rememberDefaultSqlErrorTextDict.ForEach(x => GenericServicesConfig.AddToSqlErrorDict(x.Key, x.Value));
        }

        //---------------------

        [Test]
        public void Test01ClearSqlErrorDictOk()
        {

            //SETUP  

            //ATTEMPT
            GenericServicesConfig.ClearSqlErrorDict();

            //VERIFY
            GenericServicesConfig.SqlErrorDict.Count.ShouldEqual(0);
        }

        [Test]
        public void Test02DefaultSqlErrorDictOk()
        {

            //SETUP  

            //ATTEMPT
            GenericServicesConfig.ClearSqlErrorDict();
            RestoreSqlErrorTextDict();

            //VERIFY
            CollectionAssert.AreEquivalent(new[] {547, 2601}, GenericServicesConfig.SqlErrorDict.Keys);
        }

        [Test]
        public void Test05AddNewSqlErrorDictItemOk()
        {

            //SETUP  

            //ATTEMPT
            GenericServicesConfig.ClearSqlErrorDict();
            GenericServicesConfig.AddToSqlErrorDict(-1, "A test");

            //VERIFY
            GenericServicesConfig.SqlErrorDict.Count.ShouldEqual(1);
            GenericServicesConfig.SqlErrorDict[-1].ShouldEqual("A test");
        }

        [Test]
        public void Test05UpdateSqlErrorDictItemOk()
        {

            //SETUP  

            //ATTEMPT
            GenericServicesConfig.ClearSqlErrorDict();
            GenericServicesConfig.AddToSqlErrorDict(-1, "A test");
            GenericServicesConfig.AddToSqlErrorDict(-1, "Another test");

            //VERIFY
            GenericServicesConfig.SqlErrorDict.Count.ShouldEqual(1);
            GenericServicesConfig.SqlErrorDict[-1].ShouldEqual("Another test");
        }

    }
}
