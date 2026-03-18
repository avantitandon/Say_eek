using UnityEngine;

public class HUDManager : MonoBehaviour
{
    // CONSTANTS //

    // AUDIO //

    // GAME COMPONENTS //

    [SerializeField] private PhotoPreviewController photoPreviewController;
    [SerializeField] private StatusUIController statusUIController;
    [SerializeField] private StickerOverlayController stickerOverlayController;

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
