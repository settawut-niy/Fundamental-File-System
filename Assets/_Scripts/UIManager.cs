using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class UIManager : MonoBehaviour
{
    [SerializeField] Image panel_ImagePreview;
    [SerializeField] Button button_ShowPreviewPanel;
    [SerializeField] Button button_ClosPreviewPanel;
    [SerializeField] RawImage rawImage_Preview;
    [SerializeField] Texture2D defaultImage;

    Texture2D latestImage;

    public void ShowPreviewPanel(bool isShow)
    {
        if (isShow)
        {
            SelectLatestImageInFolder();
            panel_ImagePreview.gameObject.SetActive(true);
            button_ShowPreviewPanel.gameObject.SetActive(false);
        }
        else
        {
            panel_ImagePreview.gameObject.SetActive(false);
            button_ShowPreviewPanel.gameObject.SetActive(true);
        }
    }

    void SelectLatestImageInFolder()
    {
        DirectoryInfo info = new DirectoryInfo(ScreenshotManager.instance.SelectedPath);
        FileInfo[] files = info.GetFiles().OrderBy(p => p.LastAccessTime).ToArray();

        if (files.Length > 0)
        {
            GeneratePreviewImage(files[files.Length - 1].ToString());
        }
        else
        {
            rawImage_Preview.texture = defaultImage;
        }
    }

    void GeneratePreviewImage(string latestImagePath)
    {
        var bytes = File.ReadAllBytes(latestImagePath);
        latestImage = new Texture2D(2048, 2048);
        latestImage.LoadImage(bytes);
        rawImage_Preview.texture = latestImage;
    }
}
