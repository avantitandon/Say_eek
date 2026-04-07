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
        [SerializeField] AK.Wwise.Event playFountain;
        bool fountainPlaying = false;
        

    [Header("HUD Events")]
        [SerializeField] private AK.Wwise.Event playBossText;

    [Header("Player Events")]
        [SerializeField] private float maxSpeedStepInterval = 0.48f; //fullspeed
        [SerializeField] private float maxSlowSpeedInterval = 1.3f; //slowest
        [SerializeField] private float minMoveThreshold = 0.15f; //minmovement
        [SerializeField] private AK.Wwise.Event playRubbleFootstep;
        [SerializeField] private AK.Wwise.Event playStoneFootstep;
        [SerializeField] private AK.Wwise.Event playStartFootstep;
        [SerializeField] private AK.Wwise.Event stopStartFootstep;
        [Header("Footstep Settings")]
        [SerializeField] private float distanceToFeet = 0.5f;
        [SerializeField] private float emitterLifetime = 1f;
        private float footstepTimer = 0f;

    
    [Header("NPC Events")]
        [SerializeField] private AK.Wwise.Event playBeezakaDialogue;
        bool beezakaEvent = true;
        [SerializeField] private AK.Wwise.Event playBeezakaFootstep;
        [SerializeField] private AK.Wwise.Event playBeezakaFootstepSoft;
        
        //OLD
        [SerializeField] private AK.Wwise.Event playZekeHornBig;
        [SerializeField] private AK.Wwise.Event playZekeHornSmall;
        [SerializeField] private AK.Wwise.Event playZekeHornWelcome;
        [SerializeField] private AK.Wwise.Event playZekeHornPanic;
        [SerializeField] private AK.Wwise.Event playZekeHornFlyAway;
        //NEW
        [SerializeField] private AK.Wwise.Event playZekeHorn;
        string zekeHornState = "NoHorn";

        


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
        [SerializeField] private GameObject zekeEmitter;

        [SerializeField] private GameObject churchBellEmitter;
        
        [SerializeField] private GameObject fountainEmitterFL;
        [SerializeField] private GameObject fountainEmitterFR;
        [SerializeField] private GameObject fountainEmitterBL;
        [SerializeField] private GameObject fountainEmitterBR;


    [Header("Scene Objects")]
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject beezaka; 
        [SerializeField] private GameObject zeke;





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

        //ZekeStates//
        //Zeehaviours.BlowBigHorn += PlayZekeHornBig;
        //Zeehaviours.BlowSmallHorn += PlayZekeHornSmall;
        //Zeehaviours.BlowWelcomeHorn += PlayZekeHornWelcome;
        //Zeehaviours.BlowPanicHorn += PlayZekeHornPanic;
        //Zeehaviours.BlowFlyAwayHorn += PlayZekeHornFlyAway;

        HandleNPCEvents();
        HandleFountain();

    }
   void Update()
    {
        Vector3 playerPosition = player.transform.position;
        feetEmitter.transform.position = new Vector3(playerPosition.x, feetYWorldPosition, playerPosition.z);
        
        Vector3 beezakaPosition = beezaka.transform.position;
        beezakaEmitter.transform.position = new Vector3(beezakaPosition.x, beezakaPosition.y, beezakaPosition.z);

        Vector3 zekePostition = zeke.transform.position;
        zekeEmitter.transform.position = new Vector3(zekePostition.x, zekePostition.y, zekePostition.z);
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
    public void CameraCapture(int score)
    {
        playCameraCapture.Post(player);
        if (score >= 8)
        {
            AkSoundEngine.SetSwitch("HarpSwitch", currentBarSwitch, player);
            playHarpPlayer.Post(player);
        }

        
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
            stopStartFootstep.Post(player);
            return;
        }

        float currentStepInterval = maxSpeedStepInterval / moveAmount;

        footstepTimer += Time.deltaTime;
        if (isMoving)
        {
            playStartFootstep.Post(player);
            
        }

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
    

    public void HandleZekeHorn(GameObject npc, string currentAnimation)
    {
        
    }
    

    // WORLD Methods //
    public void PlayChurchbell()
    {
        playChurchBell.Post(churchBellEmitter);
    }
    public void HandleFountain()
    {
        if (fountainPlaying) return;
        fountainPlaying = true;

        AkSoundEngine.SetSwitch("FountainPosition", "FL", fountainEmitterFL);
        playFountain.Post(fountainEmitterFL);
        AkSoundEngine.SetSwitch("FountainPosition", "FR", fountainEmitterFR);
        playFountain.Post(fountainEmitterFR);
        AkSoundEngine.SetSwitch("FountainPosition", "BL", fountainEmitterBL);
        playFountain.Post(fountainEmitterBL);
        AkSoundEngine.SetSwitch("FountainPosition", "BR", fountainEmitterBR);
        playFountain.Post(fountainEmitterBR);
    }


    // HUD Methods //
    public void PlayBossText()
    {
        playBossText.Post(player); 
    }


    // EVENT Methods //




    public void HandleZekeHorn(ZekeManager.WhatShouldZekeDo whatShouldZekeDo)
    {
        switch (whatShouldZekeDo)
        {   
            case ZekeManager.WhatShouldZekeDo.WaitStart:
                AkSoundEngine.SetSwitch("ZekeHornSwitch", "Welcome", zeke);
                zekeHornState = "Welcome";
                break;
            case ZekeManager.WhatShouldZekeDo.SeekPlayer:
                AkSoundEngine.SetSwitch("ZekeHornSwitch", "NoHorn", zeke);
                zekeHornState = "NoHorn";
                break;
            case ZekeManager.WhatShouldZekeDo.SeekEvent:
                AkSoundEngine.SetSwitch("ZekeHornSwitch", "FlyAway", zeke);
                zekeHornState = "FlyAway";
                break;
            case ZekeManager.WhatShouldZekeDo.GOTOSPECIALSPOT:
                AkSoundEngine.SetSwitch("ZekeHornSwitch", "FlyAway", zeke);
                zekeHornState = "FlyAway";
                break;
            case ZekeManager.WhatShouldZekeDo.ChillOut:
                AkSoundEngine.SetSwitch("ZekeHornSwitch", "NoHorn", zeke);
                zekeHornState = "NoHorn";
                break;
            case ZekeManager.WhatShouldZekeDo.Panic:
                AkSoundEngine.SetSwitch("ZekeHornSwitch", "Panic", zeke );
                zekeHornState = "Panic";
                break;
        }

    }
    
    public void PlayZekeHorn(GameObject npc, string currentAnimation)
    {
        if (zekeHornState == "NoHorn")
        {
            if (currentAnimation == "zeke little horn")
            {
                AkSoundEngine.SetSwitch("ZekeHornSwitch", "Small", zeke);
            }
            else if (currentAnimation == "zeke big horn")
            {
                AkSoundEngine.SetSwitch("ZekeHornSwitch", "Big", zeke);
            }

            
            
        }

     //   else if (zekeHornState == "Welcome" && currentAnimation == "zeke welcome")
     //   {
     //       AkSoundEngine.SetState("ZekeHornState", "Welcome");
    //    }
    //    else if (zekeHornState == "Panic" && currentAnimation == "zeke panic horn")
    //    {
    //        AkSoundEngine.SetState("ZekeHornState", "Panic");
    //    }
    //    else if (zekeHornState == "FlyAway" && currentAnimation == "zeke over HERE horn")
    //    {
   //         AkSoundEngine.SetState("ZekeHornState", "FlyAway");
    //    }
    //    else if (currentAnimation == "zeke little horn")
    //    {
    //        AkSoundEngine.SetState("ZekeHornState", "Small");
    //    }
   //     else if (currentAnimation == "zeke big horn")
   //     {
    //        AkSoundEngine.SetState("ZekeHornState", "Big");
    //    }

        playZekeHorn.Post(zekeEmitter);
    }
}