using System;
using Niantic.Lightship.AR.ObjectDetection;
using UnityEngine;


public class ObjectDetectionSample : MonoBehaviour
{

    [SerializeField] private ARObjectDetectionManager _objectDetectionManager;



    public static event Action<string> OnObjectRecognized;


    // =========================================================
    //  被 UIManager 调用：UIManager.ShowObjectDetection()
    //  objectDetectionSample.Initialize(arObjectDetectionManager);
    // =========================================================
    public void Initialize(ARObjectDetectionManager manager)
    {
        if (manager == null)
        {
            Debug.LogError("[Poetry] Initialize: ARObjectDetectionManager ist NULL!", this);
            return;
        }


        Debug.Log("[Poetry] Initialize aufgerufen. Manager-Referenz gesetzt.", this);
        _objectDetectionManager = manager;
    }


    // =========================================================
    //                  UNITY LIFECYCLE
    // =========================================================
    [Obsolete]
    private void OnEnable()
    {
        if (_objectDetectionManager == null)
        {
            _objectDetectionManager = FindObjectOfType<ARObjectDetectionManager>();
            if (_objectDetectionManager != null)
            {
                Debug.Log("[Poetry] OnEnable: Manager per FindObjectOfType gefunden.", this);
            }
        }


        if (_objectDetectionManager == null)
        {
            Debug.LogError("[Poetry] OnEnable: Kein ARObjectDetectionManager gefunden!", this);
            return;
        }


        Debug.Log("[Poetry] ObjectDetectionSample aktiviert. Abonniere ObjectDetectionsUpdated.");
        _objectDetectionManager.ObjectDetectionsUpdated += OnObjectDetectionsUpdated;
    }


    private void OnDisable()
    {
        if (_objectDetectionManager == null) return;


        Debug.Log("[Poetry] ObjectDetectionSample deaktiviert. Entferne Event-Listener.");
        _objectDetectionManager.ObjectDetectionsUpdated -= OnObjectDetectionsUpdated;
    }


    // =========================================================
    //              DETEKTIONS-UPDATES VOM AR-SYSTEM
    // =========================================================
    private void OnObjectDetectionsUpdated(ARObjectDetectionsUpdatedEventArgs args)
    {
        if (args.Results == null || args.Results.Count == 0)
            return;


        foreach (var detection in args.Results)
        {
            var categorizations = detection.GetConfidentCategorizations(0.5f);
            if (categorizations == null || categorizations.Count == 0)
                continue;


            categorizations.Sort((a, b) => b.Confidence.CompareTo(a.Confidence));
            var bestCategory = categorizations[0];


            Debug.Log($"[Poetry] Objekt erkannt: '{bestCategory.CategoryName}' (Conf {bestCategory.Confidence}) → sende Event");


            OnObjectRecognized?.Invoke(bestCategory.CategoryName);
        }
    }
}
