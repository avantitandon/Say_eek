using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //COPY PASTAS
        // [SerializeField] private AK.Wwise.Event
        // AkSoundEngine.SetRTPCValue("RTPC_Name", value, gameObject);
        // AkSoundEngine.SetState("StateGroup_Name", "State_Value");




    [Header("Music and Ambience Events")]
    //Old
    [SerializeField] private AK.Wwise.Event playMusic;
    [SerializeField] private AK.Wwise.Event playAmbience;
    //New
    [SerializeField] private AK.Wwise.Event playMainMusicSwitchContainer;


    [Header("Player Events")]
    [SerializeField] private AK.Wwise.Event playFootsteps;
    [SerializeField] private AK.Wwise.Event stopFootsteps;


    [Header("Camera Events")]
    //Camera Capture
    [SerializeField] private AK.Wwise.Event playCameraCapture;
    [SerializeField] private AK.Wwise.Event playHarpPlayer;

    //Camera Activety
    [SerializeField] private AK.Wwise.Event playCameraUp;
    [SerializeField] private AK.Wwise.Event playCameraActive;
    [SerializeField] private AK.Wwise.Event playCameraDown;

    //Camera Zoom
    [SerializeField] private AK.Wwise.Event playCameraZoomIn;
    [SerializeField] private AK.Wwise.Event playCameraZoomOut;


    [Header("Game Objects")]
    [SerializeField] private GameObject player; 
    
    
    
    // VARIABLES //
    private bool isPlayingFootsteps = false;
    private string currentBarSwitch = "Bar_01";




    void Start()
    {   
        Debug.Log("Music and Ambience is playing");

        //playMusic.Post(player);
        playAmbience.Post(player);

        //this plays the mainmusiccontainer and gets the cues in the music segment to switch the harp switch
        playMainMusicSwitchContainer.Post(player, (uint)AkCallbackType.AK_MusicSyncUserCue, MusicCueCallback, null);

        AkSoundEngine.SetState("Area", "SpawnStart");
        AkSoundEngine.SetState("StageState", "DjDevil");
        AkSoundEngine.SetState("TutorialState", "Start");
    }
    private void MusicCueCallback(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info)
    {  
        if (in_type != AkCallbackType.AK_MusicSyncUserCue)
            return;

        AkMusicSyncCallbackInfo cueInfo = in_info as AkMusicSyncCallbackInfo;
        if (cueInfo == null)
            return;

        string cueName = cueInfo.userCueName;

        if (cueName.StartsWith("Bar_"))
        {
            currentBarSwitch = cueName;
        }
    }
    public void HandleTutorialAudio(GameController.TutorialStep tutorialStep)
    {
        switch (tutorialStep)
        {
            case GameController.TutorialStep.IntroDelay:
                AkSoundEngine.SetState("TutorialState", "Start");
                break;
            case GameController.TutorialStep.ShowBossDialogue:
                break;
            case GameController.TutorialStep.WaitForPhoto:
                break;
            case GameController.TutorialStep.PhotoDelay:
                AkSoundEngine.SetState("TutorialState", "Picture");
                break;
            case GameController.TutorialStep.ShowPhotoCompleteDialogue:
                break;
            case GameController.TutorialStep.Complete:
                AkSoundEngine.SetState("TutorialState", "End");
                break;
        }
    }
    public void HandleMusic(string areaStateName)
    {
        AkSoundEngine.SetState("Area", areaStateName);
    }

    // CAMERA EVENTS //
    public void CameraCapture()
    {
        playCameraCapture.Post(player);

        // Harp strum w/ switch based on current bar cue
        AkSoundEngine.SetSwitch("HarpSwitch", currentBarSwitch, player);
        playHarpPlayer.Post(player);
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


    // PLAYER EVENTS
    public void HandleFootsteps(Vector3 movementDirection, bool isSlowWalking)
    {
        bool isMoving = movementDirection.magnitude > 0.1f;
        if (isSlowWalking)
        {
            Debug.Log("Player is slow walking. Setting RTPC value to 1.");
            AkSoundEngine.SetRTPCValue("IsSlowWalking", 1f, player);
        }
        else
        {
            AkSoundEngine.SetRTPCValue("IsSlowWalking", 0f, player);
        }
        if (isMoving)
        {
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


