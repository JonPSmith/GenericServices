#region licence
// The MIT License (MIT)
// 
// Filename: DbSnapShot.cs
// Date Created: 2014/05/19
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
using System.Linq;
using Tests.DataClasses;

namespace Tests.Helpers
{
    public class DbSnapShot
    {

        public int NumPosts { get; private set; }

        public int NumPostTagLinks { get; private set; }
        
        public int NumBlogs { get; private set; }

        public int NumTags { get; private set; }

        public int NumPostTagGrades { get; private set; }

        public DbSnapShot(SampleWebAppDb db)
        {
            NumBlogs = db.Blogs.Count();
            NumPostTagLinks = db.Database.SqlQuery<int>("SELECT COUNT(*) FROM dbo.TagPosts").First();
            NumPosts = db.Posts.Count();
            NumTags = db.Tags.Count();
            NumPostTagGrades = db.PostTagGrades.Count();
        }

        //creates snapshot setting zero on everything
        public DbSnapShot() { }

        public void CheckSnapShot(SampleWebAppDb db, int postsChange = 0, int postTagLinkChange = 0, int blogsChange = 0, int tagsChange = 0, int postTagGradesChange = 0)
        {
            var newSnap = new DbSnapShot(db);

            newSnap.NumPosts.ShouldEqual( NumPosts + postsChange, "posts wrong");
            newSnap.NumPostTagLinks.ShouldEqual(NumPostTagLinks + postTagLinkChange, "posttaglinks wrong");
            newSnap.NumBlogs.ShouldEqual(NumBlogs + blogsChange, "blogs wrong");
            newSnap.NumTags.ShouldEqual(NumTags + tagsChange, "tags wrong");
            newSnap.NumPostTagGrades.ShouldEqual(NumPostTagGrades + postTagGradesChange, "postTagGrades wrong");
        }

    }
}
