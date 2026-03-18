using System.Collections;
using TMPro;
using UnityEngine;
// I hate CA
public class BossTextController : MonoBehaviour
{
    //Moved everything about the hUD STATE manager for cleaner code? 
    private const int T1_THRESHOLD = 50;
    private const int T2_THRESHOLD = 100;
    private const int T3_THRESHOLD = 200;

    // The TMP field that displays the current dialogue.
    [SerializeField] private TMP_Text bossTextMessage;

    // The panel that slides in and out, plus the timing and distance
    [SerializeField] private RectTransform dialoguePanel;
    [SerializeField] private float slideDuration = 0.2f;
    [SerializeField] private float slideOffsetX = 0f;
    [SerializeField] private float slideOffsetY = 140f;
    [SerializeField] private string lowScoreMessage = "Are you even trying intern? Or just trying to make me mad";
    [SerializeField] private string t1Message = "Better. Keep moving.";
    [SerializeField] private string t2Message = "Now that looks usable.";
    [SerializeField] private string t3Message = "This is going to POP Off on the gram!.";
    private string[] dialogueLines;
    private int Currlineindex;

    // Stores the curr position.
    private Vector2 Vispos;
    private Coroutine animationRoutine;
    private Coroutine autoHideRoutine;

    public bool IsDialogueActive => gameObject.activeSelf;
    public bool IsDialogueComplete => dialogueLines == null || Currlineindex >= dialogueLines.Length;

    void Awake()
    {
        ResolveReferences();
        if (dialoguePanel != null)
        {
            Vispos = dialoguePanel.anchoredPosition;
        }

        // Start fully hidden without playing the animation on load.
        HideImmediate();
    }

    public void BeginDialogue(string[] lines)
    {
        CancelAutoHide();
        ResolveReferences();

        if (lines == null || lines.Length == 0)
        {
            HideImmediate();
            return;
        }

        dialogueLines = lines;
        Currlineindex = 0;

        gameObject.SetActive(true);

        if (dialoguePanel != null)
        {
            // Move the panel below its visible spot before sliding it in.
            dialoguePanel.anchoredPosition = HiddenPosition();
        }

        UpdateDisplayedLine();
        // Slide in only when the dialogue shows up?
        StartSlide(Vispos, false);
    }

    public void AdvanceDialogue()
    {
        if (!IsDialogueActive || dialogueLines == null)
        {
            return;
        }

        Currlineindex += 1;

        if (Currlineindex >= dialogueLines.Length)
        {
            return;
        }

        UpdateDisplayedLine();
    }

    public void Hide()
    {
        CancelAutoHide();

        if (!gameObject.activeSelf)
        {
            HideImmediate();
            return;
        }

        dialogueLines = null;
        Currlineindex = 0;
        // Slide out once when the dialogue finishes,
        StartSlide(HiddenPosition(), true);
    }
    private void HideImmediate()
    {
        if (animationRoutine != null)
        {
            StopCoroutine(animationRoutine);
            animationRoutine = null;
        }

        CancelAutoHide();

        dialogueLines = null;
        Currlineindex = 0;

        if (dialoguePanel != null)
        {
            dialoguePanel.anchoredPosition = HiddenPosition();
        }

        gameObject.SetActive(false);
    }

    public void ShowTemporaryMessage(string line, float durationSeconds)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            HideImmediate();
            return;
        }

        BeginDialogue(new[] { line });

        if (durationSeconds <= 0f)
        {
            return;
        }

        autoHideRoutine = StartCoroutine(AutoHideAfterDelay(durationSeconds));
    }

    public void ShowTemporaryMessageForScore(int score, float durationSeconds)
    {
        ShowTemporaryMessage(GetMessageForScore(score), durationSeconds);
    }

    private void UpdateDisplayedLine()
    {
        if (bossTextMessage == null || dialogueLines == null || Currlineindex >= dialogueLines.Length)
        {
            return;
        }
        bossTextMessage.text = dialogueLines[Currlineindex];
    }

    private void ResolveReferences()
    {
        // If the panel wasn't assigned manually, use the text's parent panel.
  if (bossTextMessage != null && dialoguePanel == null)
        {
            dialoguePanel = bossTextMessage.transform.parent as RectTransform;
        }
    }

    private Vector2 HiddenPosition()
    {
        // same anchored position shifted by the configured hidden offset
        return Vispos + new Vector2(slideOffsetX, -slideOffsetY);
    }

    private void StartSlide(Vector2 target, bool deactivateAfter)
    {
        if (dialoguePanel == null)
        {
            if (deactivateAfter)
            {
                gameObject.SetActive(false);
            }
            return;
        }

        if (animationRoutine != null)
        {
            StopCoroutine(animationRoutine);
        }

        // Use unscaled time so it still works during tutorial lock-> issues with any ui object
        animationRoutine = StartCoroutine(SlideTo(target, deactivateAfter));
    }

    private IEnumerator SlideTo(Vector2 target, bool deactivateAfter)
    {
        if (slideDuration <= 0f)
        {
            dialoguePanel.anchoredPosition = target;
            if (deactivateAfter)
            {
                gameObject.SetActive(false);
            }
            animationRoutine = null;
            yield break;
        }

        Vector2 start = dialoguePanel.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / slideDuration);
            // Hehe lerp. I cannot believe they actually call it that
            dialoguePanel.anchoredPosition = Vector2.Lerp(start, target, t);
            yield return null;
        }

        dialoguePanel.anchoredPosition = target;
        animationRoutine = null;

        if (deactivateAfter)
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator AutoHideAfterDelay(float durationSeconds)
    {
        yield return new WaitForSecondsRealtime(durationSeconds);
        autoHideRoutine = null;
        Hide();
    }

    private void CancelAutoHide()
    {
        if (autoHideRoutine == null)
        {
            return;
        }

        StopCoroutine(autoHideRoutine);
        autoHideRoutine = null;
    }
    // Have score be sent to this 

    private string GetMessageForScore(int score)
    {
        if (score > T3_THRESHOLD)
        {
            return t3Message;
        }

        if (score > T2_THRESHOLD)
        {
            return t2Message;
        }

        if (score > T1_THRESHOLD)
        {
            return t1Message;
        }

        return lowScoreMessage;
    }
}
