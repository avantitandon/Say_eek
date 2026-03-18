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


    // VARIABLES //

    // LOGGING //

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
