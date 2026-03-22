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
        private string currentBarSwitch = "Bar_01";


    [Header("Player Events")]
        [SerializeField] private float maxSpeedStepInterval = 0.48f; //fullspeed
        [SerializeField] private float maxSlowSpeedInterval = 1.3f; //slowest
        [SerializeField] private float minMoveThreshold = 0.15f; //minmovement
        [SerializeField] private AK.Wwise.Event playRubbleFootstep;
        [SerializeField] private AK.Wwise.Event playStoneFootstep;
        [Header("Footstep Settings")]
        [SerializeField] private float distanceToFeet = 0.5f;
        [SerializeField] private float emitterLifetime = 1f;
        private float footstepTimer = 0f;


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

    [Header("Audio Game Objects")]
        [SerializeField] private GameObject feetEmitter;
        [SerializeField] private float feetYWorldPosition = 0f;


    [Header("Scene Objects")]
        [SerializeField] private GameObject player; 





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
   void LateUpdate()
    {
        Vector3 playerPosition = player.transform.position;
        
        feetEmitter.transform.position = new Vector3(playerPosition.x, feetYWorldPosition, playerPosition.z);
    }


   // MUSIC Methods //
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

    // CAMERA methods //
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


    // PLAYER Methods //
    public void HandleFootsteps(Vector3 movementDirection)
    {
        float moveAmount = movementDirection.magnitude;
        bool isMoving = moveAmount > 0.01f;

        if (!isMoving)
        {
            footstepTimer = 0f;
            return;
        }

        float currentStepInterval = maxSpeedStepInterval / moveAmount;

        footstepTimer += Time.deltaTime;

        if (footstepTimer >= currentStepInterval)
        {
            playRubbleFootstep.Post(feetEmitter);
            playStoneFootstep.Post(player);
            footstepTimer = 0f;
        }
    }
    // bruh I had a whole function here to make a gameobject at the players feet and played a footstep texture. I realized I can do this IN WWISE SO COOL
}


