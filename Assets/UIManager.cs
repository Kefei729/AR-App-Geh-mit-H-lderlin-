using UnityEngine;
using Niantic.Lightship.AR.ObjectDetection;
using UnityEngine.XR.ARFoundation;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject objectDetectionPanel;
    public GameObject galleryPanel;
    public GameObject helpPanel;
    public GameObject ARHelpPanel;
    public GameObject PoemDisplayPanel;

    [Header("AR System Control")]
    // Die Referenz zu deinem Skript, das die Logik steuert.
    public ObjectDetectionSample objectDetectionSample; // ACHTUNG: Der Name wurde zur Eindeutigkeit angepasst.

    // Die Referenz zum AR Manager, den wir "injizieren" müssen.
    public ARObjectDetectionManager arObjectDetectionManager;

    // Die Referenz zur Kamera, um den Hintergrund zu steuern.
    public ARCameraBackground arCameraBackground;
    public ARSession arSession;

    [Header("Logik-Manager")]
    public PoetryInteractionManager poetryInteractionManager;

    void Start()
    {
        // Stelle sicher, dass das objectDetectionPanel am Anfang deaktiviert ist,
        // damit sein 'Awake' oder 'OnEnable' nicht zu früh ausgelöst wird.
        if (objectDetectionPanel != null)
            objectDetectionPanel.SetActive(false);

        // Starte im Hauptmenü. Diese Methode deaktiviert auch alle AR-Systeme.
        ShowMainMenu();

        if (poetryInteractionManager != null)
        {
            poetryInteractionManager.StopListening();
        }
    }

    // --- ÖFFENTLICHE FUNKTIONEN, DIE VON DEN BUTTONS AUFGERUFEN WERDEN ---

    public void StartObjectDetection()
    {
        ShowObjectDetection();
    }

    public void GoToMainMenu()
    {
        ShowMainMenu();
    }

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

    public void ToggleARHelp()
    {
        if (ARHelpPanel != null && ARHelpPanel.activeSelf)
        {
            // Wenn die Hilfe geschlossen wird, zurück zum AR-Modus
            ShowObjectDetection();
        }
        else
        {
            ShowARHelp();
        }
    }


    // --- PRIVATE HILFSFUNKTIONEN, DIE DEN ZUSTAND DER APP ÄNDERN ---

    private void ShowMainMenu()
    {
        Debug.Log("Zeige Hauptmenü. Stoppe AR-Kamera und Detektion.");

        mainMenuPanel.SetActive(true);
        objectDetectionPanel.SetActive(false);
        galleryPanel.SetActive(false);
        helpPanel.SetActive(false);
        ARHelpPanel.SetActive(false);
        PoemDisplayPanel.SetActive(false);

        if (arObjectDetectionManager != null) arObjectDetectionManager.enabled = false;
        if (arCameraBackground != null) arCameraBackground.enabled = false;

        // Deaktiviere das Skript, um sicherzugehen, dass es keine Updates mehr verarbeitet.
        if (objectDetectionSample != null)
        {
            objectDetectionSample.enabled = false;
        }

        if (poetryInteractionManager != null)
        {
            poetryInteractionManager.StopListening();
            poetryInteractionManager.ClearAllWords();
        }
    }

    /// <summary>
    /// Aktiviert den Objekterkennungs-Modus und startet die AR-Funktionen.
    /// HIER IST DIE WICHTIGSTE ÄNDERUNG!
    /// </summary>
    private void ShowObjectDetection()
    {
        // --- NEU: Sicherheitsprüfung ---
        // Prüfe, ob alle wichtigen Komponenten im Inspector zugewiesen sind.
        if (objectDetectionPanel == null || objectDetectionSample == null || arObjectDetectionManager == null)
        {
            Debug.LogError("UIManager FEHLER: Eines der wichtigen Felder (Panel, Sample-Skript oder AR-Manager) ist im Inspector nicht zugewiesen!", this);
            return; // Beende die Methode, um Fehler zu verhindern.
        }

        Debug.Log("Zeige Objekterkennung. Initialisiere und starte AR-Systeme.");

        arSession.Reset();
        // --- NEU: Die korrekte Reihenfolge ---
        // 1. Initialisiere das Skript mit der benötigten Manager-Referenz.
        objectDetectionSample.Initialize(arObjectDetectionManager);

        // 2. Schalte alle anderen Panels aus.
        mainMenuPanel.SetActive(false);
        galleryPanel.SetActive(false);
        helpPanel.SetActive(false);
        ARHelpPanel.SetActive(false);
        PoemDisplayPanel.SetActive(false);

        // 3. ERST JETZT aktiviere das AR-Panel und die Kamera.
        // Das Aktivieren des GameObjects wird automatisch die 'OnEnable'-Methode
        // im 'objectDetectionSample'-Skript aufrufen, welches jetzt vorbereitet ist.
        if (arObjectDetectionManager != null) arObjectDetectionManager.enabled = true;
        objectDetectionPanel.SetActive(true);
        if (arCameraBackground != null) arCameraBackground.enabled = true;

        if (poetryInteractionManager != null)
        {
            poetryInteractionManager.StartListening();
        }
    }

    private void ShowGallery()
    {
        Debug.Log("Zeige Galerie. Stoppe AR-Kamera und Detektion.");

        mainMenuPanel.SetActive(false);
        objectDetectionPanel.SetActive(false);
        galleryPanel.SetActive(true);
        helpPanel.SetActive(false);
        ARHelpPanel.SetActive(false);
        PoemDisplayPanel.SetActive(false);

        if (arCameraBackground != null) arCameraBackground.enabled = false;
        if (objectDetectionSample != null) objectDetectionSample.enabled = false;
        if (poetryInteractionManager != null) poetryInteractionManager.StopListening();
    }

    private void ShowHelp()
    {
        Debug.Log("Zeige Hilfe. Stoppe AR-Kamera und Detektion.");

        mainMenuPanel.SetActive(false);
        objectDetectionPanel.SetActive(false);
        galleryPanel.SetActive(false);
        helpPanel.SetActive(true);
        ARHelpPanel.SetActive(false);
        PoemDisplayPanel.SetActive(false);

        if (arCameraBackground != null) arCameraBackground.enabled = false;
        if (objectDetectionSample != null) objectDetectionSample.enabled = false;
        if (poetryInteractionManager != null) poetryInteractionManager.StopListening();
    }

    private void ShowARHelp()
    {
        Debug.Log("Zeige AR-Hilfe. Stoppe AR-Kamera und Detektion.");

        // Deaktiviere das AR-Panel, um nur die Hilfe anzuzeigen
        objectDetectionPanel.SetActive(false);
        ARHelpPanel.SetActive(true);

        if (arCameraBackground != null) arCameraBackground.enabled = false;
        if (objectDetectionSample != null) objectDetectionSample.enabled = false;
        if (poetryInteractionManager != null) poetryInteractionManager.StopListening();
    }
}