using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueTypewriter : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text dialogueText;
    public GameObject dialogueRoot;
    public CanvasGroup dialogueCanvasGroup;
    public CanvasGroup fadeOverlay;

    [Header("Dialogue")]
    [TextArea(2, 4)]
    public string[] lines;

    [Header("Timing")]
    public float charInterval = 0.04f;
    public float lineHoldTime = 1.0f;

    [Header("Camera Switch")]
    public Camera mainCam;
    public Camera dialogueCam;
    public float switchDelayAfterLastLine = 2.0f;

    [Header("Transition")]
    public float uiFadeOutTime = 0.35f;
    public float fadeToBlackTime = 0.35f;
    public float fadeFromBlackTime = 0.35f;

    [Header("Choice UI")]
    public ContractChoiceUI contractChoiceUI;
    public float showChoiceDelay = 0.0f;

    int index = 0;
    Coroutine playRoutine;

    void OnEnable()
    {
        if (dialogueText == null || lines == null || lines.Length == 0) return;

        if (dialogueRoot != null) dialogueRoot.SetActive(true);

        if (dialogueCanvasGroup != null)
        {
            dialogueCanvasGroup.alpha = 1f;
            dialogueCanvasGroup.interactable = true;
            dialogueCanvasGroup.blocksRaycasts = true;
        }

        if (fadeOverlay != null)
        {
            fadeOverlay.alpha = 0f;
            fadeOverlay.interactable = false;
            fadeOverlay.blocksRaycasts = false;
        }

        index = 0;

        if (playRoutine != null) StopCoroutine(playRoutine);
        playRoutine = StartCoroutine(PlayAll());
    }

    IEnumerator PlayAll()
    {
        while (index < lines.Length)
        {
            yield return StartCoroutine(TypeLine(lines[index]));
            yield return new WaitForSeconds(lineHoldTime);
            index++;
        }

        yield return new WaitForSeconds(switchDelayAfterLastLine);

        yield return StartCoroutine(TransitionToDialogueCam());
    }

    IEnumerator TypeLine(string line)
    {
        dialogueText.text = "";

        for (int i = 0; i < line.Length; i++)
        {
            dialogueText.text += line[i];
            yield return new WaitForSeconds(charInterval);
        }
    }

    IEnumerator TransitionToDialogueCam()
    {
        // Clear text + fade out dialogue UI
        if (dialogueText != null) dialogueText.text = "";

        if (dialogueCanvasGroup != null)
            yield return StartCoroutine(FadeCanvasGroup(dialogueCanvasGroup, 1f, 0f, uiFadeOutTime));

        if (dialogueRoot != null) dialogueRoot.SetActive(false);

        // Fade to black
        if (fadeOverlay != null)
            yield return StartCoroutine(FadeCanvasGroup(fadeOverlay, 0f, 1f, fadeToBlackTime));

        // Switch cameras while black
        if (mainCam != null) mainCam.gameObject.SetActive(false);
        if (dialogueCam != null) dialogueCam.gameObject.SetActive(true);

        // Fade from black
        if (fadeOverlay != null)
            yield return StartCoroutine(FadeCanvasGroup(fadeOverlay, 1f, 0f, fadeFromBlackTime));

        // Show choice UI AFTER the new camera is visible
        if (showChoiceDelay > 0f) yield return new WaitForSeconds(showChoiceDelay);
        if (contractChoiceUI != null) contractChoiceUI.Show();
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
