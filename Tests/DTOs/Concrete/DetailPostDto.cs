using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using GenericServices;
using GenericServices.Concrete;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.UiHelpers;

[assembly: InternalsVisibleTo("Tests")]

namespace Tests.DTOs.Concrete
{
    public class DetailPostDto : EfGenericDto<Post, DetailPostDto>, IDetailPostDto
    {

        [UIHint("HiddenInput")]
        [Key]
        public int PostId { get; set; }

        [MinLength(2), MaxLength(128)]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        //-------------------------------------------
        //properties that cannot be set directly (The data layer looks after them)

        public DateTime LastUpdated { get; internal set; }

        //------------------------------------------
        //these two items are altered by the  

        [UIHint("HiddenInput")]
        public int BlogId { get; set; }

        public List<PostTagLink> AllocatedTags { get;  set; }            //this must be copied back

        //------------------------------------------
        //Item set up if update

        public string BloggerName { get; internal set; }

        //-------------------------------------------
        //now the various lists for user interaction

        /// <summary>
        /// This allows a single blogger to be chosen from the list
        /// </summary>
        public DropDownListType Bloggers { get; set; }

        public MultiSelectListType UserChosenTags { get; set; }

        //-------------------------------------------
        //calculated properties to help display

        /// <summary>
        /// When it was last updated in DateTime format
        /// </summary>
        public DateTime LastUpdatedUtc { get { return DateTime.SpecifyKind(LastUpdated, DateTimeKind.Utc); } }

        public string TagNames { get { return string.Join(", ", AllocatedTags.Select(x => x.HasTag.Name)); } }


        //ctor
        public DetailPostDto()
        {
            Bloggers = new DropDownListType();
            UserChosenTags = new MultiSelectListType();
        }


        //-----------------------------------------------------------------------------
        //override methods

        //----------------------------------------------
        //overridden methods

        internal protected override CrudFunctions SupportedFunctions
        {
            get { return CrudFunctions.All; }
        }

        /// <summary>
        /// This sets up the dropdownlist for the possible bloggers and the MultiSelectList of tags
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        internal protected override void SetupSecondaryData(IDbContextWithValidation context, DetailPostDto dto)
        {

            dto.Bloggers.SetupDropDownListContent(
                context.Set<Blog>()
                    .ToList()
                    .Select(x => new KeyValuePair<string, string>(x.Name, x.BlogId.ToString("D"))),
                "--- choose blogger ---");
            if (dto.PostId != 0)
                //there is an entry, so set the selected value to that
                dto.Bloggers.SetSelectedValue( dto.BlogId.ToString("D"));

            var preselectedTags = dto.PostId == 0
                ? new List<KeyValuePair<string, int>>()
                : context.Set<PostTagLink>()
                    .Where(x => x.PostId == dto.PostId)
                    .Select( x => new { Key = x.HasTag.Name, Value = x.TagId})
                    .ToList()
                    .Select(x => new KeyValuePair<string, int>(x.Key, x.Value))
                    .ToList();
            dto.UserChosenTags.SetupMultiSelectList(
                context.Set<Tag>().ToList().Select(x => new KeyValuePair<string, int>(x.Name, x.TagId)), preselectedTags);
        }

        internal protected override ISuccessOrErrors CopyDtoToData(IDbContextWithValidation context, DetailPostDto dto, Post post)
        {

            var db = context as TemplateWebAppDb;
            if (db == null)
                throw new NullReferenceException("The IDbContextWithValidation must be linked to TemplateWebAppDb.");

            //Copy over the standard items
            var status = new SuccessOrErrors().SetSuccessMessage("OK if no errors set");

            //now we sort out the blogger
            var errMsg = SetupBloggerIdFromDropDownList(db, post);
            if (errMsg != null)
                status.AddSingleError(errMsg);

            //now we sort out the tags
            errMsg = ChangeTagsBasedOnMultiSelectList(db, post);
            if (errMsg != null)
                status.AddSingleError(errMsg);

            if (status.IsValid)
                //now we copy the items to the right place
                status = base.CopyDtoToData(context, dto, post);

            return status;
        }


        //---------------------------------------------------
        //private helpers

        private string SetupBloggerIdFromDropDownList(TemplateWebAppDb db, Post post)
        {
            
            var blogId = Bloggers.SelectedValueAsInt;
            if (blogId == null)
                return "The blogger was not selected. You must do that before the post can be saved.";
            
            var blogger = db.Blogs.Find((int) blogId);
            if (blogger == null)
                return "Could not find the blogger you selected, which is odd.";
            
            //all ok
            if ((int) blogId != post.BlogId)
                post.Blogger = null;                //old info is incorrect, so remove it

            BlogId = (int) blogId;
            return null;
        }

        private string ChangeTagsBasedOnMultiSelectList(TemplateWebAppDb db, Post post)
        {
            var requiredTagIds = UserChosenTags.GetFinalSelectionAsInts();
            if (!requiredTagIds.Any())
                return "You must select at least one tag for the post.";

            if (requiredTagIds.Any(x => db.Tags.Find(x) == null))
                return "Could not find one of the tags, which is odd.";

            var tagLinksToDelete =
                db.PostTagLinks.Where(x => !requiredTagIds.Contains(x.TagId) && x.PostId == PostId).ToList();
            var tagLinksToAdd = requiredTagIds
                .Where(x => !db.PostTagLinks.Any(y => y.TagId == x && y.PostId == PostId))
                .Select(z => new PostTagLink {InPost = post, HasTag = db.Tags.Find(z)}).ToList();

            //Now the complicated bit! (has to deal with both update and create, which have different needs)
            //We need to both update the PostTagLinks record AND the Posts AllocatedTags array.
            //If we don't do both we get foreign key problems
            //Now we update the AllocatedTags property in the dto, which the CopyUpdateProperties will then copy into the post
            //First we get the AllocatedTag entry right 
            AllocatedTags = db.PostTagLinks.Where(x => x.PostId == PostId).ToList()        //first part finds current entries...
                .Where( x => !tagLinksToDelete.Any(y => y.TagId == x.TagId))               //then we remove any we don't want any more      
                .ToList();
            //Then add any new ones
            AllocatedTags.AddRange(tagLinksToAdd);

            //secondly we get the PostTagLinks entries right (must come second)
            tagLinksToDelete.ForEach( x => db.PostTagLinks.Remove(x));
            tagLinksToAdd.ForEach(x => db.PostTagLinks.Add(x));
            //********************************************************************
            //If using EF 6 you could use the more efficent RemoveRange. See below
            //db.PostTagLinks.RemoveRange(tagLinksToDelete);
            //db.PostTagLinks.AddRange(tagLinksToAdd);
            //********************************************************************

            return null;
        }
    }
}
