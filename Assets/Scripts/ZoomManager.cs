using UnityEngine;

public class ZoomManager : MonoBehaviour
{

    // CONSTANTS //

    private const float TGT_PLAYER_ASPECT = 16/9f;
    private const float TGT_CAMERA_ASPECT = 5/4f;
    private const float BASE_FOV = 50f;

    private const float ZOOM_FACTOR = 0.7f;

    private const float FRAME_FACTOR = 13/14f;

    private const float ZOOM_IN_TIME = 0.15f;

    private const float ZOOM_OUT_TIME = 0.20f;

    // AUDIO //
    [SerializeField] private AudioManager audioManager;

    // GAME COMPONENTS //

    [SerializeField] public Camera playerCamera;
    [SerializeField] public Camera photoCamera;

    // VARIABLES //

    private bool zooming;

    private bool _zoomWasHeldLastFrame = false;

    private float _playerFovVel;
    private float _photoFovVel;

    // LOGGING //


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerCamera.fieldOfView = BASE_FOV;
        photoCamera.fieldOfView = BASE_FOV * FRAME_FACTOR;

        zooming = false;
    }

    // Update is called once per frame
    void Update()
    {

        // calculate target zoomed in/out FOV for the player and the photo camera
        // if the view is the proper resolution or wider, unity properly fixes vertical FOV (does not trim top of screen)
        // if the view is thinner/taller, we need to fix horizontal FOV to not trim of the sides of the screen
        // our FOVs are measured in vertical, so we need to do conversions to find out the vertial FOV that fixes the horizontal FOV

        // player / photo camera zoomed in / out FOVs
        float player_in = BASE_FOV * ZOOM_FACTOR;
        float player_out = BASE_FOV;
        float photo_in = BASE_FOV * ZOOM_FACTOR * FRAME_FACTOR;
        float photo_out = BASE_FOV * FRAME_FACTOR;

        // if the screen is thinner or taller, we need to fix the horizontal FOV
        // note TGT_PLAYER_ASPECT is the target minimum ratio we want for the player camera, and playerCamera.aspect is the current actual
        // aspect of the camera, which is the same, taller, or wider.
        if (Screen.width / Screen.height < TGT_PLAYER_ASPECT)
        {
            player_in = Camera.HorizontalToVerticalFieldOfView(Camera.VerticalToHorizontalFieldOfView(player_in, TGT_PLAYER_ASPECT), playerCamera.aspect);
            player_out = Camera.HorizontalToVerticalFieldOfView(Camera.VerticalToHorizontalFieldOfView(player_out, TGT_PLAYER_ASPECT), playerCamera.aspect);
            photo_in = Camera.HorizontalToVerticalFieldOfView(Camera.VerticalToHorizontalFieldOfView(photo_in, TGT_CAMERA_ASPECT), photoCamera.aspect);
            photo_out = Camera.HorizontalToVerticalFieldOfView(Camera.VerticalToHorizontalFieldOfView(photo_out, TGT_CAMERA_ASPECT), photoCamera.aspect); 
        }

        // adjust the zoom accordingly

        float target_player_fov = zooming ? player_in : player_out;
        float target_photo_fov = zooming ? photo_in : photo_out;
        float smoothTime = zooming ? ZOOM_IN_TIME : ZOOM_OUT_TIME;

        playerCamera.fieldOfView = Mathf.SmoothDamp(
            playerCamera.fieldOfView, target_player_fov, ref _playerFovVel, smoothTime);

        photoCamera.fieldOfView = Mathf.SmoothDamp(
            photoCamera.fieldOfView, target_photo_fov, ref _photoFovVel, smoothTime);
    }

    public void ZoomIn()
    {
        if (!zooming)
        {
            audioManager.CameraZoomIn();
        }
        zooming = true;
    }

    public void ZoomOut()
    {
        if (zooming)
        {
            audioManager.CameraZoomOut();
        }
        zooming = false;
    }
}
