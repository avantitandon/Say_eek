using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //COPY PASTAS
        // [SerializeField] private AK.Wwise.Event


    [Header("Music and Ambience Events")]
    [SerializeField] private AK.Wwise.Event playMusic;
    [SerializeField] private AK.Wwise.Event playAmbience;


    [Header("Player Events")]
    [SerializeField] private AK.Wwise.Event playFootsteps;
    [SerializeField] private AK.Wwise.Event stopFootsteps;


    [Header("Camera Events")]
    //Camera Capture
    [SerializeField] private AK.Wwise.Event playCameraCapture;

    //Camera Activety
    [SerializeField] private AK.Wwise.Event playCameraUp;
    [SerializeField] private AK.Wwise.Event playCameraActive;
    [SerializeField] private AK.Wwise.Event playCameraDown;

    //Camera Zoom
    [SerializeField] private AK.Wwise.Event playCameraZoomIn;
    [SerializeField] private AK.Wwise.Event playCameraZoomOut;


    [Header("Game Objects")]
    [SerializeField] private GameObject player; 
    
    
    
    
    private bool isPlayingFootsteps = false;





    void Start()
    {   
        Debug.Log("Music and Ambience is playing");

        playMusic.Post(player);
        playAmbience.Post(player);
    }

    public void Footsteps()
    {
        

    }

    void LateUpdate()
    {
        
    }

    // CAMERA EVENTS //
    public void CameraCapture()
    {
        playCameraCapture.Post(player);
    }
    public void CameraUp()
    {
        playCameraUp.Post(player);
    }
    public void CameraActive()
    {
        playCameraActive.Post(player);
    }
    public void CameraDown()
    {
        playCameraDown.Post(player);
    }

    public void CameraZoomIn()
    {
        playCameraZoomIn.Post(player);
    }
    public void CameraZoomOut()
    {
        playCameraZoomOut.Post(player);
    }


    
    public void HandleFootsteps(Vector3 movementDirection)
    {
        bool isMoving = movementDirection.magnitude > 0.1f;

        if (isMoving)
        {
            Debug.Log("boing");
            if (!isPlayingFootsteps)
            {
                playFootsteps.Post(player);
                isPlayingFootsteps = true;
            }
        }
        else
        {
            if (isPlayingFootsteps)
            {
                stopFootsteps.Post(player);
                isPlayingFootsteps = false;
            }
        }
    }

}


