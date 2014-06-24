﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GenericServices;
using GenericServices.Core;
using GenericServices.Services;
using GenericServices.ServicesAsync;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.UiHelpers;

[assembly: InternalsVisibleTo("Tests")]

namespace Tests.DTOs.Concrete
{
    public class DetailPostDtoAsync : InstrumentedEfGenericDtoAsync<Post, DetailPostDtoAsync>
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

        public ICollection<Tag> Tags { get; set; }            //this must be copied back

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

        public string TagNames { get { return string.Join(", ", Tags.Select(x => x.Name)); } }


        //ctor
        public DetailPostDtoAsync()
        {
            Bloggers = new DropDownListType();
            UserChosenTags = new MultiSelectListType();
        }


        //----------------------------------------------
        //overridden methods

        internal protected override ServiceFunctions SupportedFunctions
        {
            get { return ServiceFunctions.AllCrud; }
        }

        /// <summary>
        /// This sets up the dropdownlist for the possible bloggers and the MultiSelectList of tags
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        internal protected override async Task SetupSecondaryDataAsync(IDbContextWithValidation context, DetailPostDtoAsync dto)
        {

            var bloggers = await context.Set<Blog>().ToListAsync();

            dto.Bloggers.SetupDropDownListContent(bloggers.Select(x => new KeyValuePair<string, string>(x.Name, x.BlogId.ToString("D"))),
                "--- choose blogger ---");
            if (dto.PostId != 0)
                //there is an entry, so set the selected value to that
                dto.Bloggers.SetSelectedValue( dto.BlogId.ToString("D"));

            List<KeyValuePair<string, int>> preselectedTags;
            if (dto.PostId == 0)
            {
                //create, so just produce empty list
                preselectedTags = new List<KeyValuePair<string, int>>();
            }
            else
            {

                var tags = await context.Set<Tag>()
                    .Where(x => x.Posts.Any(y => y.PostId == dto.PostId))
                    .Select(x => new {Key = x.Name, Value = x.TagId})
                    .ToListAsync();
                preselectedTags = tags.Select(x => new KeyValuePair<string, int>(x.Key, x.Value))
                    .ToList();
            }

            dto.UserChosenTags.SetupMultiSelectList(
                context.Set<Tag>().ToList().Select(x => new KeyValuePair<string, int>(x.Name, x.TagId)), preselectedTags);
        }

        internal protected override async Task<ISuccessOrErrors> CopyDtoToDataAsync(IDbContextWithValidation context, DetailPostDtoAsync dto, Post post)
        {

            var db = context as SampleWebAppDb;
            if (db == null)
                throw new NullReferenceException("The IDbContextWithValidation must be linked to TemplateWebAppDb.");

            var status = SuccessOrErrors.Success("OK if no errors set");

            //now we sort out the blogger
            var errMsg = await SetupBloggerIdFromDropDownList(db, post);
            if (errMsg != null)
                status.AddNamedParameterError("Bloggers", errMsg);

            //now we sort out the tags
            errMsg = await ChangeTagsBasedOnMultiSelectList(db, post);
            if (errMsg != null)
                status.AddNamedParameterError("UserChosenTags", errMsg);

            if (status.IsValid)
                //now we copy the items to the right place
                status = await base.CopyDtoToDataAsync(context, dto, post);

            return status;
        }


        //---------------------------------------------------
        //private helpers

        private async Task<string> SetupBloggerIdFromDropDownList(SampleWebAppDb db, Post post)
        {
            
            var blogId = Bloggers.SelectedValueAsInt;
            if (blogId == null)
                return "The blogger was not selected. You must do that before the post can be saved.";
            
            var blogger = await db.Blogs.FindAsync((int) blogId);
            if (blogger == null)
                return "Could not find the blogger you selected. Did another user delete it?";
            
            //all ok
            if ((int) blogId != post.BlogId)
                post.Blogger = null;                //old info is incorrect, so remove it

            BlogId = (int) blogId;
            return null;
        }

        private async Task<string> ChangeTagsBasedOnMultiSelectList(SampleWebAppDb db, Post post)
        {
            var requiredTagIds = UserChosenTags.GetFinalSelectionAsInts();
            if (!requiredTagIds.Any())
                return "You must select at least one tag for the post.";

            if (requiredTagIds.Any(x => db.Tags.Find(x) == null))
                return "Could not find one of the tags. Did another user delete it?";

            if (post.PostId != 0)
                //This is an update so we need to load the tags
                db.Entry(post).Collection(p => p.Tags).Load();

            var newTagsForPost = await db.Tags.Where(x => requiredTagIds.Contains(x.TagId)).ToListAsync();
            Tags = newTagsForPost;      //will be copied over by copyDtoToData

            return null;
        }
    }
}
