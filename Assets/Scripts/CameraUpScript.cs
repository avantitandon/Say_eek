using UnityEngine;
using UnityEngine.InputSystem;

public class CameraUpScript : MonoBehaviour
{
    // camera states

    public enum State
    {
        Down,
        Rising,
        Up,
        Falling
    }

    // CONSTANTS //

    // AUDIO //
    [SerializeField] private AK.Wwise.Event cameraUpEvent;
    [SerializeField] private AK.Wwise.Event cameraDownEvent;


    // GAME COMPONENTS //

    [Header("Animator")]
    [SerializeField] private GameObject cameraModel;

    [SerializeField] private GameObject framelines;
    [SerializeField] private Animator anim;


    // VARIABLES //
    [SerializeField] private string stateName = "CameraUp";
    [SerializeField] private int layer = 0;
    [SerializeField] private float speed = 1f;
    

    [Header("Hold Behavior")]
    [Range(0f, 1f)]
    
    [SerializeField] private float holdStart;
    // like holdcontinue but where we want the animation to end on the way up

    [Header("Hold Behavior")]
    [Range(0f, 1f)]
    [SerializeField] private float holdContinue;
    // changing this doesn't matter much, overriden by prefab (good thing i checked editor in game)
    // was 0.454 in prefab before trimming

    // How far from the start we must get before we allow clamping to holdNormalized
    [SerializeField] private float minProgressBeforeHold = 0.02f;
    [Header("Playtest Logging")]
    [SerializeField] private bool enablePlaytestLogs = true;

    // if the camera is on the way up
    private State state;


    public bool IsCameraUp()
    {
        return state == State.Up;
    }

    void Awake()
    {
        if (anim == null) anim = GetComponentInChildren<Animator>();
        framelines.SetActive(false);
    }

    void Start()
    {
        state = State.Down;
    }

    void Update()
    {

    }

    // call this function every frame that the camera key is held down
    public void Hold()
    {
        // if the camera is down, make it rise
        if (state == State.Down)
        {
            anim.speed = speed;
            anim.Play(stateName, layer, 0f);
            anim.Update(0f);

            state = State.Rising;
            LogPlaytest("Camera moving up.");
        }
        // if the camera is rising, check if we should stop
        else if (state == State.Rising)
        {
            // get the current animation state
            var st = anim.GetCurrentAnimatorStateInfo(layer);

            // if we are at the top, stop
            if (st.normalizedTime >= holdStart)
            { // holding at holdNormalized

                anim.Play(stateName, layer, holdContinue);
                framelines.SetActive(true);
                cameraModel.SetActive(false);
                anim.speed = 0f;

                state = State.Up;
                cameraUpEvent.Post(gameObject);
                LogPlaytest("Camera active.");
            }
            // continue otherwise
            else if (st.normalizedTime < minProgressBeforeHold) {
                anim.speed = speed;
            }    
        }

        // consider allowing the player to pull up the camera while it is falling
    }

    // call this function every frame that the camera key is released
    public void Release()
    {
        // if the camera is up, have it start coming down
        if (state == State.Up)
        {
            framelines.SetActive(false);
            cameraModel.SetActive(true);
            anim.speed = speed;
            anim.Play(stateName, layer, holdContinue);
            anim.Update(0f);

            state = State.Falling;
            cameraDownEvent.Post(gameObject);
            LogPlaytest("Camera moving down.");
        }
        // if the camera is falling, check if it is done
        else if (state == State.Falling)
        {
            // get the animation state
            var st = anim.GetCurrentAnimatorStateInfo(layer);
            
            // if the camera finished falling
            if (st.IsName(stateName) && st.normalizedTime >= 1f) {
                state = State.Down;
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
