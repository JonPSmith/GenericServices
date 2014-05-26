using System;
using System.IO;

namespace Tests.Helpers
{
    internal static class TestFileHelpers
    {
        private const string TestFileDirectoryName = @"\TestData";

        //-------------------------------------------------------------------

        internal static string GetTestFileFilePath(string searchPattern)
        {
            string[] fileList = GetTestFileFilesOfGivenName(searchPattern);

            if (fileList.Length != 1)
                throw new Exception(string.Format("GetTestFileFilePath: The searchString {0} found {1} file. Either not there or ambiguous",
                    searchPattern, fileList.Length));

            return fileList[0];
        }

        internal static string GetTestFileContent(string searchPattern)
        {
            var filePath = GetTestFileFilePath(searchPattern);
            return File.ReadAllText(filePath);
        }

        internal static string[] GetTestFileFilesOfGivenName(string searchPattern = "")
        {
            var directory = GetTestDataFileDirectory();
            if (searchPattern.Contains(@"\"))
            {
                //Has subdirectory in search pattern, so change directory
                directory = Path.Combine(directory, searchPattern.Substring(0, searchPattern.LastIndexOf('\\')));
                searchPattern = searchPattern.Substring(searchPattern.LastIndexOf('\\')+1);
            }

            string[] fileList = Directory.GetFiles(directory, searchPattern);

            return fileList;
        }

        //------------------------------------------------------------------------------

        public static string GetTestDataFileDirectory(string alternateTestDir = TestFileDirectoryName)
        {
            string pathToManipulate = Environment.CurrentDirectory;
            const string toRemoveFromEnd = @"\bin\debug";
            if (!pathToManipulate.EndsWith(toRemoveFromEnd, StringComparison.InvariantCultureIgnoreCase))
                throw new Exception("bad news guys. Not the expected path");

            return pathToManipulate.Substring(0, pathToManipulate.Length - toRemoveFromEnd.Length) + alternateTestDir;
        }

    }
}
