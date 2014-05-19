using System;
using System.Collections.Generic;
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
            AllPossibleOptions = allPossibleOptions.ToList();           //we take copies of the collections
            InitialSelection = initialSelectionValues.ToList();
            FinalSelection = InitialSelection.Select(x => x.Value.ToString("D")).ToArray();
        }


        /// <summary>
        /// This returns the multiselection integers in the order that the user selected them
        /// </summary>
        /// <returns></returns>
        public int[] GetFinalSelectionAsInts()
        {
            if (FinalSelection == null)
                throw new NullReferenceException(
                    "FinalSelection of the MultiSelectList is null. Did you forget to set it in the view?");

            var result = new List<int>();
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
