using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ContractChoiceUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject choicePanel;
    public Button signButton;
    public Button declineButton;

    [Header("Signature")]
    public SignatureSequencePlayer signaturePlayer;
    public GameObject signatureRoot;                 // Optional: assign the SignatureImage (or its parent) here
    public float extraHoldAfterSignature = 1.2f;

    [Header("Fade & Scene")]
    public CanvasGroup fadeOverlay;
    public float fadeToBlackTime = 0.6f;
    public string nextSceneName = "HeistTutorial";

    [Header("Auto Setup")]
    public bool hideOnStart = true;

    Coroutine routine;

    void Awake()
    {
        // Hide choice UI at boot so it won't be visible before you call Show().
        if (hideOnStart)
            Hide();

        // Ensure signature is hidden until sign is pressed.
        if (signatureRoot != null)
            signatureRoot.SetActive(false);
        else if (signaturePlayer != null)
            signaturePlayer.gameObject.SetActive(false);

        // Optional: keep fade overlay hidden at start
        if (fadeOverlay != null)
        {
            fadeOverlay.alpha = 0f;
            fadeOverlay.blocksRaycasts = false;
            fadeOverlay.interactable = false;
        }

        // Optional: wire buttons automatically (you can still wire in Inspector)
        if (signButton != null)
        {
            signButton.onClick.RemoveListener(OnSign);
            signButton.onClick.AddListener(OnSign);
        }

        if (declineButton != null)
        {
            declineButton.onClick.RemoveListener(OnDecline);
            declineButton.onClick.AddListener(OnDecline);
        }
    }

    public void Show()
    {
        if (choicePanel != null) choicePanel.SetActive(true);
        else gameObject.SetActive(true);

        if (signButton != null) signButton.interactable = true;
        if (declineButton != null) declineButton.interactable = true;
    }

    public void ShowAfterDelay(float delay)
    {
        StartCoroutine(ShowDelayRoutine(delay));
    }

    IEnumerator ShowDelayRoutine(float delay)
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);
        Show();
    }

    public void Hide()
    {
        if (choicePanel != null) choicePanel.SetActive(false);
        else gameObject.SetActive(false);
    }

    public void OnSign()
    {
        if (routine != null) return;
        routine = StartCoroutine(SignFlow());
    }

    IEnumerator SignFlow()
    {
        if (signButton != null) signButton.interactable = false;
        if (declineButton != null) declineButton.interactable = false;

        // Hide buttons immediately
        Hide();

        // Show signature and play
        if (signatureRoot != null) signatureRoot.SetActive(true);
        else if (signaturePlayer != null) signaturePlayer.gameObject.SetActive(true);

        if (signaturePlayer != null)
            yield return signaturePlayer.PlayOnceRoutine();

        if (extraHoldAfterSignature > 0f)
            yield return new WaitForSeconds(extraHoldAfterSignature);

        // Fade to black
        if (fadeOverlay != null)
        {
            fadeOverlay.gameObject.SetActive(true);
            fadeOverlay.blocksRaycasts = true;
            fadeOverlay.interactable = false;
            yield return FadeCanvasGroup(fadeOverlay, fadeOverlay.alpha, 1f, fadeToBlackTime);
        }

        SceneManager.LoadScene(nextSceneName);
    }

    public void OnDecline()
    {
        // Implement your decline flow here, e.g. dialogue + return to menu.
        // SceneManager.LoadScene("StartMenu");
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
    {
        if (cg == null) yield break;

        cg.alpha = from;

        if (duration <= 0f)
        {
            cg.alpha = to;
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }

        cg.alpha = to;
    }
}
