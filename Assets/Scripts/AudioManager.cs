using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private AK.Wwise.Event playMusic;
    [SerializeField] private AK.Wwise.Event playAmbience;
    [SerializeField] private AK.Wwise.Event playFootsteps;
    [SerializeField] private AK.Wwise.Event stopFootsteps;
    [SerializeField] private AK.Wwise.Event playCameraCapture;
    [SerializeField] private AK.Wwise.Event playCamerViewOpen;
    [SerializeField] private AK.Wwise.Event playCameraViewClose;
    // Flag to track if footsteps are currently playing
    private bool isPlayingFootsteps = false;




    void Start()
    {
        playMusic.Post(player);
        playAmbience.Post(player);
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
    public void playCapture()
    {
        playCameraCapture.Post(player);
    }
    public void playViewOpen()
    {
        playCamerViewOpen.Post(player);
    }
    public void playViewClose()
    {
        playCameraViewClose.Post(player);
    }




    
}
    
