using System;

namespace SFB
{
    public struct ExtensionFilter
    {
        public string Name;
        public string[] Extensions;

        public ExtensionFilter(string filterName, params string[] filterExtensions)
        {
            Name = filterName;
            Extensions = filterExtensions;
        }
    }

    public class StandaloneFileBrowser
    {
        private static IStandaloneFileBrowser _platformWrapper = null;

        static StandaloneFileBrowser()
        {
            _platformWrapper = new StandaloneFileBrowserWindows();
        }

        public static string[] OpenFilePanel(string title, string directory, string extension, bool multiselect)
        {
            var extensions = string.IsNullOrEmpty(extension) ? null : new[] {new ExtensionFilter("",extension)};
            return OpenFilePanel(title, directory, extensions, multiselect);
        }

        public static string[] OpenFilePanel(string title, string directory, ExtensionFilter[] extensions, bool mutiselect)
        {
            return _platformWrapper.OpenFilePanel(title, directory, extensions, mutiselect);
        }

        public static string[] OpenFolderPanel(string title, string directory, bool multiselect)
        {
            return _platformWrapper.OpenFolderPanel(title, directory, multiselect);
        }
    }
}
