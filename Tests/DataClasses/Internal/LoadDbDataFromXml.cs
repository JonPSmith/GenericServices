using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
using Tests.DataClasses.Concrete;

[assembly: InternalsVisibleTo("Tests")]

namespace Tests.DataClasses.Internal
{

    internal class LoadDbDataFromXml
    {
        private Dictionary<string, Tag> _tagsDict;


        public List<Blog> Bloggers { get; private set; }

        public List<Post> Posts { get; private set; }

        public IEnumerable<Tag> Tags { get { return _tagsDict.Values; } }

        public List<PostTagGrade> PostTagGrades { get; private set; }

        //ctor
        public LoadDbDataFromXml(string fullFilepath)
        {
            //var assemblyHoldingFile = Assembly.GetExecutingAssembly();

            //using (var fileStream = assemblyHoldingFile.GetManifestResourceStream(fullFilepath))
            using (var fileStream = new XmlTextReader(fullFilepath))
            {
                var xmlData = XElement.Load(fileStream);

                //now decode and return
                _tagsDict = DecodeTags(xmlData.Element("Tags"));
                DecodeBlogsAndGrades(xmlData.Element("Blogs"), _tagsDict);
            }
        }

        private void DecodeBlogsAndGrades(XElement element, Dictionary<string, Tag> tagsDict)
        {
            Bloggers = new List<Blog>();
            Posts = new List<Post>();
            PostTagGrades = new List<PostTagGrade>();
            foreach (var blogXml in element.Elements("Blog"))
            {
                var newBlogger = new Blog()
                {
                    Name = blogXml.Element("Name").Value,
                    EmailAddress = blogXml.Element("Email").Value,
                    Posts = new Collection<Post>()
                };

                foreach (var postXml in blogXml.Element("Posts").Elements("Post"))
                {
                    var newPost = new Post()
                    {
                        Blogger = newBlogger,
                        Title = postXml.Element("Title").Value,
                        Content = postXml.Element("Content").Value,
                        Tags = postXml.Element("TagSlugs").Value.Split(',').Select(x => tagsDict[x.Trim()]).ToList()
                    };


                    //look for PostTagGrades for this post
                    foreach (var postTagXml in postXml.Elements("PostTagGrade"))
                    {
                        var newPostTag = new PostTagGrade
                        {
                            PostPart = newPost,
                            TagPart = tagsDict[postTagXml.Element("TagSlug").Value.Trim()],
                            Grade = int.Parse( postTagXml.Element("Grade").Value)
                        };
                        PostTagGrades.Add( newPostTag);
                    }
                    Posts.Add(newPost);
                    newBlogger.Posts.Add(newPost);

                }
                Bloggers.Add(newBlogger);
            }
        }

        private static Dictionary<string, Tag> DecodeTags(XElement element)
        {
            var result = new Dictionary<string, Tag>();
            foreach (var newTag in element.Elements("Tag").Select(tagXml => new Tag()
            {
                Name = tagXml.Element("Name").Value,
                Slug = tagXml.Element("Slug").Value
            }))
            {
                result[newTag.Slug] = newTag;
            }
            return result;
        }
    }
}
