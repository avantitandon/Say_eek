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
    private const float TUTORIAL_PHOTO_RESPONSE_DELAY_SECONDS = 2f;
    private const float TUTORIAL_RETRY_MESSAGE_DURATION_SECONDS = 2.5f;
    private const int MAX_PHOTOS = 20;
    private const int TUTORIAL_VALID_PHOTO_SCORE_THRESHOLD = 0;
    private const string TUTORIAL_RETRY_LINE = "Try again. The guests need to be in frame.";

    private static readonly string[] BossDialogueLines =
    {
        "You're at the party now? Good.",
        "The brides have been waiting.",
        "Take a photo of a guest there.",
    };

// come after camera
    private static readonly string[] TutorialPhotoCompleteLines =
    {
        "Good. You should be receiving live engagement now, so pay attention.",
        "The event ends at 12:00 AM.",
        "Prove to me it was worth hiring you. Good luck.",
    };

    private static readonly string[] TutorialFollowupTextLines =
    {
        "I’ll text you updates throughout the night.",
        "Explore the venue and take a variety of photos, understood? Ghosts only.",
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
        ShowTutorialFollowupText,
        Complete
    }

    private TutorialStep tutorialStep;
    private float tutorialStepStartTime = 0.0f;
    private int lastEvaluatedTutorialPhotoCount = 0;
    private int pendingTutorialPhotoScore = -1;
    private int tutorialFollowupTextIndex = 0;
    private bool tutorialFollowupTextStarted = false;
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
                    hudManager.HideIncomingCallPrompt();
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
                if (playerController.GetPhotosTaken() > lastEvaluatedTutorialPhotoCount)
                {
                    lastEvaluatedTutorialPhotoCount = playerController.GetPhotosTaken();
                    pendingTutorialPhotoScore = playerController.GetLastPhotoScore();
                    playerController.SetGameplayInputEnabled(false);
                    tutorialStep = TutorialStep.PhotoDelay;
                    tutorialStepStartTime = Time.unscaledTime;
                }
                break;

            case TutorialStep.PhotoDelay:
                if (Time.unscaledTime - tutorialStepStartTime >= TUTORIAL_PHOTO_RESPONSE_DELAY_SECONDS)
                {
                    if (pendingTutorialPhotoScore <= TUTORIAL_VALID_PHOTO_SCORE_THRESHOLD)
                    {
                        hudManager.ShowTemporaryBossText(TUTORIAL_RETRY_LINE, TUTORIAL_RETRY_MESSAGE_DURATION_SECONDS);
                        playerController.SetGameplayInputEnabled(true);
                        pendingTutorialPhotoScore = -1;
                        tutorialStep = TutorialStep.WaitForPhoto;
                    }
                    else
                    {
                        hudManager.HideBossText();
                        pendingTutorialPhotoScore = -1;
                        tutorialStep = TutorialStep.ShowPhotoCompleteDialogue;
                    }
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
                    hudManager.HideBossText();
                    tutorialFollowupTextIndex = 0;
                    tutorialFollowupTextStarted = false;
                    tutorialStep = TutorialStep.ShowTutorialFollowupText;
                    tutorialStepStartTime = Time.unscaledTime;
                    Debug.Log("GameController: tutorial c dialogue complete.");
                }
                break;

            case TutorialStep.ShowTutorialFollowupText:
                if (tutorialFollowupTextIndex < TutorialFollowupTextLines.Length)
                {
                    if (!tutorialFollowupTextStarted)
                    {
                        hudManager.ShowCustomPictureBossText(
                            TutorialFollowupTextLines[tutorialFollowupTextIndex],
                            0f);
                        tutorialFollowupTextStarted = true;
                    }

                    if (playerController.WasDialogueAdvancePressedThisFrame())
                    {
                        tutorialFollowupTextIndex += 1;
                        tutorialFollowupTextStarted = false;

                        if (tutorialFollowupTextIndex < TutorialFollowupTextLines.Length)
                        {
                            hudManager.ShowCustomPictureBossText(
                                TutorialFollowupTextLines[tutorialFollowupTextIndex],
                                0f);
                            tutorialFollowupTextStarted = true;
                        }
                    }
                }

                if (tutorialFollowupTextIndex >= TutorialFollowupTextLines.Length)
                {
                    hudManager.HidePictureBossText();
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
        hudManager.SetPictureBossTextEnabled(true);
        Debug.Log("GameController: tutorial finished, gameplay enabled.");
    }

    private void StartEndSequence()
    {
        gameState = State.End;
    }
}
