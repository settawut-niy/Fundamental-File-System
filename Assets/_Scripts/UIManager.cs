using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class UIManager : MonoBehaviour
{
    [Header("Standalone Button")]
    [SerializeField] Image panel_ButtonShowPreview;

    [Header("On Image Preview Panel")]
    [SerializeField] Image panel_ImagePreview;
    [SerializeField] Button button_ClosPreviewPanel;
    [SerializeField] Button button_Exit;

    [Header("Select Path Panel")]
    [SerializeField] Text text_Path;
    [SerializeField] Button button_SelectFolder;

    [Header("Refresh Panel")]
    [SerializeField] Toggle toggle_AutoRefresh;
    [SerializeField] Button button_Refresh;
    bool isWaitForAutoRefresh = false;

    [Header("Image File Preview")]
    [SerializeField] Text text_ImageName;
    [SerializeField] RawImage rawImage_Preview;
    [SerializeField] Texture2D defaultImage;
    Texture2D latestImage;

    void Update()
    {
        if (panel_ImagePreview.gameObject.activeSelf)
        {
            AutoRefresh();
            text_Path.text = ScreenshotManager.instance.SelectedPath;
        }
    }

    public void ShowPreviewPanel(bool isShow)
    {
        if (isShow)
        {
            panel_ImagePreview.gameObject.SetActive(true);
            panel_ButtonShowPreview.gameObject.SetActive(false);
        }
        else
        {
            panel_ImagePreview.gameObject.SetActive(false);
            panel_ButtonShowPreview.gameObject.SetActive(true);
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
            GetFileName("");
        }
    }

    void GeneratePreviewImage(string latestImagePath)
    {
        var bytes = File.ReadAllBytes(latestImagePath);
        latestImage = new Texture2D(2048, 2048);
        latestImage.LoadImage(bytes);
        rawImage_Preview.texture = latestImage;

        GetFileName(latestImagePath);
    }

    void GetFileName(string name)
    {
        switch (name)
        {
            case "":
                text_ImageName.text = defaultImage.name;
                break;
            default:
                text_ImageName.text = Path.GetFileName(name);
                break;
        }
    }

    void AutoRefresh()
    {
        if (toggle_AutoRefresh.isOn && !isWaitForAutoRefresh)
        {
            StartCoroutine(WaitForAutoRefresh());
        }
    }

    IEnumerator WaitForAutoRefresh()
    {
        isWaitForAutoRefresh = true;
        yield return new WaitForSeconds(1f);
        SelectLatestImageInFolder();
        isWaitForAutoRefresh = false;
    }

    public void SelectFolderButton()
    {
        ScreenshotManager.instance.ChangeScreenshotPathFolder();
    }

    public void AutoRefreshToggle()
    {
        if (toggle_AutoRefresh.isOn)
        {
            button_Refresh.interactable = false;
        }
        else
        {
            button_Refresh.interactable = true;
        }
    }

    public void RefreshButton()
    {
        SelectLatestImageInFolder();
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
