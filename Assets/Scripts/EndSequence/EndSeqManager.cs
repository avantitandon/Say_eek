using UnityEngine;
using UnityEngine.InputSystem;

using System.Collections.Generic;


public class EndSeqManager : MonoBehaviour
{
    private enum State
    {
        Selection,
        Review,
        FinalScoring,
        Complete
    }


    // CONSTANTS


    private const float MIN_SCORING_TIME = 2f;



    // USER INPUTS

    InputAction changePhotoAction;
    InputAction selectAction;
    InputAction deselectAction;
    InputAction sendAction;
    InputAction advanceTextAction;


    // AUDIO

    // GAME COMPONENTS

    [SerializeField] private GameObject photoSelectionUI;
    [SerializeField] private PhotoSelectionController photoSelectionController;
    [SerializeField] private PhotoReviewController photoReviewController;
    [SerializeField] private FinalScoreController finalScoreController;

    // VARIABLES

    private List<int> scores;
    private List<Texture2D> photos;

    private State endSeqState;


    private float scoring_time;





    // LOGGING


    // check if the end sequence is complete
    public bool IsComplete()
    {
        return endSeqState == State.Complete;
    }

    // initialize the end sequence
    public void Init(List<int> gameScores, List<Texture2D> gamePhotos)
    {
        // need to set active before running script
        photoSelectionUI.SetActive(true);
        photoSelectionController.toggleCanvas(true);

        Debug.Log(gamePhotos.Count);
        Debug.Log("initing");
        endSeqState = State.Selection;

        photoSelectionController.Init(gamePhotos);

        scores = new List<int>(gameScores);
        photos = new List<Texture2D>(gamePhotos);

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        changePhotoAction = InputSystem.actions.FindAction("changePhoto");
        selectAction = InputSystem.actions.FindAction("PhoneToggle");
        deselectAction = InputSystem.actions.FindAction("PhoneToggle");
        sendAction = InputSystem.actions.FindAction("submitPhotos");
        advanceTextAction = InputSystem.actions.FindAction("advanceText");
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void HandleEndSequence()
    {
        HandleSelection();
        HandleReview();
        HandleFinalScoring();
    }

    private void HandleSelection()
    {
        bool photoSelected = false;

        if (endSeqState != State.Selection)
        {
            return;
        }

        // change photo selection
        photoSelectionController.changeSelection(changePhotoAction.ReadValue<Vector2>());

        // select current photo
        if (selectAction.WasPressedThisFrame())
        {
            photoSelected = photoSelectionController.selectCurrentPhoto();
        }
        // deselect current photo
        if (deselectAction.WasPressedThisFrame() && !photoSelected)
        {
            photoSelectionController.deselectCurrentPhoto();
        }

        // try to end photo selection
        if (photoSelectionController.attemptSubmit(sendAction.WasPressedThisFrame())) {
            photoSelectionController.toggleCanvas(false);
            List<int> featuredIds =  photoSelectionController.getFeaturedIds();
            List<int> featuredScores = new List<int>();
            foreach (int id in featuredIds)
            {
                featuredScores.Add(scores[id]);
            }

            finalScoreController.Init();
            finalScoreController.updateScoreText(featuredScores);
            finalScoreController.toggleCanvas(true);
            scoring_time = Time.time;
            endSeqState = State.FinalScoring; // skipping review for now
        }
    }

    private void HandleReview()
    {
        if (endSeqState != State.Review)
        {
            return;
        }
    }

    private void HandleFinalScoring()
    {
        if (endSeqState != State.FinalScoring)
        {
            return;
        }

        if (Time.time - scoring_time > MIN_SCORING_TIME)
        {
            finalScoreController.toggleAdvanceText(true);

            if (finalScoreController.attemptContinue(sendAction.WasPressedThisFrame()))
            {
                finalScoreController.toggleCanvas(false);
                endSeqState = State.Complete;
            }
        }
    }
}
