#region licence
// The MIT License (MIT)
// 
// Filename: Tests12BuildFilter.cs
// Date Created: 2014/07/20
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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GenericServices.Core.Internal;
using NUnit.Framework;
using Tests.DataClasses.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group01DataClasses
{
    class Tests12BuildFilter
    {
        private IQueryable<Post> _posts;

        private PropertyInfo _postIdProp;
        private PropertyInfo _blogIdProp;
        private PropertyInfo _titleIdProp;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            var post1 = new Post() { PostId = 123, BlogId = 321, Title = "AAA"};
            var post2 = new Post() { PostId = 234, BlogId = 432, Title = "BBB" };
            _posts = (new List<Post> { post1, post2 }).AsQueryable();
            _postIdProp = typeof(Post).GetProperty("PostId");
            _blogIdProp = typeof(Post).GetProperty("BlogId");
            _titleIdProp = typeof(Post).GetProperty("Title");
            //There seems to be a setup cost. Once the query is built once then its fast.
            var filter = BuildFilter.CreateFilter<Post>(new PropertyInfo[] { _postIdProp }, new object[] { 123 });
            var cache = _posts.Where(filter).ToList();
        }

        [Test]
        public void Check01LocalBuildOfWhereOk()
        {
            //SETUP
            var myInt = 123;

            //ATTEMPT
            Expression<Func<Post, bool>> filter;
            using (new TimerToConsole("Build filter"))
                filter = BuildEqual( _posts, _postIdProp, myInt);

            //VERIFY
            List<Post> result;
            using (new TimerToConsole("Run query"))
            {
                result = _posts.Where(filter).ToList();
                // result = posts.Where(x => x.PostId == myInt).ToList();
            }
            result.Count.ShouldEqual(1);
        }

        //-------------------------------------------

        private static Expression<Func<T, bool>> BuildEqual<T>(IQueryable<T> source, PropertyInfo prop, object expectedValue)
        {
            var p = Expression.Parameter(typeof(T), "p");
            var m = Expression.Property(p, prop);
            var c = Expression.Constant(expectedValue);
            var ex = Expression.Equal(m, c);

            return Expression.Lambda<Func<T, bool>>(ex, p);
        }


        [Test]
        public void Check02ReturnIQueryableOneIntKeyOk()
        {
            //SETUP
            var myInt = 123;
            Expression<Func<Post, bool>> filter;

            //ATTEMPT
            using (new TimerToConsole("Build filter"))
                 filter = BuildFilter.CreateFilter<Post>(new [] {_postIdProp}, new object[] {myInt});

            List<Post> result;
            using (new TimerToConsole("Run query"))
                result = _posts.Where(filter).ToList();

            //VERIFY
            result.Count.ShouldEqual(1);
        }

        [Test]
        public void Check03ReturnIQueryableOneStringKeyOk()
        {
            //SETUP
            Expression<Func<Post, bool>> filter;

            //ATTEMPT
            using (new TimerToConsole("Build filter"))
                filter = BuildFilter.CreateFilter<Post>(new[] { _titleIdProp }, new object[] { "BBB" });

            List<Post> result;
            using (new TimerToConsole("Run query"))
                result = _posts.Where(filter).ToList();

            //VERIFY
            result.Count.ShouldEqual(1);
        }

        [Test]
        public void Check05ReturnIQueryableTwoKeysOk()
        {
            //SETUP
            Expression<Func<Post, bool>> filter;

            //ATTEMPT
            using (new TimerToConsole("Build filter"))
            {
                filter = BuildFilter.CreateFilter<Post>(new PropertyInfo[] { _postIdProp, _blogIdProp }, new object[] { 123, 321 });
                //query = _posts.Where(x => x.PostId == 123 && x.BlogId == 321);
            }

            List<Post> result;
            using (new TimerToConsole("Run query"))
                result = _posts.Where(filter).ToList();

            //VERIFY
            result.Count.ShouldEqual(1);
        }


        //-------------------------------------------
        //error conditions

        [Test]
        public void Check20KeyWrongTypeBad()
        {
            //SETUP

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>( () => BuildFilter.CreateFilter<Post>(new [] {_postIdProp}, new object[] {"AAA"}));

            //VERIFY
        }

        [Test]
        public void Check21WrongNumberOfKeysBad()
        {
            //SETUP

            //ATTEMPT
            var ex = Assert.Throws<ArgumentException>(() => BuildFilter.CreateFilter<Post>(new[] { _postIdProp }, new object[] { 123, 321 }));

            //VERIFY
        }





    }
}
