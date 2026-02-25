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
    [SerializeField] private float holdNormalized = 0.45f;

    // How far from the start we must get before we allow clamping to holdNormalized
    [SerializeField] private float minProgressBeforeHold = 0.02f;

    // if the camera is on the way up
    private bool camUp = false;

    // if the camera is on the way down
    private bool camDown = false;

    // if the camera is usable (overlay on and etc) 
    private bool camActive = false;
    private bool rightClickBlocked = false;

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

    void Update()
    {
        if (Mouse.current == null || anim == null) return;

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            if (rightClickBlocked) return;

            if (!camActive) {  // camera going up animation
                rightClickBlocked = true; // block right click until we let go to prevent double triggering
                anim.speed = speed;
                anim.Play(stateName, layer, 0f);
                anim.Update(0f);

                camDown = false;
                camUp = true;
                camActive = false;
            } else {    // camera going down animation
                rightClickBlocked = true;
                framelines.SetActive(false);
                cameraModel.SetActive(true);
                anim.speed = speed;
                anim.Play(stateName, layer, holdNormalized);
                anim.Update(0f);

                camDown = true;
                camUp = false;
                camActive = false;
            }
        }

        if (camUp) {
            var st = anim.GetCurrentAnimatorStateInfo(layer);
            if (st.normalizedTime >= holdNormalized) { // holding at holdNormalized
                anim.Play(stateName, layer, holdNormalized);
                framelines.SetActive(true);
                cameraModel.SetActive(false);
                anim.speed = 0f;
                rightClickBlocked = false;

                camUp = false;
                camActive = true;
                camDown = false;
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

                rightClickBlocked = false;
            }
        }
    }
}