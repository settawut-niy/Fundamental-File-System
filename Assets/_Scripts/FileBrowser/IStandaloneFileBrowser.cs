using System;

namespace SFB
{
    public interface IStandaloneFileBrowser
    {
        string[] OpenFilePanel(string title, string directory, ExtensionFilter[] extensions, bool mutiselect);
        string[] OpenFolderPanel(string title, string directory, bool mutiselect);
    }
}
