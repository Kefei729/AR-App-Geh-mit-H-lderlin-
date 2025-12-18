using UnityEngine;
using Niantic.Lightship.AR.ObjectDetection;
using UnityEngine.XR.ARFoundation;

public class UIManager : MonoBehaviour
{
    [Header("1. UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject objectDetectionPanel; // The AR View
    public GameObject galleryPanel;
    public GameObject helpPanel;
    public GameObject ARHelpPanel;
    public GameObject PoemDisplayPanel;

    [Header("Tutorial Settings")]
    // Drag your "DynamicTutorialPanel" here
    public GameObject tutorialPanel;
    private const string TUTORIAL_KEY = "HasSeenTutorial"; // Key to save progress

    [Header("2. AR System Control")]
    // The script that handles Lightship logic
    public ObjectDetectionSample objectDetectionSample;
    // The AR Manager component
    public ARObjectDetectionManager arObjectDetectionManager;
    // The AR Camera Background (we disable this in menus to save battery)
    public ARCameraBackground arCameraBackground;
    public ARSession arSession;

    [Header("3. Logic Managers")]
    public PoetryInteractionManager poetryInteractionManager;
    public PhotoManager photoManager;

    // --- INTERNAL STATE ---
    private bool wasInARMode = false;

    void Start()
    {
        // Ensure panels are set correctly on app start
        if (objectDetectionPanel != null) objectDetectionPanel.SetActive(false);
        if (tutorialPanel != null) tutorialPanel.SetActive(false);

        // Start in the Main Menu
        ShowMainMenu();

        if (poetryInteractionManager != null)
        {
            poetryInteractionManager.StopListening();
        }
    }

    // =========================================================
    //               PUBLIC BUTTON FUNCTIONS
    // =========================================================

    // Button: "Start Experience" in Main Menu
    public void StartObjectDetection()
    {
        ShowObjectDetection(isReturningFromGallery: false);
    }

    // Button: "Back" or "Home"
    public void GoToMainMenu()
    {
        ShowMainMenu();
    }

    // Button: Camera Icon
    public void OnCaptureButtonClick()
    {
        if (photoManager != null)
        {
            photoManager.CaptureAndSaveInternal();
        }
        else
        {
            Debug.LogError("PhotoManager is not assigned in UIManager!");
        }
    }

    // Called by TutorialManager when the animation finishes
    public void CloseTutorial()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);

            // Mark as seen so it doesn't show again (optional)
            PlayerPrefs.SetInt(TUTORIAL_KEY, 1);
            PlayerPrefs.Save();

            // NOW start the real AR word detection
            if (poetryInteractionManager != null)
            {
                poetryInteractionManager.StartListening();
            }
        }
    }

    // Button: Gallery Icon
    public void ToggleGallery()
    {
        if (galleryPanel != null && galleryPanel.activeSelf)
        {
            // CLOSING GALLERY
            if (wasInARMode)
            {
                ShowObjectDetection(isReturningFromGallery: true);
            }
            else
            {
                ShowMainMenu();
            }
        }
        else
        {
            // OPENING GALLERY
            if (objectDetectionPanel != null && objectDetectionPanel.activeSelf)
            {
                wasInARMode = true;
            }
            else
            {
                wasInARMode = false;
            }

            ShowGallery();
        }
    }

    public void ToggleHelp()
    {
        if (helpPanel != null && helpPanel.activeSelf) ShowMainMenu();
        else ShowHelp();
    }

    public void ToggleARHelp()
    {
        if (ARHelpPanel != null && ARHelpPanel.activeSelf)
        {
            ShowObjectDetection(isReturningFromGallery: true);
        }
        else
        {
            ShowARHelp();
        }
    }


    // =========================================================
    //               PRIVATE STATE FUNCTIONS
    // =========================================================

    private void ShowMainMenu()
    {
        Debug.Log("Switching to: Main Menu");
        wasInARMode = false;

        if (mainMenuPanel) mainMenuPanel.SetActive(true);
        if (objectDetectionPanel) objectDetectionPanel.SetActive(false);
        if (galleryPanel) galleryPanel.SetActive(false);
        if (helpPanel) helpPanel.SetActive(false);
        if (ARHelpPanel) ARHelpPanel.SetActive(false);
        if (PoemDisplayPanel) PoemDisplayPanel.SetActive(false);
        if (tutorialPanel) tutorialPanel.SetActive(false);

        if (arObjectDetectionManager != null) arObjectDetectionManager.enabled = false;
        if (arCameraBackground != null) arCameraBackground.enabled = false;
        if (objectDetectionSample != null) objectDetectionSample.enabled = false;

        if (poetryInteractionManager != null)
        {
            poetryInteractionManager.StopListening();
            poetryInteractionManager.ClearAllWords();
        }
    }

    private void ShowObjectDetection(bool isReturningFromGallery)
    {
        Debug.Log($"Switching to: Object Detection (Returning: {isReturningFromGallery})");

        if (objectDetectionPanel == null) return;

        wasInARMode = true;

        // 1. Reset Session ONLY if starting fresh
        if (!isReturningFromGallery)
        {
            arSession.Reset();
            objectDetectionSample.Initialize(arObjectDetectionManager);
        }

        // 2. UI States
        mainMenuPanel.SetActive(false);
        galleryPanel.SetActive(false);
        helpPanel.SetActive(false);
        ARHelpPanel.SetActive(false);
        PoemDisplayPanel.SetActive(false);

        objectDetectionPanel.SetActive(true); // Activate AR Panel

        // 3. Enable AR Systems
        if (arObjectDetectionManager != null) arObjectDetectionManager.enabled = true;
        if (arCameraBackground != null) arCameraBackground.enabled = true;
        if (objectDetectionSample != null) objectDetectionSample.enabled = true;

        // 4. Logic & Tutorial
        if (!isReturningFromGallery)
        {
            // Case A: Starting fresh -> Check if we need Tutorial
            // Change "!PlayerPrefs.HasKey..." to "true" if you want to test it every time
            if (tutorialPanel != null && !PlayerPrefs.HasKey(TUTORIAL_KEY))
            {
                tutorialPanel.SetActive(true);
                // We do NOT start poetry listening yet. Waiting for Tutorial to finish.
            }
            else
            {
                // Fallback: Start immediately if no tutorial needed
                if (poetryInteractionManager != null) poetryInteractionManager.StartListening();
            }
        }
        else
        {
            // Case B: Returning from Gallery -> Resume immediately
            if (poetryInteractionManager != null) poetryInteractionManager.StartListening();
            if (tutorialPanel != null) tutorialPanel.SetActive(false);
        }
    }

    private void ShowGallery()
    {
        Debug.Log("Switching to: Gallery");

        if (mainMenuPanel) mainMenuPanel.SetActive(false);
        if (objectDetectionPanel) objectDetectionPanel.SetActive(false);
        if (galleryPanel) galleryPanel.SetActive(true);
        if (helpPanel) helpPanel.SetActive(false);
        if (ARHelpPanel) ARHelpPanel.SetActive(false);
        if (PoemDisplayPanel) PoemDisplayPanel.SetActive(false);
        if (tutorialPanel) tutorialPanel.SetActive(false);

        // Pause AR
        if (arCameraBackground != null) arCameraBackground.enabled = false;
        if (objectDetectionSample != null) objectDetectionSample.enabled = false;
        if (poetryInteractionManager != null) poetryInteractionManager.StopListening();

        if (photoManager != null) photoManager.RefreshGalleryUI();
    }

    private void ShowHelp()
    {
        Debug.Log("Switching to: Help");
        mainMenuPanel.SetActive(false);
        objectDetectionPanel.SetActive(false);
        galleryPanel.SetActive(false);
        helpPanel.SetActive(true);
        // ... (Disable AR as above)
    }

    private void ShowARHelp()
    {
        Debug.Log("Switching to: AR Help");
        if (objectDetectionPanel) objectDetectionPanel.SetActive(false);
        if (ARHelpPanel) ARHelpPanel.SetActive(true);
    }
}