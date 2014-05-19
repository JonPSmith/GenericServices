using System;
using System.Collections.Generic;
using System.Linq;
using GenericServices;
using Tests.DataClasses;

namespace Tests.DTOs.Concrete
{
    public class PostSetupService : IPostSetupService
    {
        
        private readonly TemplateWebAppDb _db;

        public PostSetupService(IDbContextWithValidation db)
        {
            _db = db as TemplateWebAppDb;
            if (_db == null)
                throw new NullReferenceException("The IDbContextWithValidation must be linked to TemplateWebAppDb.");
        }

        //------------------------------------------------------------------------------
        //additional methods for setting up items related to the update or creation of the TData

        /// <summary>
        /// This should be called if there is an error and the screen needs to be reshown. It resets the dropdownLists etc with data from the database
        /// </summary>
        /// <param name="dto"></param>
        public void SetupDropDownLists(IDetailPostDto dto)
        {
            dto.Bloggers.SetupDropDownListContent( _db.Blogs.ToList().Select( x => new KeyValuePair<string, string>(x.Name, x.BlogId.ToString("D"))), "--- choose blogger ---");
            if (dto.PostId != 0)
                //there is an entry, so set the selected value to that
                dto.Bloggers.SetSelectedValue( dto.BlogId.ToString("D"));

            var preselectedTags = dto.PostId == 0
                ? new List<KeyValuePair<string, int>>()
                : _db.PostTagLinks
                    .Where(x => x.PostId == dto.PostId)
                    .Select( x => new { Key = x.HasTag.Name, Value = x.TagId})
                    .ToList()
                    .Select(x => new KeyValuePair<string, int>(x.Key, x.Value))
                    .ToList();
            dto.UserChosenTags.SetupMultiSelectList(
                _db.Tags.ToList().Select(x => new KeyValuePair<string, int>(x.Name, x.TagId)), preselectedTags);
        }
       
    }
}
