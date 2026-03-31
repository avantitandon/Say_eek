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

    [Header("World Events")]
        [SerializeField] AK.Wwise.Event playChurchBell;

    [Header("HUD Events")]
        [SerializeField] private AK.Wwise.Event playBossText;

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
    
    [Header("NPC Events")]
        [SerializeField] private AK.Wwise.Event playBeezakaDialogue;
        bool beezakaEvent = true;
        [SerializeField] private AK.Wwise.Event playBeezakaFootstep;
        [SerializeField] private AK.Wwise.Event playBeezakaFootstepSoft;


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

        [SerializeField] private GameObject beezakaEmitter;
        [SerializeField] private GameObject churchBellEmitter;


    [Header("Scene Objects")]
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject beezaka; 





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

        HandleNPCEvents();
    }
   void Update()
    {
        Vector3 playerPosition = player.transform.position;
        feetEmitter.transform.position = new Vector3(playerPosition.x, feetYWorldPosition, playerPosition.z);
        
        Vector3 beezakaPosition = beezaka.transform.position;
        beezakaEmitter.transform.position = new Vector3(beezakaPosition.x, beezakaPosition.y, beezakaPosition.z);
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

    // NPC Methods //
    public void HandleNPCEvents()
    {
        //BEEZAKA//
        if (beezakaEvent = true)
        {
            playBeezakaDialogue.Post(beezakaEmitter);
            beezakaEvent = false;
        }
        else return;
    }
    public void HandleFootstep(GameObject npc, string currentAnimation)
    //Use "Default" as the currentAnimation if there is no need for different animation states. Its ref. in NPCAnimationAudio script

    {
        //BEEZAKA//
        if (npc.name.Contains("beezaka"))
        {
            if (currentAnimation == "Default")
            {
                playBeezakaFootstep.Post(npc);
                //Debug.Log("NORMAL FOOTSTEP" + npc.name);
            }
             else if (currentAnimation == "Idle")
            {
                playBeezakaFootstepSoft.Post(npc);
                //Debug.Log("SOFT FOOTSTEP" + npc.name);
            }
        }

    }
    

    // WORLD Methods //
    public void PlayChurchbell()
    {
        playChurchBell.Post(churchBellEmitter);
    }


    // HUD Methods //
    public void PlayBossText()
    {
        playBossText.Post(player); 
    }










}