using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class GameController : MonoBehaviour
{

    [SerializeField] private CameraControllerMonolith cameraController;
    [SerializeField] private EndUIController endUIController;
    [SerializeField] private PhoneUIController phoneUIController;


    public bool gameActive = false;

    public const int MAX_PHOTOS = 3;
    public int photosTaken = 0;
    public int[] scores;

    public GameObject player;
    [FormerlySerializedAs("camera")]
    public GameObject cameraObject;
    public GameObject debugOverlay;
    public GameObject endUI;

    private float endTime = 0;


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
    }

    // Update is called once per frame
    void Update()
    {
        bool isPhoneOpen = phoneUIController != null && phoneUIController.IsOpen;

        // score of any photos taken this turn
        int curr_score = 0;

        // take a photo
        if (photoAction.WasPressedThisFrame())
        {
            curr_score = cameraController.TakePhoto();
        }

        // if the end screen has been on for more than 7 seconds, turn it off
        if (endUI.activeSelf && (Time.time > endTime + 7))
        {
            endUI.SetActive(false);
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
            gameActive = true;
            debugOverlay.SetActive(false);
            scores = new int[MAX_PHOTOS]; // garabage collection to do? maybe? could this have garbage?

        }

        // if the game is on
        if (gameActive)
        {
            // save photo score from this frame
            if (photoAction.WasPressedThisFrame())
            {
                scores[photosTaken] = curr_score;
                photosTaken = photosTaken + 1;
            }

            // end the game if we have max photos
            if (photosTaken == MAX_PHOTOS)
            {
                endUIController.SetScoreText(scores, photosTaken);

                endTime = Time.time;
                endUI.SetActive(true);
                gameActive = false;
            }
        }
    }
}
