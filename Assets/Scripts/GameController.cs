using UnityEngine;

public class GameController : MonoBehaviour
{
    private enum State
    {
        Tutorial,
        Active,
        End
    }

    // CONSTANTS //

    private const float ROUND_DURATION_SECONDS = 300f;

    private const int MAX_PHOTOS = 20;

    // AUDIO //

    // GAME COMPONENTS //

    [SerializeField] private PlayerController playerController;

    [SerializeField] private HUDManager hudManager;

    // VARIABLES //

    private State gameState;

    private float roundStartTime = 0.0f;

    // LOGGING //

    [SerializeField] private bool enablePlaytestLogs = true;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // double check functionality; do we need this? does it help with lag?
        Application.targetFrameRate = 60;

        // start the game in the tutorial
        gameState = State.Tutorial;
    }

    // Update is called once per frame
    void Update()
    {
        HandleTutorial();
        HandleGame();
        HandleEndSequence();
    }

    private void HandleTutorial()
    {
        // only run if in the tutorial state
        if (gameState != State.Tutorial)
        {
            return;
        }

        // we don't have a tutorial yet, move to game state
        gameState = State.Active;

        // if this was the tutorial, we can have substates and restrict player movement

    }

    private void HandleGame()
    {
        // only run if the game is active
        if (gameState != State.Active)
        {
            return;
        }

        float time_elapsed = Time.time - roundStartTime;
        int photos_left = MAX_PHOTOS - playerController.GetPhotosTaken();

        // update the HUD with the current time and taken photos
        hudManager.SetStatusUI(time_elapsed, photos_left);

        // end the game if either photos or time reach the limit
        if (photos_left <= 0 || (time_elapsed >= ROUND_DURATION_SECONDS))
        {
            StartEndSequence();
        }
    }

    private void HandleEndSequence()
    {
        // only run if the game is in the end state
        if (gameState != State.End)
        {
            return;
        }

        Debug.Log("Game Ended");
    }


    private void StartGame()
    {
        gameState = State.Active;
        roundStartTime = Time.time;
        playerController.ResetState();
    }

    private void StartEndSequence()
    {
        gameState = State.End;
    }
}
