using UnityEngine;

public class HUDManager : MonoBehaviour
{
    // CONSTANTS //

    private const int T1_THRESHOLD = 50;
    private const int T2_THRESHOLD = 100;
    private const int T3_THRESHOLD = 200;

    // AUDIO //

    // GAME COMPONENTS //

    [SerializeField] private PhotoPreviewController photoPreviewController;
    [SerializeField] private StatusUIController statusUIController;
    [SerializeField] private BossTextController bossTextController;
    [SerializeField] private BossTextController pictureBossTextController;
    [SerializeField] private string pictureBossLowScoreMessage = "Are you even trying intern? Or just trying to make me mad";
    [SerializeField] private string pictureBossT1Message = "Better. Keep moving.";
    [SerializeField] private string pictureBossT2Message = "Now that looks usable.";
    [SerializeField] private string pictureBossT3Message = "This is going to POP Off on the gram!.";
    [SerializeField] private float pictureBossMessageDuration = 1.5f;


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

public void ShowPictureBossText(int score)
{
    if (pictureBossTextController == null)
    {
        return;
    }

    pictureBossTextController.ShowTemporaryMessage(GetPictureBossMessage(score), pictureBossMessageDuration);
}



    public void DisplayPhotoPreview(int score, Texture2D photo)
    {
        photoPreviewController.DisplayPhotoPreview(score, photo);
    }

    public void SetStatusUI (float timeElapsed, int photosLeft)
    {
        statusUIController.SetStatusUI(timeElapsed, photosLeft);
    }

    private string GetPictureBossMessage(int score)
    {
        if (score > T3_THRESHOLD)
        {
            return pictureBossT3Message;
        }

        if (score > T2_THRESHOLD)
        {
            return pictureBossT2Message;
        }

        if (score > T1_THRESHOLD)
        {
            return pictureBossT1Message;
        }

        return pictureBossLowScoreMessage;
    }
}
