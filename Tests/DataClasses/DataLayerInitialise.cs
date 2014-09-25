#region licence
// The MIT License (MIT)
// 
// Filename: DataLayerInitialise.cs
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
using System.Data.Entity;
using Tests.DataClasses.Internal;

namespace Tests.DataClasses
{

    public static class DataLayerInitialise
    {


        /// <summary>
        /// This should be called at Startup
        /// </summary>
        public static void InitialiseThis()
        {
            //Initialise the database
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<SampleWebAppDb>());
        }

        public static void ResetDatabaseToTestData(SampleWebAppDb context, string filepathOfXmlFile)
        {
            context.Posts.RemoveRange( context.Posts);
            context.Tags.RemoveRange( context.Tags);
            context.Blogs.RemoveRange( context.Blogs);
            context.PostTagGrades.RemoveRange(context.PostTagGrades);
            context.PostLinks.RemoveRange(context.PostLinks);
            context.SaveChanges();

            var loader = new LoadDbDataFromXml(filepathOfXmlFile);

            context.Blogs.AddRange(loader.Bloggers);                //note: The order things appear in the database are not obvious
            //have to add these by hand
            context.PostTagGrades.AddRange(loader.PostTagGrades);
            context.SaveChanges();
        }
    }

}
