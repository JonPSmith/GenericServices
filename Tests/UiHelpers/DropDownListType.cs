using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Tests.UiHelpers
{
    public class DropDownListType
    {

        /// <summary>
        /// This contains the strings to show and the value to return if that string is selected
        /// </summary>
        public Collection<KeyValuePair<string,string>> KeyValueList { get; private set; }

        /// <summary>
        /// This is the value returned from the list
        /// </summary>
        [Required]
        public string SelectedValue { get; set; }

        /// <summary>
        /// This returns an integer if the SelectedValue is a string, otherwise null
        /// </summary>
        public int? SelectedValueAsInt
        {
            get
            {
                int result;
                if (int.TryParse(SelectedValue, out result))
                    return result;

                return null;
            }         
        }


        /// <summary>
        /// This sets up the KeyValueList to the given list
        /// This must be done before handed to MVC for display (or redisplay on error)
        /// </summary>
        /// <param name="keyValueList"></param>
        /// <param name="promptString">If supplied then puts up string and the user must select something. Otherwise first item selected</param>
        public void SetupDropDownListContent(IEnumerable<KeyValuePair<string, string>> keyValueList, 
                                string promptString)
        {
            KeyValueList = new Collection<KeyValuePair<string, string>>(keyValueList.ToList());       //take a copy
            if (promptString != null)
                KeyValueList.Insert(0, new KeyValuePair<string, string>(promptString, null));
            else
                SelectedValue = KeyValueList[0].Value;
        }

        public void SetSelectedValue(string valueAsString)
        {
            var foundEntry = KeyValueList.FirstOrDefault(x => x.Value == valueAsString);
            SelectedValue = KeyValueList.Any(x => x.Value == valueAsString) 
                ? KeyValueList.First(x => x.Value == valueAsString).Value 
                : "--- selecte from list ---";
        }

        public override string ToString()
        {
            if (SelectedValue != null )
                return string.Format("Selected Value = {0}", SelectedValue);

            if (KeyValueList == null)
                return "KeyValue list has not been set up yet.";

            return string.Format("{0} items in list, but no selected value", KeyValueList.Count);

        }

    }
}
