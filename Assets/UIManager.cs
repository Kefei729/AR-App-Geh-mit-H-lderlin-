using UnityEngine;
using Niantic.Lightship.AR.ObjectDetection; // Wichtig, um ARObjectDetectionManager zu kennen!
using UnityEngine.XR.ARFoundation; // Wichtig, um ARCameraBackground zu kennen!

public class UIManager : MonoBehaviour
{
    // ----- Referenzen zu allen UI-Panels -----
    // Ziehen Sie die entsprechenden GameObjects aus Ihrer Hierarchy in diese Felder im Inspector.
    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject objectDetectionPanel;
    public GameObject galleryPanel;
    public GameObject helpPanel;
    public GameObject ARHelpPanel;

    // ----- Referenzen zu den zu steuernden AR-Komponenten -----
    // Ziehen Sie die "AR Object Detection Manager"-Komponente von Ihrer Main Camera hierher.
    // Ziehen Sie die "AR Camera Background"-Komponente von Ihrer Main Camera hierher.
    [Header("AR System Control")]
    public ARObjectDetectionManager objectDetectionManager;
    public ARCameraBackground arCameraBackground;

    void Start()
    {
        // Beim Start der App das Hauptmenü anzeigen und die AR-Funktionen komplett deaktivieren.
        ShowMainMenu();
    }

    // --- ÖFFENTLICHE FUNKTIONEN, DIE VON DEN BUTTONS AUFGERUFEN WERDEN ---

    // Diese Funktion wird vom "Start"-Button aufgerufen.
    public void StartObjectDetection()
    {
        ShowObjectDetection();
    }

    // Diese Funktion wird von "Zurück"-Buttons aufgerufen.
    public void GoToMainMenu()
    {
        ShowMainMenu();
    }

    // Diese Funktion schaltet das Galerie-Panel an oder aus.
    public void ToggleGallery()
    {
        if (galleryPanel != null && galleryPanel.activeSelf)
        {
            ShowMainMenu();
        }
        else
        {
            ShowGallery();
        }
    }

    // Diese Funktion schaltet das Hilfe-Panel an oder aus.
    public void ToggleHelp()
    {
        if (helpPanel != null && helpPanel.activeSelf)
        {
            ShowMainMenu();
        }
        else
        {
            ShowHelp();
        }
    }

    // Diese Funktion schaltet das AR-Hilfe-Panel an oder aus.
    public void ToggleARHelp()
    {
        if (ARHelpPanel != null && ARHelpPanel.activeSelf)
        {
            ShowMainMenu();
        }
        else
        {
            ShowARHelp();
        }
    }


    // --- PRIVATE HILFSFUNKTIONEN, DIE DEN ZUSTAND DER APP ÄNDERN ---

    // Zeigt das Hauptmenü an und schaltet alle AR-Funktionen aus.
    private void ShowMainMenu()
    {
        Debug.Log("Zeige Hauptmenü. Stoppe AR-Kamera und Detektion.");

        // UI-Panels aktivieren/deaktivieren
        mainMenuPanel.SetActive(true);
        objectDetectionPanel.SetActive(false);
        galleryPanel.SetActive(false);
        helpPanel.SetActive(false);
        ARHelpPanel.SetActive(false);

        // Die AR-Kameraansicht ausblenden
        if (arCameraBackground != null)
        {
            arCameraBackground.enabled = false;
        }

        // Die Objekterkennungs-Logik deaktivieren
        if (objectDetectionManager != null)
        {
            objectDetectionManager.enabled = false;
        }
    }

    // Zeigt den Objekterkennungs-Modus an und startet die AR-Funktionen.
    private void ShowObjectDetection()
    {
        Debug.Log("Zeige Objekterkennung. Starte AR-Kamera und Detektion.");

        // UI-Panels aktivieren/deaktivieren
        mainMenuPanel.SetActive(false);
        objectDetectionPanel.SetActive(true);
        galleryPanel.SetActive(false);
        helpPanel.SetActive(false);
        ARHelpPanel.SetActive(false);

        // Die AR-Kameraansicht einblenden
        if (arCameraBackground != null)
        {
            arCameraBackground.enabled = true;
        }

        // Die Objekterkennungs-Logik aktivieren
        if (objectDetectionManager != null)
        {
            objectDetectionManager.enabled = true;
        }
    }

    // Zeigt die Galerie an.
    private void ShowGallery()
    {
        Debug.Log("Zeige Galerie. Stoppe AR-Kamera und Detektion.");

        mainMenuPanel.SetActive(false);
        objectDetectionPanel.SetActive(false);
        galleryPanel.SetActive(true);
        helpPanel.SetActive(false);
        ARHelpPanel.SetActive(false);

        if (arCameraBackground != null) arCameraBackground.enabled = false;
        if (objectDetectionManager != null) objectDetectionManager.enabled = false;
    }

    // Zeigt die Hilfe an.
    private void ShowHelp()
    {
        Debug.Log("Zeige Hilfe. Stoppe AR-Kamera und Detektion.");

        mainMenuPanel.SetActive(false);
        objectDetectionPanel.SetActive(false);
        galleryPanel.SetActive(false);
        helpPanel.SetActive(true);
        ARHelpPanel.SetActive(false);

        if (arCameraBackground != null) arCameraBackground.enabled = false;
        if (objectDetectionManager != null) objectDetectionManager.enabled = false;
    }

    // Zeigt die AR-Hilfe an.
    private void ShowARHelp()
    {
        Debug.Log("Zeige AR-Hilfe. Stoppe AR-Kamera und Detektion.");

        mainMenuPanel.SetActive(false);
        objectDetectionPanel.SetActive(false);
        galleryPanel.SetActive(false);
        helpPanel.SetActive(false);
        ARHelpPanel.SetActive(true);

        if (arCameraBackground != null) arCameraBackground.enabled = false;
        if (objectDetectionManager != null) objectDetectionManager.enabled = false;
    }
}