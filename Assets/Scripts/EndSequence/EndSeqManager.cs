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



    // USER INPUTS

    InputAction changePhotoAction;
    InputAction selectAction;
    InputAction deselectAction;
    InputAction sendAction;
    InputAction advanceTextAction;


    // AUDIO

    // GAME COMPONENTS

    [SerializeField] private PhotoSelectionController photoSelectionController;
    [SerializeField] private PhotoReviewController photoReviewController;
    [SerializeField] private FinalScoreController finalScoreController;

    // VARIABLES

    private List<int> scores;
    private List<Texture2D> photos;

    private State endSeqState;





    // LOGGING


    // check if the end sequence is complete
    bool IsComplete()
    {
        return endSeqState == State.Complete;
    }

    // initialize the end sequence
    void Init(List<int> gameScores, List<Texture2D> gamePhotos)
    {
        endSeqState = State.Selection;

        //photoSelectionController.

        scores = new List<int>(gameScores);
        photos = new List<Texture2D>(gamePhotos);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        changePhotoAction = InputSystem.actions.FindAction("changePhoto");
        selectAction = InputSystem.actions.FindAction("pickPhoto");
        deselectAction = InputSystem.actions.FindAction("unpickPhoto");
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
        if (endSeqState != State.Selection)
        {
            return;
        }

        //changePhotoAction.
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
    }
}
