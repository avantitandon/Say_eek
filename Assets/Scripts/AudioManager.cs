using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //COPY PASTAS
        // [SerializeField] private AK.Wwise.Event
    [Header("Music and Ambience Events")]
    [SerializeField] private AK.Wwise.Event PlayMusic;
    [SerializeField] private AK.Wwise.Event PlayAmbience;
    [Header("Player Events")]
    [SerializeField] private AK.Wwise.Event playFootsteps;
    [SerializeField] private AK.Wwise.Event stopFootsteps;
    [Header("Camera Events")]
    [SerializeField] private AK.Wwise.Event playCameraCapture;
    [SerializeField] private AK.Wwise.Event playCameraUp;
    [SerializeField] private AK.Wwise.Event playCameraDown;

    [Header("Game Objects")]
    [SerializeField] private GameObject player; 
    
    
    
    
    private bool isPlayingFootsteps = false;





    void Start()
    {   
        Debug.Log("Music and Ambience is playing");

        PlayMusic.Post(player);
        PlayAmbience.Post(player);
    }

    public void Footsteps()
    {
        

    }

    void LateUpdate()
    {
        
    }
    public void CameraCapture()
    {
        playCameraCapture.Post(player);
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


