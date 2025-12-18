using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;


public class PoetryInteractionManager : MonoBehaviour
{
    [Header("UI Verkn�pfungen")]
    public GameObject wordButtonPrefab;
    public Transform liveWordContainer;
    public Transform sidebarContainer;
    public GameObject poemDisplayPanel;
    public TextMeshProUGUI poemText;


    private List<string> collectedWords = new List<string>();
    private const int MAX_COLLECTED_WORDS = 6;
    private HashSet<string> currentlyDisplayedWords = new HashSet<string>();


    /// <summary>
    /// Startet das Lauschen auf Objekterkennungs-Events.
    /// Wird vom UIManager aufgerufen, wenn die AR-Ansicht aktiviert wird.
    /// </summary>
    public void StartListening()
    {
        Debug.Log("PoetryInteractionManager: Beginnt mit dem Lauschen auf Objekterkennung.");
        ObjectDetectionSample.OnObjectRecognized += HandleObjectRecognized;
    }


    /// <summary>
    /// Beendet das Lauschen auf Objekterkennungs-Events.
    /// Wird vom UIManager aufgerufen, wenn die AR-Ansicht verlassen wird.
    /// </summary>
    public void StopListening()
    {
        Debug.Log("PoetryInteractionManager: Beendet das Lauschen auf Objekterkennung.");
        ObjectDetectionSample.OnObjectRecognized -= HandleObjectRecognized;
    }


    /// <summary>
    /// Setzt alle angezeigten und gesammelten W�rter zur�ck.
    /// Sorgt f�r einen sauberen Zustand beim erneuten Betreten der AR-Ansicht.
    /// </summary>
    public void ClearAllWords()
    {
        foreach (Transform child in liveWordContainer)
        {
            Destroy(child.gameObject);
        }
        currentlyDisplayedWords.Clear();
    }


    /// <summary>
    /// Diese Methode wird ausgef�hrt, wenn das OnObjectRecognized-Event empfangen wird.
    /// </summary>
    private void HandleObjectRecognized(string detectedName)
    {
        // =========================================================================
        // DIES IST DIE ENTSCHEIDENDE TEST-ZEILE, DIE ICH HINZUGEF�GT HABE:
        Debug.Log($"EVENT EMPFANGEN! Wort: '{detectedName}'. Versuche jetzt, den Button zu erstellen.");
        // =========================================================================


        if (!currentlyDisplayedWords.Contains(detectedName))
        {
            // Deine Logik, um immer nur ein Wort anzuzeigen:
            foreach (Transform child in liveWordContainer) { Destroy(child.gameObject); }
            currentlyDisplayedWords.Clear();


            // Sicherheitspr�fung: Stelle sicher, dass die Prefab-Referenz existiert.
            if (wordButtonPrefab == null || liveWordContainer == null)
            {
                Debug.LogError("FEHLER: WordButtonPrefab oder LiveWordContainer sind im Inspector nicht zugewiesen!", this);
                return;
            }


            GameObject wordButtonObj = Instantiate(wordButtonPrefab, liveWordContainer);
            wordButtonObj.GetComponentInChildren<TextMeshProUGUI>().text = detectedName;
            wordButtonObj.GetComponent<Button>().onClick.AddListener(() => CollectWord(detectedName, wordButtonObj));
            currentlyDisplayedWords.Add(detectedName);
        }
    }


    private void CollectWord(string word, GameObject buttonToMove)
    {
        if (collectedWords.Count < MAX_COLLECTED_WORDS && !collectedWords.Contains(word))
        {
            collectedWords.Add(word);
            buttonToMove.transform.SetParent(sidebarContainer, false);
            buttonToMove.GetComponent<Button>().onClick.RemoveAllListeners();
            buttonToMove.GetComponent<Button>().onClick.AddListener(() => ShowPoemFor(word));
        }
        else
        {
            Destroy(buttonToMove);
        }
        currentlyDisplayedWords.Remove(word);
    }


    private void ShowPoemFor(string word)
    {
        Debug.Log("Zeige Gedicht f�r das Wort: " + word);
        // Hier kommt sp�ter die Logik, um das echte Gedicht aus deiner Datenbank zu holen.
        string poem = $"Dies ist ein Platzhalter-Gedicht f�r das Wort '{word}'.";
        poemText.text = poem;
        poemDisplayPanel.SetActive(true);
    }


    public void ClosePoemPanel()
    {
        if (poemDisplayPanel != null)
        {
            poemDisplayPanel.SetActive(false);
        }
    }
}
