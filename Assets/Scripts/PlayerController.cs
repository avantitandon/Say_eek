using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{

    // USER INPUTS //
    // see GameController for debug inputs and start/end sequence inputs


    InputAction moveAction;

    InputAction lookAction;

    InputAction holdCameraAction;

    InputAction zoomAction;

    InputAction photoAction;

    InputAction dialogueAdvanceAction;

    // AUDIO //

    // GAME COMPONENTS //

    [SerializeField] private MovementStateManager moveManager;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private HUDManager hudManager;

    // VARIABLES //

    private int photosTaken = 0;
    private bool gameplayInputEnabled = true;

    private List<int> scores;
    private List<Texture2D> photos;

    // LOGGING //


    public int GetPhotosTaken()
    {
        return photosTaken;
    }

    public void SetGameplayInputEnabled(bool isEnabled)
    {
        gameplayInputEnabled = isEnabled;
    }

    public bool WasDialogueAdvancePressedThisFrame()
    {
        return dialogueAdvanceAction != null && dialogueAdvanceAction.WasPressedThisFrame();
    }

    public void ResetState()
    {
        photosTaken = 0;
        scores = new List<int>();
        photos = new List<Texture2D>(); 
        // not really dealing with deleting / long term saving of old textures
        // leads to overflow after too many photos across resets

        // probably a good idea to move to starting position here
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        lookAction = InputSystem.actions.FindAction("Look");
        holdCameraAction = InputSystem.actions.FindAction("ToggleCamera");
        zoomAction = InputSystem.actions.FindAction("Zoom");
        photoAction = InputSystem.actions.FindAction("Attack");
        dialogueAdvanceAction = InputSystem.actions.FindAction("PhoneToggle");

        // check what this does
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // initiate the lists to store scores and photos
        ResetState();
    }

    // Update is called once per frame
    void Update()
    {
        // blocking some things the player can do. We can change this
        if (!gameplayInputEnabled)
        {
            cameraController.Release();
            cameraController.ZoomOut();
            moveManager.SetWalkSpeed();
            return;
        }

        // move the player with the current move input
        moveManager.MovePlayer(moveAction.ReadValue<Vector2>());

        // adjust the direction the camera is facing
        // use current look input, and current player position
        cameraController.Look(lookAction.ReadValue<Vector2>(), transform.position);

        // hold the camera up if the button is pressed (held)
        if (holdCameraAction.IsPressed())
        {
            cameraController.Hold();
            moveManager.SetCameraUpWalkSpeed();
        }
        // let the camera fall if the button is released
        else
        {
            cameraController.Release();
            moveManager.SetWalkSpeed();
        }

        // zoom in if the zoom button is pressed (held)
        if (zoomAction.IsPressed() && cameraController.IsCameraUp())
        {
            cameraController.ZoomIn();
        }
        // in camera controller, consider making it that putting down camera while fully zoomed in
        // instantly fully zooms out, like putting down a spyglass.
        else
        {
            cameraController.ZoomOut();
        }


        // take a photo if the photo button was pressed down
        // only called on the first frame the photo button is pushed down
        if (photoAction.WasPressedThisFrame() && cameraController.IsCameraUp())
        {
            // score is int, photo is Texture2D
            var (curr_score, curr_photo) = cameraController.TakePhoto();
            photosTaken += 1;

            // save the score and the photo
            scores.Add(curr_score);
            photos.Add(curr_photo);

            // display the photo
            hudManager.DisplayPhotoPreview(curr_score, curr_photo);
            hudManager.ShowPictureBossText();


            // LogPlaytest($"Photo taken. index={photosTaken + 1}/{MAX_PHOTOS} score={curr_score} timeRemaining={TimeRemainingSeconds:0.0}s");
        }

        // // If photo input is blocked log why 
        // else if (photoAction.WasPressedThisFrame() && Time.unscaledTime - lastBlockedPhotoLogTime > 0.5f)
        // {
        //     string reason = isPhoneOpen ? "phone_open" : (camUp.IsCameraActive() ? "unknown" : "camera_not_active");
        //     LogPlaytest($"Photo input ignored. reason={reason}");
        //     lastBlockedPhotoLogTime = Time.unscaledTime;
        // }
    }
}
