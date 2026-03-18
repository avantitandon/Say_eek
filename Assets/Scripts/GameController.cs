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
    private const float TUTORIAL_INTRO_DELAY_SECONDS = 3f;

    private const int MAX_PHOTOS = 20;

    private static readonly string[] BossDialogueLines =
    {
        "INTERN! You’re at the venue now? Good. ",
        "Explore the venue and take a variety of photos, understood?",
        "The event ends at 12AM, by the way.",
        "I’ll text you updates throughout the night.",

    };

    // AUDIO //

    // GAME COMPONENTS //

    [SerializeField] private PlayerController playerController;

    [SerializeField] private HUDManager hudManager;

    // VARIABLES //

    private State gameState;
    private float roundStartTime = 0.0f;

    // LOGGING //

    [SerializeField] private bool enablePlaytestLogs = true;


    // sorry about messing up your flow martin, will clean up the code later!! want it working for the tut
    // Tutorial  code starts here


    private enum TutorialStep
    {
        IntroDelay,
        ShowBossDialogue,
        Complete
    }

    private TutorialStep tutorialStep;
    private float tutorialStepStartTime = 0.0f;
    private bool bossDialogueStarted = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // ensure we are starting with tutorial
        tutorialStep = TutorialStep.IntroDelay;
        tutorialStepStartTime = Time.time;
        // double check functionality; do we need this? does it help with lag?
        Application.targetFrameRate = 60;

        // start the game in the tutorial
        gameState = State.Tutorial;
        bossDialogueStarted = false;
        playerController.SetGameplayInputEnabled(false);
        hudManager.HideBossText();
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
        if (gameState != State.Tutorial)
        {
            return;
        }

        switch (tutorialStep)
        {
            case TutorialStep.IntroDelay:
                if (Time.time - tutorialStepStartTime >= TUTORIAL_INTRO_DELAY_SECONDS)
                {
                    tutorialStep = TutorialStep.ShowBossDialogue;
                    tutorialStepStartTime = Time.time;
                }
                break;

            case TutorialStep.ShowBossDialogue:
                if (!bossDialogueStarted)
                {
                    hudManager.BeginBossDialogue(BossDialogueLines);
                    bossDialogueStarted = true;
                }

                if (playerController.WasDialogueAdvancePressedThisFrame())
                {
                    hudManager.AdvanceBossDialogue();
                }

                if (hudManager.IsBossDialogueComplete())
                {
                    tutorialStep = TutorialStep.Complete;
                }
                break;

            case TutorialStep.Complete:
                hudManager.HideBossText();
                StartGame();
                break;
        }
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
        playerController.SetGameplayInputEnabled(true);
    }

    private void StartEndSequence()
    {
        gameState = State.End;
    }
}
