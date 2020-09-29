using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[RequireComponent(typeof(Camera))]
public class ScreenshotManager : MonoBehaviour
{
    Camera mainCamera;
    bool takeScreenshotOnNextFrame;

    [SerializeField] int screenshotWidth = 2048, screenshotHeight = 2048;

    void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            TakeScreenshot(screenshotWidth, screenshotHeight);
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
        }
    }

    string ScreenshotName()
    {
        return string.Format
            (
                "{0}/Screenshot_{1}x{2}_{3}.png",
                Application.dataPath,
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
}
