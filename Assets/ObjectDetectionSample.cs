using System;
using System.Collections;
using System.Collections.Generic;
using Niantic.Lightship.AR.ObjectDetection;
using UnityEngine;

public class ObjectDetectionSample : MonoBehaviour
{
    [SerializeField] private float _probabilityThreshold = .5f;

    [SerializeField] private ARObjectDetectionManager _objectDetectionManager;
    private bool _isDetectionRunning = false;

    private Color[] colors = new[]
    {
        Color.red,
        Color.blue,
        Color.green,
        Color.yellow,
        Color.magenta,
        Color.cyan,
        Color.white,
        Color.black
    };



    [SerializeField] private DrawRect _drawRect;

    private Canvas _canvas;


    private void Awake()
    {
        _canvas = FindObjectOfType<Canvas>();
    }


    // --- NEUE ÖFFENTLICHE FUNKTIONEN ---
    public void StartDetection()
    {
        if (_isDetectionRunning) return; // Nicht doppelt starten

        Debug.Log("ObjectDetectionSample: Starte die Erkennung.");
        _isDetectionRunning = true;
        _objectDetectionManager.enabled = true;
        _objectDetectionManager.MetadataInitialized += ObjectDetectionManagerOnMetadataInitialized;

        // Manchmal muss man die Events neu abonnieren, falls sie in OnDisable entfernt wurden
        _objectDetectionManager.ObjectDetectionsUpdated += ObjectDetectionManagerOnObjectDetectionsUpdated;
    }

    public void StopDetection()
    {
        if (!_isDetectionRunning) return; // Nicht stoppen, wenn es nicht läuft

        Debug.Log("ObjectDetectionSample: Stoppe die Erkennung.");
        _isDetectionRunning = false;
        _objectDetectionManager.enabled = false;
        _objectDetectionManager.ObjectDetectionsUpdated -= ObjectDetectionManagerOnObjectDetectionsUpdated;
        // MetadataInitialized muss man seltener entfernen, aber zur Sicherheit:
        _objectDetectionManager.MetadataInitialized -= ObjectDetectionManagerOnMetadataInitialized;
    }

    private void OnDisable() // WICHTIG: Stoppt die Erkennung, wenn das GameObject deaktiviert wird
    {
        StopDetection();
    }

    private void OnDestroy()
    {
           StopDetection();
    }

    private void ObjectDetectionManagerOnMetadataInitialized(ARObjectDetectionModelEventArgs obj)
    {
        _objectDetectionManager.ObjectDetectionsUpdated += ObjectDetectionManagerOnObjectDetectionsUpdated;
    }

    private void ObjectDetectionManagerOnObjectDetectionsUpdated(ARObjectDetectionsUpdatedEventArgs obj)
    {
        string resultString = "";
        float _confidence = 0;
        string _name = "";
        var result = obj.Results;

        if (result == null)
            return;

        _drawRect.ClearRects();

        for (int i = 0; i < result.Count; i++)
        {
            var detection = result[i];
            var categorization = detection.GetConfidentCategorizations(.5f);

            if (categorization.Count <= 0)
            {
                break;
            }

            categorization.Sort((a, b) => b.Confidence.CompareTo(a.Confidence));


            var categoryToDisplay = categorization[0];
            _confidence = categoryToDisplay.Confidence;
            _name = categoryToDisplay.CategoryName;

            int h = Mathf.FloorToInt(_canvas.GetComponent<RectTransform>().rect.height);
            int w = Mathf.FloorToInt(_canvas.GetComponent<RectTransform>().rect.width);

            var rect = result[i].CalculateRect(w,h,Screen.orientation);
            resultString = $"{_name} : {_confidence}\n";
            _drawRect.CreateRect(rect, colors[i % colors.Length], resultString);
        }
    }

   
}