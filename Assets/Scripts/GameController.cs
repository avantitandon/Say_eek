using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class GameController : MonoBehaviour
{

    [SerializeField] private AK.Wwise.Event GameEndEvent;
    [SerializeField] private MovementStateManager playerMovementManager;
    [SerializeField] private CameraControllerMonolith cameraController;
    [SerializeField] private EndUIController endUIController;
    [SerializeField] private PhoneUIController phoneUIController;
    [SerializeField] private CameraUpScript camUp;          // gotta add camera animation before running, i cant add to prefab idk y
    [SerializeField] private BossTextScript BossTextScript;


    [Header("Playtest Logging")]
    [SerializeField] private bool enablePlaytestLogs = true;


    public bool gameActive = false;

    // VARIABLES for the thingy 
    [SerializeField] private float roundDurationSeconds = 300f; // keeping this 2 min thirty for demo // upped to 5min for ubisoft
    // for alpha testing? // will change later
    public float TimeRemainingSeconds {get;private set;} // curr value

    public const int MAX_PHOTOS = 20;
    public int photosTaken = 0;
    public int[] scores;

    public GameObject player;
    [FormerlySerializedAs("camera")]
    public GameObject cameraObject;
    public GameObject debugOverlay;
    public GameObject gameUI;
    public GameObject endUI;
    
    public GameObject photoPreviewOverlay;

    private float endTime = 0;
    // starting timestamp?
    private float roundStartTime = 0;
    private float lastBlockedPhotoLogTime = -999f;


    // a camera controller should take care of # of photos taken

    // all input should enter through this controller, to be neat (i think?)
    // no, should be based on concern. good as is. think about what controller should control what
    // makes sense for game controller to orchestrate actions that cause scoring (?)
    // game controller shouldn't change camera fov/zoom, i think. camera controller could do that?
    // i guess should research unity input organization.
    InputAction enableGameAction;
    InputAction photoAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.targetFrameRate = 60;




        // bind inputs
        enableGameAction = InputSystem.actions.FindAction("EnableGame");
        photoAction = InputSystem.actions.FindAction("Attack");

        // create score array
        scores = new int[MAX_PHOTOS];

        if (phoneUIController == null)
        {
            phoneUIController = FindFirstObjectByType<PhoneUIController>();
        }

        if (phoneUIController == null)
        {
            phoneUIController = gameObject.AddComponent<PhoneUIController>();
        }

        phoneUIController.Initialize(
            () => endUI == null || !endUI.activeSelf);

        // Martin im switching this into start round with added protection incase 
        startRound();
        LogPlaytest($"Initialized. roundDuration={roundDurationSeconds:0.0}s maxPhotos={MAX_PHOTOS}");
    }

    // Update is called once per frame
    void Update()
    {
        if (camUp.IsCameraUp() || camUp.IsCameraActive())
        {
            playerMovementManager.moveSpeed = MovementStateManager.CAMERA_UP_WALK_SPEED;
        }
        else
        {
            playerMovementManager.moveSpeed = MovementStateManager.WALK_SPEED;
        }

        bool isPhoneOpen = phoneUIController != null && phoneUIController.IsOpen;

        // score of any photos taken this turn
        int curr_score = 0;


        bool phototaken = false;

        // take a photo
        // dont want to take a photo when clicking on the phone
        // and only when the camera is active


        // log when photo is captures
        if (photoAction.WasPressedThisFrame() && !isPhoneOpen && camUp.IsCameraActive())
        {
            phototaken = true;
            curr_score = cameraController.TakePhoto();
            LogPlaytest($"Photo taken. index={photosTaken + 1}/{MAX_PHOTOS} score={curr_score} timeRemaining={TimeRemainingSeconds:0.0}s");
        }
        // If photo input is blocked log why 
        else if (photoAction.WasPressedThisFrame() && Time.unscaledTime - lastBlockedPhotoLogTime > 0.5f)
        {
            string reason = isPhoneOpen ? "phone_open" : (camUp.IsCameraActive() ? "unknown" : "camera_not_active");
            LogPlaytest($"Photo input ignored. reason={reason}");
            lastBlockedPhotoLogTime = Time.unscaledTime;
        }

        // if camera has gone up, disable game ui
        if (camUp.IsCameraActive())
        {
            gameUI.SetActive(false);
        }

        // if camera has started to go down, renable game ui
        if (camUp.IsCameraDown())
        {
            gameUI.SetActive(true);
        }


        // if the end screen has been on for more than 7 seconds, turn it off
        if (endUI.activeSelf && (Time.time > endTime + 7))
        {
            endUI.SetActive(false);
            gameUI.SetActive(true);
            // bring back the debug overlay
            debugOverlay.SetActive(true);
            photoPreviewOverlay.SetActive(true);
        }

        if (isPhoneOpen)
        {
            return;
        }

        // if the game is inactive and the end screen is off and we ask to turn on the game, start it
        bool startPressed = enableGameAction != null && enableGameAction.WasPressedThisFrame();
        if (!startPressed && Keyboard.current != null)
        {
            startPressed = Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame;
        }

        if (!gameActive && startPressed && !endUI.activeSelf)
        {
            LogPlaytest("Start input detected. Starting round.");
            startRound();
        }

        // if the game is on
        if (gameActive)
        {
            // count remaining time here
            TimeRemainingSeconds = Mathf.Max(0f, roundDurationSeconds - (Time.time - roundStartTime));
            // if time is 0
            if (TimeRemainingSeconds <= 0f)
            {
                EndRound("timer_expired");
                return;
            }

            // PLACEHOLDER LOGIC!!!!!!!!!

            if ( TimeRemainingSeconds <= 290f  && TimeRemainingSeconds >= 289f)
            {
                BossTextScript.SetTextMessage("Boss Says Something", 1f);
            }

            // END OF PLACEHOLDER LOGIC!!!!!1

            // save photo score from this frame
            if (phototaken)
            {
                scores[photosTaken] = curr_score;
                photosTaken = photosTaken + 1;
            }

            // end the game if we have max photos
            if (photosTaken == MAX_PHOTOS)
            {
                EndRound("max_photos_reached");
            }
        }
    }

    private void startRound()
    {
        // keep these 2 lines for prod builds
        // auto starts the game
        
        gameActive = true;
        debugOverlay.SetActive(false);
        // null check 
        if (gameUI != null)
        {gameUI.SetActive(true);
        }
        if (endUI != null)
        { endUI.SetActive(false);
        }
        if (photoPreviewOverlay != null)
        {
            photoPreviewOverlay.SetActive(true);
        }
        scores = new int[MAX_PHOTOS];
        photosTaken = 0;
        roundStartTime = Time.time;
        TimeRemainingSeconds = roundDurationSeconds;
        LogPlaytest($"Round started. duration={roundDurationSeconds:0.0}s maxPhotos={MAX_PHOTOS}");
    }

    private void EndRound(string reason)
    {
        GameEndEvent.Post(gameObject);
        gameActive = false;
        endUIController.SetScoreText(scores, photosTaken);

        endTime = Time.time;
        if (gameUI!=null)
        {
            // adding proteccc
            gameUI.SetActive(false);
        }
        if (endUI!=null)
        {
            // added proteccccc
            endUI.SetActive(true);
        }
        if (photoPreviewOverlay != null)
        {
            photoPreviewOverlay.SetActive(false);
        }

// added a total score? we will use this in the future for winning/losing endings i think
        int totalScore = 0;
        for (int i = 0; i < photosTaken; i++)
        {
            totalScore += scores[i];
        }

        LogPlaytest($"Round ended. reason={reason} photosTaken={photosTaken} totalScore={totalScore}");
    }


    private void LogPlaytest(string message)
    {
        if (!enablePlaytestLogs && !PlaytestLogWriter.RuntimeLoggingEnabled)
        {
            return;
        }

        PlaytestLogWriter.Log("GameController", message);
    }
}
