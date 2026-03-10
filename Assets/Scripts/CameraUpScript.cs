using UnityEngine;
using UnityEngine.InputSystem;

public class CameraUpScript : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] private GameObject cameraModel;
    [SerializeField] private Animator anim;
    [SerializeField] private string stateName = "CameraUp";
    [SerializeField] private int layer = 0;
    [SerializeField] private float speed = 1f;
    [SerializeField] private GameObject framelines;

    [Header("Hold Behavior")]
    [Range(0f, 1f)]
    
    [SerializeField] private float holdStart;
    // like holdcontinue but where we want the animation to end on the way up

    [Header("Hold Behavior")]
    [Range(0f, 1f)]
    [SerializeField] private float holdContinue = 0.15f; // was 0.45
    // changing this doesn't matter much, overriden by prefab (good thing i checked editor in game)
    // was 0.454 in prefab before trimming

    // How far from the start we must get before we allow clamping to holdNormalized
    [SerializeField] private float minProgressBeforeHold = 0.02f;
    [Header("Playtest Logging")]
    [SerializeField] private bool enablePlaytestLogs = true;

    // if the camera is on the way up
    private bool camUp = false;

    // if the camera is on the way down
    private bool camDown = false;

    // if the camera is usable (overlay on and etc) 
    private bool camActive = false;
    InputAction cameraToggleAction;

    public bool IsCameraUp() {
        return camUp;
    }

    public bool IsCameraDown()
    {
        return camDown;
    }

    public bool IsCameraActive()
    {
        return camActive;
    }

    void Awake()
    {
        if (anim == null) anim = GetComponentInChildren<Animator>();
        framelines.SetActive(false);
    }

    void Start()
    {
        cameraToggleAction = InputSystem.actions.FindAction("ToggleCamera");
    }

    void Update()
    {
        if (Mouse.current == null || anim == null) return;

        if (cameraToggleAction.WasPressedThisFrame())
        {
            LogPlaytest("ToggleCamera pressed.");
            if (!camActive && !camUp && !camDown) {  // camera going up animation
                anim.speed = speed;
                anim.Play(stateName, layer, 0f);
                anim.Update(0f);

                camDown = false;
                camUp = true;
                camActive = false;
                LogPlaytest("Camera moving up.");
            } 
        }

        if (!cameraToggleAction.IsPressed())
        {
            if (camActive) {    // camera going down animation
                framelines.SetActive(false);
                cameraModel.SetActive(true);
                anim.speed = speed;
                anim.Play(stateName, layer, holdContinue);
                anim.Update(0f);

                camDown = true;
                camUp = false;
                camActive = false;
                LogPlaytest("Camera moving down.");
            } 
        }
            
        if (camUp) {
            var st = anim.GetCurrentAnimatorStateInfo(layer);
            if (st.normalizedTime >= holdStart) { // holding at holdNormalized

                // don't want the camera ui to flash on screen if going down right away. just copied code from start camera down.
                if (!cameraToggleAction.IsPressed())
                {
                    framelines.SetActive(false);
                    cameraModel.SetActive(true);
                    anim.speed = speed;
                    anim.Play(stateName, layer, holdContinue);
                    anim.Update(0f);

                    camDown = true;
                    camUp = false;
                    camActive = false;
                    LogPlaytest("Camera moving down before activation.");
                }
                else
                {
                    anim.Play(stateName, layer, holdContinue);
                    framelines.SetActive(true);
                    cameraModel.SetActive(false);
                    anim.speed = 0f;

                    camUp = false;
                    camActive = true;
                    camDown = false;
                    LogPlaytest("Camera active.");
                }
            } else if (st.normalizedTime < minProgressBeforeHold) { // allow going back up if we haven't reached the hold point
                anim.speed = speed;
            }

        }

        // playing the rest of the animation
        if (camDown) {
            var st = anim.GetCurrentAnimatorStateInfo(layer);
            if (st.IsName(stateName) && st.normalizedTime >= 1f) { // finished going down
                camUp = false;
                camDown = false;
                camActive = false;
                LogPlaytest("Camera idle/down.");
            }
        }
    }

    private void LogPlaytest(string message)
    {
        if (!enablePlaytestLogs && !PlaytestLogWriter.RuntimeLoggingEnabled)
        {
            return;
        }

        PlaytestLogWriter.Log("CameraUp", message);
    }
}
