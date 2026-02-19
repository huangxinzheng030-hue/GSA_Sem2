using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SignatureChoiceFlow : MonoBehaviour
{
    [Header("UI")]
    public GameObject buttonsRoot;          // container of the two buttons
    public Button signButton;
    public Button declineButton;

    [Header("Signature Image (UI)")]
    public Image signatureImage;            // the Image that shows the signature
    public Sprite[] signatureFrames;        // drag 31 sprites here in order
    public float fps = 24f;
    public bool hideSignatureOnStart = true;

    [Header("After Sign")]
    public float holdAfterSignature = 0.8f;

    [Header("Fade & Scene")]
    public CanvasGroup fadeOverlay;
    public float fadeToBlackTime = 0.6f;
    public string nextSceneName = "HeistTutorial";

    [Header("Decline")]
    public float declineHoldTime = 0.6f;
    public string declineSceneName = "StartMenu";

    Coroutine routine;

    void Awake()
    {
        if (signButton != null) signButton.onClick.AddListener(OnSign);
        if (declineButton != null) declineButton.onClick.AddListener(OnDecline);
    }

    void Start()
    {
        if (signatureImage != null)
        {
            signatureImage.color = new Color(1f, 1f, 1f, 1f);
            if (hideSignatureOnStart)
            {
                signatureImage.enabled = false;
                signatureImage.sprite = null;
            }
            else
            {
                signatureImage.enabled = true;
            }
        }

        if (fadeOverlay != null)
        {
            fadeOverlay.alpha = 0f;
            fadeOverlay.blocksRaycasts = false;
            fadeOverlay.interactable = false;
        }
    }

    public void ShowButtons()
    {
        if (buttonsRoot != null) buttonsRoot.SetActive(true);
        if (signButton != null) signButton.interactable = true;
        if (declineButton != null) declineButton.interactable = true;
    }

    void HideButtons()
    {
        if (signButton != null) signButton.interactable = false;
        if (declineButton != null) declineButton.interactable = false;
        if (buttonsRoot != null) buttonsRoot.SetActive(false);
    }

    public void OnSign()
    {
        if (routine != null) return;
        routine = StartCoroutine(SignFlow());
    }

    IEnumerator SignFlow()
    {
        HideButtons();

        yield return PlaySignatureOnce();

        if (holdAfterSignature > 0f)
            yield return new WaitForSeconds(holdAfterSignature);

        yield return FadeToBlackAndLoad(nextSceneName);
    }

    public void OnDecline()
    {
        if (routine != null) return;
        routine = StartCoroutine(DeclineFlow());
    }

    IEnumerator DeclineFlow()
    {
        HideButtons();

        if (declineHoldTime > 0f)
            yield return new WaitForSeconds(declineHoldTime);

        yield return FadeToBlackAndLoad(declineSceneName);
    }

    IEnumerator PlaySignatureOnce()
    {
        if (signatureImage == null || signatureFrames == null || signatureFrames.Length == 0)
            yield break;

        signatureImage.enabled = true;

        float frameTime = (fps <= 0f) ? 0.04f : (1f / fps);

        for (int i = 0; i < signatureFrames.Length; i++)
        {
            signatureImage.sprite = signatureFrames[i];
            yield return new WaitForSeconds(frameTime);
        }
    }

    IEnumerator FadeToBlackAndLoad(string sceneName)
    {
        if (fadeOverlay != null)
        {
            fadeOverlay.blocksRaycasts = true;
            fadeOverlay.interactable = false;

            float from = fadeOverlay.alpha;
            float to = 1f;

            float t = 0f;
            while (t < fadeToBlackTime)
            {
                t += Time.deltaTime;
                fadeOverlay.alpha = Mathf.Lerp(from, to, t / fadeToBlackTime);
                yield return null;
            }
            fadeOverlay.alpha = 1f;
        }

        SceneManager.LoadScene(sceneName);
    }
}
