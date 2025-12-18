using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro; 

public class TutorialManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject tutorialPanel;
    public Button dummyWordButton;   
    public RectTransform handIcon;   
    public RectTransform sidebarTarget; 

    [Header("Visuals")]
    public Vector3 handOffset = new Vector3(50, -50, 0); 
    public float flySpeed = 3f;

    [Header("Settings")]
    public UIManager uiManager;

    private bool isFlying = false; 
    private bool isPhaseTwo = false; 
    private Vector3 originalHandScale;

    void Start()
    {
        originalHandScale = handIcon.localScale;

        if (dummyWordButton != null)
        {
            
            handIcon.position = dummyWordButton.transform.position + handOffset;

           
            dummyWordButton.onClick.RemoveAllListeners();
            dummyWordButton.onClick.AddListener(OnDummyWordClicked);
        }
    }

    void Update()
    {
        // animate hand icon
        if (handIcon != null && handIcon.gameObject.activeSelf)
        {
            float scale = 1.0f + Mathf.Sin(Time.time * 8f) * 0.15f; // 8f : velocity, 0.15f : amplitude
            handIcon.localScale = originalHandScale * scale;

            
            if (!isFlying && !isPhaseTwo)
            {
                handIcon.position = dummyWordButton.transform.position + handOffset;
            }
        }
    }

    
    private void OnDummyWordClicked()
    {
        if (isFlying) return; 

        if (!isPhaseTwo)
        {
            
            Debug.Log("Tutorial: Phase 1 Complete. Flying to sidebar...");
            StartCoroutine(FlyToSidebarSequence());
        }
        else
        {
            
            Debug.Log("Tutorial: Phase 2 Complete. Ending.");
            EndTutorial();
        }
    }

    IEnumerator FlyToSidebarSequence()
    {
        isFlying = true;
        dummyWordButton.interactable = false; 

        float t = 0;
        Vector3 startPos = dummyWordButton.transform.position;
        Vector3 endPos = sidebarTarget.position;

       // phase 1: moving to sidebar
        while (t < 1f)
        {
            t += Time.deltaTime * flySpeed;
            // interpolation
            Vector3 currentPos = Vector3.Lerp(startPos, endPos, t);

            // move button and hand
            dummyWordButton.transform.position = currentPos;

            // hand offset
            handIcon.position = currentPos + handOffset;

            
            dummyWordButton.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.6f, t);

            yield return null;
        }

       
        isFlying = false;
        isPhaseTwo = true; // into phase two
        dummyWordButton.interactable = true; // wait for next click

       
        TMP_Text btnText = dummyWordButton.GetComponentInChildren<TMP_Text>();
        if (btnText != null) btnText.text = "Collect!"; //reminder for collection
    }

    public void EndTutorial()
    {
        tutorialPanel.SetActive(false);
        PlayerPrefs.SetInt("HasSeenTutorial", 1);
        PlayerPrefs.Save();

        if (uiManager != null)
        {
            uiManager.CloseTutorial();
        }
    }
}