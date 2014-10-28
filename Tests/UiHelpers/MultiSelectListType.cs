#region licence
// The MIT License (MIT)
// 
// Filename: MultiSelectListType.cs
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Tests.UiHelpers
{
    public class MultiSelectListType
    {

        /// <summary>
        /// This contains the strings to show and the value to return if that string is selected
        /// </summary>
        public List<KeyValuePair<string, int>> AllPossibleOptions { get; private set; }

        /// <summary>
        /// This contains the strings to show and the value to return if that string is selected
        /// </summary>
        public List<KeyValuePair<string, int>> InitialSelection { get; private set; }

        /// <summary>
        /// This must be set by the returning form with the values (as strings)
        /// </summary>
        public string [] FinalSelection { get; set; }

        /// <summary>
        /// This sets up the AllPossibleOptions and the InitialSelection list
        /// This must be done before handed to MVC for display (or redisplay on error)
        /// </summary>
        /// <param name="allPossibleOptions"></param>
        /// <param name="initialSelectionValues">the Ids of the initial selected values</param>
        public void SetupMultiSelectList(IEnumerable<KeyValuePair<string, int>> allPossibleOptions,
                                         IEnumerable<KeyValuePair<string, int>> initialSelectionValues)
        {
            AllPossibleOptions = allPossibleOptions.ToList() ;           //we take copies of the collections
            InitialSelection = initialSelectionValues.ToList();
            FinalSelection = InitialSelection.Select(x => x.Value.ToString("D")).ToArray();
        }


        /// <summary>
        /// This returns the multiselection integers in the order that the user selected them
        /// </summary>
        /// <returns></returns>
        public int[] GetFinalSelectionAsInts()
        {
            var result = new Collection<int>();
            if (FinalSelection == null)
                //the FinalSelection can be null if no item has been selected, so return empty array
                return result.ToArray();

            foreach (var intAsString in FinalSelection)
            {
                int id;
                if (!int.TryParse(intAsString, out id))
                    throw new ArgumentException("One of the FinalSelection answers was not an integer");
                
                result.Add(id);
            }
            return result.ToArray();
        }
    }
}
