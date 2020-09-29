using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using SFB;

[RequireComponent(typeof(Camera))]
public class ScreenshotManager : MonoBehaviour
{
    Camera mainCamera;
    bool takeScreenshotOnNextFrame;

    string path;

    [SerializeField] int screenshotWidth = 2048, screenshotHeight = 2048;
    [SerializeField] RawImage output;
    Texture2D texture;

    void Awake()
    {
        mainCamera = GetComponent<Camera>();
        path = Application.dataPath;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            TakeScreenshot(screenshotWidth, screenshotHeight);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangePathToSave();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            ShowImageInFolder();
        }
    }

    void OnPostRender()
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
                path,
                screenshotWidth,
                screenshotHeight,
                System.DateTime.Now.ToString("yyyy-MM-dd-HHmmss")
            );
    }

    void TakeScreenshot(int width, int height)
    {
        mainCamera.targetTexture = RenderTexture.GetTemporary(width, height, 16);
        takeScreenshotOnNextFrame = true;
    }

    void ChangePathToSave()
    {
        var paths = StandaloneFileBrowser.OpenFolderPanel("Select Folder", "", true);

        path = "";

        foreach (string s in paths)
        {
            path += s;
        }

        print("Change a path to " + path);
    }

    void ShowImageInFolder ()
    {
        string[] files = Directory.GetFiles(path);

        string url = files[files.Length-1];

        var bytes = File.ReadAllBytes(url);

        texture = new Texture2D(2048, 2048);
        texture.LoadImage(bytes);

        output.texture = texture;
    }
}
