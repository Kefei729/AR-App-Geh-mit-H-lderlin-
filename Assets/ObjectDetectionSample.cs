using System;
using Niantic.Lightship.AR.ObjectDetection;
using UnityEngine;

public class ObjectDetectionSample : MonoBehaviour
{
    // Diese Variable wird jetzt NUR noch durch die Initialize-Methode gesetzt.
    private ARObjectDetectionManager _objectDetectionManager;

    public static event Action<string> OnObjectRecognized;

    // Wir brauchen den Canvas hier nicht mehr, das kann das UI-Skript selbst verwalten.

    // Die Initialize-Methode ist der einzige Weg, wie dieses Skript seinen Manager bekommt.
    public void Initialize(ARObjectDetectionManager manager)
    {
        if (manager == null)
        {
            Debug.LogError("Der übergebene ARObjectDetectionManager ist null!", this);
            return;
        }
        Debug.Log("ObjectDetectionSample wurde erfolgreich initialisiert.", this);
        _objectDetectionManager = manager;
    }

    // OnEnable wird aufgerufen, wenn der UIManager das Panel aktiviert.
    private void OnEnable()
    {
        if (_objectDetectionManager == null)
        {
            Debug.LogError("ARObjectDetectionManager ist NICHT zugewiesen! UIManager hat Initialize() nicht korrekt aufgerufen.", this);
            return;
        }

        Debug.Log("ObjectDetectionSample: Skript aktiviert. Starte Lauschen auf Metadaten.");
        // WICHTIG: Wir aktivieren nicht mehr den Manager selbst, sondern lauschen nur auf seine Events.
        // Der UIManager ist dafür verantwortlich, ihn zu aktivieren.
        _objectDetectionManager.MetadataInitialized += OnMetadataInitialized;
    }

    // OnDisable wird aufgerufen, wenn der UIManager das Panel deaktiviert.
    private void OnDisable()
    {
        if (_objectDetectionManager == null) return;

        Debug.Log("ObjectDetectionSample: Skript deaktiviert. Stoppe Lauschen.");
        // Wichtig: Immer die Events abbestellen, die man abonniert hat.
        _objectDetectionManager.MetadataInitialized -= OnMetadataInitialized;
        _objectDetectionManager.ObjectDetectionsUpdated -= OnObjectDetectionsUpdated;
    }

    private void OnMetadataInitialized(ARObjectDetectionModelEventArgs obj)
    {
        Debug.Log("Metadaten initialisiert. Lausche jetzt auf Objekterkennungs-Updates.");
        _objectDetectionManager.ObjectDetectionsUpdated += OnObjectDetectionsUpdated;
    }

    private void OnObjectDetectionsUpdated(ARObjectDetectionsUpdatedEventArgs obj)
    {
        if (obj.Results == null || obj.Results.Count == 0)
            return;

        foreach (var detection in obj.Results)
        {
            var categorizations = detection.GetConfidentCategorizations(0.5f);
            if (categorizations.Count > 0)
            {
                categorizations.Sort((a, b) => b.Confidence.CompareTo(a.Confidence));
                var bestCategory = categorizations[0];

                Debug.Log($"Objekt erkannt: '{bestCategory.CategoryName}'. Sende Event...");
                OnObjectRecognized?.Invoke(bestCategory.CategoryName);
            }
        }
    }
}