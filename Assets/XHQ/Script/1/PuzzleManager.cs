using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class PuzzleManager : MonoBehaviour
{
    public PuzzleRing[] rings;          
    public GameObject ringsContainer;   

    public Animator safeAnimator;       
    public GameObject detailCamera;     
    public float blendWaitTime = 0.5f;  

    public AudioClip winSound;          
    
    private bool hasWon = false;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!hasWon && CheckWinCondition())
        {
            StartCoroutine(WinSequence());
        }
    }

    bool CheckWinCondition()
    {
        if (rings == null || rings.Length == 0) return false;
        foreach (PuzzleRing ring in rings)
        {
            if (ring == null || !ring.IsCorrect()) return false;
        }
        return true;
    }

    IEnumerator WinSequence()
    {
        hasWon = true;

        foreach (PuzzleRing ring in rings)
        {
            if (ring != null) ring.LockRing();
        }

        if (ringsContainer != null)
        {
            ringsContainer.SetActive(false);
        }

        if (detailCamera != null)
        {
            detailCamera.SetActive(false);
        }

        yield return new WaitForSeconds(blendWaitTime);

        if (winSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(winSound);
        }

        if (safeAnimator != null)
        {
            safeAnimator.SetTrigger("OpenSafe");
        }
    }
}