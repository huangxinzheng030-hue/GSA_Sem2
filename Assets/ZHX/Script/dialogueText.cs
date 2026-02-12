using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueTypewriter : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text dialogueText;

    [Header("Dialogue")]
    [TextArea(2, 4)]
    public string[] lines;

    [Header("Timing")]
    public float charInterval = 0.04f;  
    public float lineHoldTime = 1.0f;   

    int index = 0;
    Coroutine playRoutine;

    void OnEnable()
    {
        if (dialogueText == null || lines == null || lines.Length == 0) return;
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
}
