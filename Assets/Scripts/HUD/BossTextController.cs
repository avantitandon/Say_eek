using UnityEngine;
using TMPro;

public class BossTextController : MonoBehaviour
{
    [SerializeField] private TMP_Text bossTextMessage;

// Line and index varable
    private string[] dialogueLines;
    private int currentLineIndex;

// Ok i didn't know you could do this in C# lowkinuenly, i saw this guy do it in a yt video 
// i'm impress
// avoids messy function code
    public bool IsDialogueActive => gameObject.activeSelf;
    public bool IsDialogueComplete => dialogueLines == null || currentLineIndex >= dialogueLines.Length;

    void Start()
    {
        Hide();
    }

// enter dialogue
    public void BeginDialogue(string[] lines) // all dialogue in string array, allows iterating
    {
        if (lines == null || lines.Length == 0)
        {
            Hide();
            return;
        }

        dialogueLines = lines;
        currentLineIndex = 0;
        gameObject.SetActive(true);
        UpdateDisplayedLine();

    }

    public void AdvanceDialogue()
    // walk through dialogue, mapped to tab, will keep updating the text
    {
        if (!IsDialogueActive || dialogueLines == null)
        {
            return;
        }

        currentLineIndex += 1;

        if (currentLineIndex >= dialogueLines.Length)
        {
            return;
        }
        // updates the string

        UpdateDisplayedLine();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        dialogueLines = null;
        currentLineIndex = 0;
    }

    private void UpdateDisplayedLine()
    {
        if (bossTextMessage == null || dialogueLines == null || currentLineIndex >= dialogueLines.Length)
        {
            // end 
            return;
        }

// set the current text to the index 
        bossTextMessage.text = dialogueLines[currentLineIndex];
    }
}
