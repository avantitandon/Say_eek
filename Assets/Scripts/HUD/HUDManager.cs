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
        ResolveIncomingCallPromptReferences();

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
    ResolveIncomingCallPromptReferences();

    if (incomingCallPrompt == null)
    {
        return;
    }

    incomingCallPrompt.SetActive(true);

    if (incomingCallPromptShake != null)
    {
        incomingCallPromptShake.SetRinging(true);
    }
}

public void HideIncomingCallPrompt()
{
    ResolveIncomingCallPromptReferences();

    if (incomingCallPromptShake != null)
    {
        incomingCallPromptShake.SetRinging(false);
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

public void ShowPictureBossText(int score)
{
    if (!pictureBossTextEnabled || pictureBossTextController == null)
    {
        return;
    }

    pictureBossTextController.ShowTemporaryMessageForScore(score, pictureBossMessageDuration);
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

    private void ResolveIncomingCallPromptReferences()
    {
        if (incomingCallPrompt != null && incomingCallPromptShake != null)
        {
            return;
        }

        foreach (UIRingShake ringShake in Resources.FindObjectsOfTypeAll<UIRingShake>())
        {
            if (ringShake == null || ringShake.gameObject.scene.rootCount == 0)
            {
                continue;
            }

            if (incomingCallPromptShake == null)
            {
                incomingCallPromptShake = ringShake;
            }

            if (incomingCallPrompt == null)
            {
                incomingCallPrompt = ringShake.transform.root.gameObject;
            }

            if (incomingCallPrompt != null && incomingCallPromptShake != null)
            {
                return;
            }
        }
    }
}
