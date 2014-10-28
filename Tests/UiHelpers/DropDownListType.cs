#region licence
// The MIT License (MIT)
// 
// Filename: DropDownListType.cs
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
