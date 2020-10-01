using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Ookii.Dialogs;

namespace SFB
{
    public class WindowWrapper : IWin32Window
    {
        private IntPtr _hwnd;
        public WindowWrapper(IntPtr handle) { _hwnd = handle; }
        public IntPtr Handle { get { return _hwnd; } }
    }
    public class StandaloneFileBrowserWindows : IStandaloneFileBrowser
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        public string[] OpenFilePanel(string title, string directory, ExtensionFilter[] extensions, bool mutiselect)
        {
            var fd = new VistaOpenFileDialog();
            fd.Title = title;
            if (extensions != null)
            {
                fd.Filter = GetFilterFromFileExtensionList(extensions);
                fd.FilterIndex = 1;
            }
            else
            {
                fd.Filter = string.Empty;
            }
            fd.Multiselect = mutiselect;
            if(!string.IsNullOrEmpty(directory))
            {
                fd.FileName = GetDirectoryPath(directory);
            }
            var res = fd.ShowDialog(new WindowWrapper(GetActiveWindow()));
            var filenames = res == DialogResult.OK ? fd.FileNames : new string[0];
            fd.Dispose();
            return filenames;
        }

        public string[] OpenFolderPanel(string title, string directory, bool mutiselect)
        {
            var fd = new VistaFolderBrowserDialog();
            fd.Description = title;

            if (!string.IsNullOrEmpty(directory))
            {
                fd.SelectedPath = GetDirectoryPath(directory);
            }

            var res = fd.ShowDialog(new WindowWrapper(GetActiveWindow()));
            var filenames = res == DialogResult.OK ? new[] { fd.SelectedPath } : new string[0];
            fd.Dispose();

            return filenames;
        }

        private static string GetFilterFromFileExtensionList(ExtensionFilter[] extensions)
        {
            var filterString = "";

            foreach (var filter in extensions)
            {
                filterString += filter.Name + "(";

                foreach (var ext in filter.Extensions)
                {
                    filterString += "*." + ext + ",";
                }

                filterString = filterString.Remove(filterString.Length - 1);
                filterString += ") |";

                foreach (var ext in filter.Extensions)
                {
                    filterString += "*." + ext + "; ";
                }

                filterString += "|";
            }

            filterString = filterString.Remove(filterString.Length - 1);

            return filterString;
        }

        private static string GetDirectoryPath(string directory)
        {
            var directoryPath = Path.GetFullPath(directory);

            if (!directoryPath.EndsWith("\\"))
            {
                directoryPath += "\\";
            }
            
            if (Path.GetPathRoot(directoryPath) == directoryPath)
            {
                return directory;
            }

            return Path.GetDirectoryName(directoryPath) + Path.DirectorySeparatorChar;
        }
    }

}
