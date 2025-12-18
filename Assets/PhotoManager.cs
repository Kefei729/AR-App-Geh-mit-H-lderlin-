using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Linq;

public class PhotoManager : MonoBehaviour
{
    [Header("1. Gallery UI Configuration")]
    public Transform galleryContentRoot;
    public GameObject photoItemPrefab;

    [Header("2. Full Screen Viewer (New!)")]
    
    public GameObject photoViewerPanel;
    
    public Image fullScreenImage;

    [Header("3. Capture Settings")]
    public GameObject[] uiElementsToHide;

    private string folderName = "HoelderlinPhotos";

    public void CaptureAndSaveInternal()
    {
        StartCoroutine(CaptureRoutine());
    }

    IEnumerator CaptureRoutine()
    {
        foreach (var ui in uiElementsToHide) if (ui != null) ui.SetActive(false);
        yield return new WaitForEndOfFrame();

        Texture2D screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenTexture.Apply();

        foreach (var ui in uiElementsToHide) if (ui != null) ui.SetActive(true);

        byte[] bytes = screenTexture.EncodeToJPG(90);
        string fileName = "Poem_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".jpg";
        string filePath = Path.Combine(GetSavePath(), fileName);

        File.WriteAllBytes(filePath, bytes);
        Debug.Log("Saved: " + filePath);
        Destroy(screenTexture);
    }

    //renew
    public void RefreshGalleryUI()
    {
        if (galleryContentRoot == null || photoItemPrefab == null) return;

        foreach (Transform child in galleryContentRoot) Destroy(child.gameObject);

        string path = GetSavePath();
        if (!Directory.Exists(path)) return;

        var filePaths = Directory.GetFiles(path, "*.jpg")
                                 .OrderByDescending(d => new FileInfo(d).CreationTime);

        foreach (string filePath in filePaths)
        {
            Sprite sprite = LoadSpriteFromPath(filePath);
            if (sprite != null)
            {
                GameObject newItem = Instantiate(photoItemPrefab, galleryContentRoot);

                // Image setting
                Image img = newItem.GetComponent<Image>();
                if (img == null) img = newItem.GetComponentInChildren<Image>();
                if (img != null) img.sprite = sprite;

               
                Button btn = newItem.GetComponent<Button>();
                if (btn != null)
                {
                    // when clicked, open full screen viewer
                    btn.onClick.AddListener(() => OpenPhoto(sprite));
                }
            }
        }
    }

    public void OpenPhoto(Sprite sprite)
    {
        if (photoViewerPanel != null && fullScreenImage != null)
        {
            fullScreenImage.sprite = sprite;
            photoViewerPanel.SetActive(true);
        }
    }

    // close full screen viewer
    public void ClosePhoto()
    {
        if (photoViewerPanel != null)
        {
            photoViewerPanel.SetActive(false);
        }
    }

    
    private string GetSavePath()
    {
        string path = Path.Combine(Application.persistentDataPath, folderName);
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        return path;
    }

    private Sprite LoadSpriteFromPath(string filePath)
    {
        if (File.Exists(filePath))
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }
        return null;
    }
}