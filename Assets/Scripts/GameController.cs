using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{

    public bool gameActive = false;

    public const int MAX_PHOTOS = 3;
    public int photosTaken = 0;
    public int[] scores;

    public GameObject player;
    public GameObject camera;
    public GameObject debugOverlay;


    // a camera controller should take care of # of photos taken


    InputAction enableGameAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // bind inputs
        enableGameAction = InputSystem.actions.FindAction("EnableGame");

        // create score array
        scores = new int[MAX_PHOTOS];
    }

    // Update is called once per frame
    void Update()
    {
        if (enableGameAction.WasPressedThisFrame())
        {
            gameActive = true;
            debugOverlay.SetActive(false);

        }
        if (gameActive)
        {

        }
    }
}
