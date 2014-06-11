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
            context.SaveChanges();

            var loader = new LoadDbDataFromXml(filepathOfXmlFile);

            context.Blogs.AddRange(loader.Bloggers);                //note: The order things appear in the database are not obvious
            //have to add these by hand
            context.PostTagGrades.AddRange(loader.PostTagGrades);
            context.SaveChanges();
        }
    }

}
