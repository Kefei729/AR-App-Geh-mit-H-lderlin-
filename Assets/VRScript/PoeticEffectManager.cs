// PoeticEffectManager.cs
using System.Collections;
using UnityEngine;
using TMPro; // We need this for TextMeshPro

public class PoeticEffectManager : MonoBehaviour
{
    public GameObject poetryLinePrefab; // A template for one line of text
    public Transform spawnPoint;        // Where the text starts appearing
    private bool hasStarted = false;

    // The poem for the "Tower"
    private string[] poemLines = {
        "Mauern stehn",
        "sprachlos und kalt,",
        "im Winde",
        "klirren die Fahnen."
    };

    // This public function will be called by our trigger
    public void StartTowerExperience()
    {
        if (hasStarted) return; // Prevent it from starting multiple times

        Debug.Log("TURM-ERLEBNIS GESTARTET!");
        hasStarted = true;
        StartCoroutine(SpawnPoetryLines());
    }

    private IEnumerator SpawnPoetryLines()
    {
        foreach (string line in poemLines)
        {
            // Create a new text object from the prefab
            GameObject newLine = Instantiate(poetryLinePrefab, spawnPoint.position, spawnPoint.rotation);

            // Set the text
            newLine.GetComponent<TextMeshPro>().text = line;

            // Wait for 2 seconds before spawning the next line
            yield return new WaitForSeconds(2f);
        }
    }
}