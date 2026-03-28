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
    // private const float TUTORIAL_POST_PHOTO_DELAY_SECONDS = 0.6f;
    private const float TUTORIAL_POST_PHOTO_DELAY_SECONDS = 2f; // 
    private const int MAX_PHOTOS = 20;

    private static readonly string[] BossDialogueLines =
    {
        "You made it to the party. Good.",
        "The brides are waiting.",
        "Take a photo of one of the guests.",
    };

// come after camera
    private static readonly string[] TutorialPhotoCompleteLines =
    {
        "Good. You should be getting live engagement now, so pay attention.",
        "The event ends at 12:00 AM.",
        "I’ll send updates throughout the night.",
        "Explore the venue and take a variety of photos. Ghosts only.",
        "Show me I was right to hire you. Good luck.",
    };

    // AUDIO //
    [SerializeField] private AudioManager audioManager;

    // GAME COMPONENTS //

    [SerializeField] private PlayerController playerController;

    [SerializeField] private HUDManager hudManager;

    [SerializeField] private EndSeqManager endSeqManager;

    // VARIABLES //

    private State gameState;
    private float roundStartTime = 0.0f;

    // LOGGING //

    [SerializeField] private bool enablePlaytestLogs = true;


    // sorry about messing up your flow martin, will clean up the code later!! want it working for the tut
    // Tutorial  code starts here


    public enum TutorialStep
    {
        IntroDelay,
        WaitForAnswerCall,
        ShowBossDialogue,
        WaitForPhoto,
        PhotoDelay,
        ShowPhotoCompleteDialogue,
        Complete
    }

    private TutorialStep tutorialStep;
    private float tutorialStepStartTime = 0.0f;
    private bool answerCallPromptStarted = false;
    private bool bossDialogueStarted = false;
    private bool photoCompleteDialogueStarted = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // ensure we are starting with tutorial
        tutorialStep = TutorialStep.IntroDelay;
        tutorialStepStartTime = Time.unscaledTime;
        // double check functionality; do we need this? does it help with lag?
        Application.targetFrameRate = 60;

        // start the game in the tutorial
        gameState = State.Tutorial;
        bossDialogueStarted = false;
        playerController.SetGameplayInputEnabled(false);
        hudManager.HideBossText();
        hudManager.SetPictureBossTextEnabled(false);
        Debug.Log("GameController: entered tutorial intro delay.");
    }

    // Update is called once per frame
    void Update()
    {
        HandleTutorial();
        HandleGame();
        HandleEndSequence();
        audioManager.HandleTutorialAudio(tutorialStep); 
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
                if (Time.unscaledTime - tutorialStepStartTime >= TUTORIAL_INTRO_DELAY_SECONDS)
                {
                    tutorialStep = TutorialStep.WaitForAnswerCall;
                    tutorialStepStartTime = Time.unscaledTime;
                    Debug.Log("GameController: tutorial intro delay complete, waiting for answer call input.");
                }
                break;

            case TutorialStep.WaitForAnswerCall:
                if (!answerCallPromptStarted)
                {
                    hudManager.ShowIncomingCallPrompt();
                    answerCallPromptStarted = true;
                }

                if (playerController.WasDialogueAdvancePressedThisFrame())
                {
                    hudManager.HideIncomingCallPrompt();
                    tutorialStep = TutorialStep.ShowBossDialogue;
                    tutorialStepStartTime = Time.unscaledTime;
                    Debug.Log("GameController: answer call prompt acknowledged, showing boss dialogue.");
                }
                break;

            case TutorialStep.ShowBossDialogue:
                if (!bossDialogueStarted)
                {
                    hudManager.BeginBossDialogue(BossDialogueLines);
                    bossDialogueStarted = true;
                    Debug.Log("GameController: boss dialogue started.");
                }

                // add logic for tutorial camera display thing

                if (playerController.WasDialogueAdvancePressedThisFrame())
                {
                    hudManager.AdvanceBossDialogue();
                }

                if (hudManager.IsBossDialogueComplete())
                {
                    hudManager.HideBossText();
                    playerController.SetGameplayInputEnabled(true);
                    tutorialStep = TutorialStep.WaitForPhoto;
                }
                break;

            case TutorialStep.WaitForPhoto:
                if (playerController.GetPhotosTaken() > 0)
                {
                    playerController.SetGameplayInputEnabled(false);
                    tutorialStep = TutorialStep.PhotoDelay;
                    tutorialStepStartTime = Time.unscaledTime;

                }
                break;

            case TutorialStep.PhotoDelay:
                if (Time.unscaledTime - tutorialStepStartTime >= TUTORIAL_POST_PHOTO_DELAY_SECONDS)
                {
                    tutorialStep = TutorialStep.ShowPhotoCompleteDialogue;

                }
                break;
            // might remove all of this from game controller after? just hard to figure out how to seperate and BossText is 
            case TutorialStep.ShowPhotoCompleteDialogue:
                if (!photoCompleteDialogueStarted)
                {
                    hudManager.BeginBossDialogue(TutorialPhotoCompleteLines);
                    photoCompleteDialogueStarted = true;

                }

                if (playerController.WasDialogueAdvancePressedThisFrame())
                {
                    hudManager.AdvanceBossDialogue();
                }

                if (hudManager.IsBossDialogueComplete())
                {
                    tutorialStep = TutorialStep.Complete;
                    Debug.Log("GameController: tutorial c dialogue complete.");
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
        hudManager.SetPictureBossTextEnabled(true);
        Debug.Log("GameController: tutorial finished, gameplay enabled.");
    }

    private void StartEndSequence()
    {
        gameState = State.End;
    }
}
