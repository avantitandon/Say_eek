using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class GameController : MonoBehaviour
{

    [SerializeField] private CameraControllerMonolith cameraController;
    [SerializeField] private EndUIController endUIController;
    [SerializeField] private PhoneUIController phoneUIController;


    public bool gameActive = false;

    // VARIABLES for the thingy 
    [SerializeField] private float roundDurationSeconds = 60f; // keeping this a minute
    // for alpha testing? // will change later
    public float TimeRemainingSeconds {get;private set;} // curr value

    public const int MAX_PHOTOS = 10;
    public int photosTaken = 0;
    public int[] scores;

    public GameObject player;
    [FormerlySerializedAs("camera")]
    public GameObject cameraObject;
    public GameObject debugOverlay;
    public GameObject gameUI;
    public GameObject endUI;

    private float endTime = 0;
    // starting timestamp?
    private float roundStartTime = 0;


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
    }

    // Update is called once per frame
    void Update()
    {
        bool isPhoneOpen = phoneUIController != null && phoneUIController.IsOpen;

        // score of any photos taken this turn
        int curr_score = 0;

        // take a photo
        // dont want to take a photo when clicking on the phone
        if (photoAction.WasPressedThisFrame() && !isPhoneOpen)
        {
            curr_score = cameraController.TakePhoto();
        }

        // if the end screen has been on for more than 7 seconds, turn it off
        if (endUI.activeSelf && (Time.time > endTime + 7))
        {
            endUI.SetActive(false);
            gameUI.SetActive(true);
            // bring back the debug overlay
            debugOverlay.SetActive(true);
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
            startRound();
        }

        // if the game is on
        if (gameActive)
        {
            // count remaining time here
            TimeRemainingSeconds = Mathf.Max(0f, roundDurationSeconds - (Time.time - roundStartTime));
            // if time is 0
            if (TimeRemainingSeconds <= 0f)
            {EndRound();
                return;
            }

            // save photo score from this frame
            if (photoAction.WasPressedThisFrame())
            {
                scores[photosTaken] = curr_score;
                photosTaken = photosTaken + 1;
            }

            // end the game if we have max photos
            if (photosTaken == MAX_PHOTOS)
            {
                EndRound();
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
        scores = new int[MAX_PHOTOS];
        photosTaken = 0;
        roundStartTime = Time.time;
        TimeRemainingSeconds = roundDurationSeconds;
    }

    private void EndRound()
    {
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
    }
}
