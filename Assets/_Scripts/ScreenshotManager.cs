using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SFB;

[RequireComponent(typeof(Camera))]
public class ScreenshotManager : MonoBehaviour
{
    public static ScreenshotManager instance;

    // Camera that use to take a screenshot
    Camera mainCamera;

    // Screenshot setting
    int screenshotWidth = 2048, screenshotHeight = 2048;
    bool takeScreenshotOnNextFrame = false;

    // Path of screenshot folder
    string m_selectedPath;

    public string SelectedPath
    {
        get { return m_selectedPath; }
    }

    void Awake()
    {
        instance = this;

        mainCamera = GetComponent<Camera>();

        GenerateDefaultPath();
    }

    void GenerateDefaultPath()
    {
        var Folder = Directory.CreateDirectory(Application.dataPath + "/Screenshots Folder");
        m_selectedPath = Application.dataPath + "/" + Folder.Name;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            TakeScreenshot(screenshotWidth, screenshotHeight);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeScreenshotPathFolder();
        }
    }

    void OnPostRender()
    {
        GenerateScreenshot();
    }

    void TakeScreenshot(int width, int height)
    {
        mainCamera.targetTexture = RenderTexture.GetTemporary(width, height, 16);
        takeScreenshotOnNextFrame = true;
    }

    void GenerateScreenshot()
    {
        if (takeScreenshotOnNextFrame)
        {
            takeScreenshotOnNextFrame = false;
            RenderTexture renderTexture = mainCamera.targetTexture;

            Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
            renderResult.ReadPixels(rect, 0, 0);

            byte[] byteArray = renderResult.EncodeToPNG();
            File.WriteAllBytes(ScreenshotName(), byteArray);
            print("Saved screenshot!");

            RenderTexture.ReleaseTemporary(renderTexture);
            mainCamera.targetTexture = null;

            Application.OpenURL(ScreenshotName());
        }
    }

    string ScreenshotName()
    {
        return string.Format
            (
                "{0}/Screenshot_{1}x{2}_{3}.png",
                m_selectedPath,
                screenshotWidth,
                screenshotHeight,
                System.DateTime.Now.ToString("yyyy-MM-dd-HHmmss")
            );
    }

    public void ChangeScreenshotPathFolder()
    {
        var paths = StandaloneFileBrowser.OpenFolderPanel("Select Folder", "", true);

        if (paths.Length < 1){ return;}

        m_selectedPath = "";

        foreach (string s in paths)
        {
            m_selectedPath += s;
        }
    }
}
