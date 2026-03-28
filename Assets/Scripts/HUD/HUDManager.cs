using UnityEngine;

public class HUDManager : MonoBehaviour
{
    // CONSTANTS //

    // AUDIO //

    // GAME COMPONENTS //

    [SerializeField] private PhotoPreviewController photoPreviewController;
    [SerializeField] private StatusUIController statusUIController;
    [SerializeField] private StickerOverlayController stickerOverlayController;
    [SerializeField] private BossTextController bossTextController;
    [SerializeField] private BossTextController pictureBossTextController;
    [SerializeField] private GameObject incomingCallPrompt;
    [SerializeField] private UIRingShake incomingCallPromptShake;
    [SerializeField] private float pictureBossMessageDuration = 1.5f;

    // to distinguish between picturebosstext and tutorial
    [SerializeField] private bool pictureBossTextEnabled = true;


    // VARIABLES //

    // LOGGING //

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (incomingCallPrompt != null)
        {
            incomingCallPrompt.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // messy for now, thinking of ways to condense this in a seperate script? just so hud sta

    // hud state manager has one begin boss dialogue

public void BeginBossDialogue(string[] lines)
{
    bossTextController.BeginDialogue(lines);
}

public void ShowIncomingCallPrompt()
{
    if (incomingCallPrompt != null)
    {
        incomingCallPrompt.SetActive(true);
    }

    if (incomingCallPromptShake != null)
    {
        incomingCallPromptShake.gameObject.SetActive(true);
    }

    if (incomingCallPromptShake != null)
    {
        incomingCallPromptShake.SetRinging(true);
    }
}

public void HideIncomingCallPrompt()
{
    if (incomingCallPromptShake != null)
    {
        incomingCallPromptShake.SetRinging(false);
        incomingCallPromptShake.gameObject.SetActive(false);
    }

    if (incomingCallPrompt != null)
    {
        incomingCallPrompt.SetActive(false);
    }
}

public void AdvanceBossDialogue()
{
    bossTextController.AdvanceDialogue();
}

public bool IsBossDialogueComplete()
{
    return bossTextController.IsDialogueComplete;
}

public void HideBossText()
{
    bossTextController.Hide();
}

// switch to text

public void ShowTemporaryBossText(string line, float durationSeconds)
{
    bossTextController.ShowTemporaryMessage(line, durationSeconds);
}

public void ShowPictureBossText(int score)
{
    if (!pictureBossTextEnabled || pictureBossTextController == null)
    {
        return;
    }

    pictureBossTextController.ShowTemporaryMessageForScore(score, pictureBossMessageDuration);
}
// forgot to switch to boss text this was a pain in the 

public void ShowCustomPictureBossText(string line, float durationSeconds)
{
    if (pictureBossTextController == null)
    {
        return;
    }

    pictureBossTextController.ShowTemporaryMessage(line, durationSeconds);
}

public void HidePictureBossText()
{
    if (pictureBossTextController == null)
    {
        return;
    }

    pictureBossTextController.Hide();
}

// function for bosstext

public void SetPictureBossTextEnabled(bool isEnabled)
{
    pictureBossTextEnabled = isEnabled;

    if (!isEnabled && pictureBossTextController != null)
    {
        pictureBossTextController.Hide();
    }
}



    public void DisplayPhotoPreview(int score, Texture2D photo)
    {
    photoPreviewController.DisplayPhotoPreview(score, photo);
    stickerOverlayController.DisplayStickerOverlay(score);
    }

    public void SetStatusUI (float timeElapsed, int photosLeft)
    {
        statusUIController.SetStatusUI(timeElapsed, photosLeft);
    }
}
