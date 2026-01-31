using UnityEngine;
using UnityEngine.InputSystem;

// for saving screenshots
using System.IO;

// for ui text
using TMPro;


public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float mouseSensitivity = 150f;
    [SerializeField] private float minY = -35f;
    [SerializeField] private float maxY = 60f;
    // audioSource;

    private float xRotation;
    private float yRotation;

    public GameObject body1;
    public GameObject body2;

    public AudioClip shutter;
    AudioSource cameraAudio;

    public Camera playerCamera;
    public Camera photoCamera;
    public RenderTexture photort;

    public float normalFov = 60f;
    public float zoomFov = 50f;
    public float zoomInTime = 0.15f;
    public float zoomOutTime = 0.20f;

    private float _playerFovVel;
    private float _photoFovVel;


    InputAction lookAction;
    InputAction photoAction;
    InputAction zoomAction;

    InputAction saveAction;
    InputAction ghostAction;

    bool ghostsOn = false;


    public int photoScore = 0;
    public TMP_Text UItext;


    int ghostHit = 0;


    void Start()
    {
        cameraAudio = GetComponent<AudioSource>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        lookAction = InputSystem.actions.FindAction("Look");
        photoAction = InputSystem.actions.FindAction("Attack");
        zoomAction = InputSystem.actions.FindAction("Zoom");

        saveAction = InputSystem.actions.FindAction("Interact");
        ghostAction = InputSystem.actions.FindAction("GhostDbg");

        playerCamera.fieldOfView = normalFov;
        photoCamera.fieldOfView  = normalFov;

    }

    void LateUpdate()
    {
        UItext.text = "Last Photo Score: " + photoScore.ToString() + "\nGhost Hit: " + ghostHit.ToString();
        if (photoAction.WasPressedThisFrame())
        {
            TakePhoto();
            photoScore += 10;
            
        }
        if (saveAction.WasPressedThisFrame())
        {
            SavePhoto();
        }

        if (ghostAction.WasPressedThisFrame())
        {
            if (!ghostsOn)
            {
                playerCamera.cullingMask = 127;
                ghostsOn = true;
            }
            else
            {
                playerCamera.cullingMask = 63;
                ghostsOn = false;
            }
        }



        Vector3 lookValue = lookAction.ReadValue<Vector2>();
        float mouseX = lookValue.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookValue.y * mouseSensitivity * Time.deltaTime;

        bool zooming = zoomAction.IsPressed(); // hold zoom

        float target = zooming ? zoomFov : normalFov;
        float smoothTime = zooming ? zoomInTime : zoomOutTime;

        playerCamera.fieldOfView = Mathf.SmoothDamp(
            playerCamera.fieldOfView, target, ref _playerFovVel, smoothTime);

        photoCamera.fieldOfView = Mathf.SmoothDamp(
            photoCamera.fieldOfView, target, ref _photoFovVel, smoothTime);

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minY, maxY);


        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        transform.position = player.position + Vector3.up * 1.6f;
    }


    public Vector3 GetCameraForward()
    {
        Vector3 forward = transform.forward;
        forward.y = 0;
        return forward.normalized;
    }


    public Vector3 GetCameraRight()
    {
        Vector3 right = transform.right;
        right.y = 0;
        return right.normalized;
    }

    public void TakePhoto()
    {
        photoCamera.Render();
        Debug.Log("Screenshot captured");
        cameraAudio.PlayOneShot(shutter);


        // needs to be recalculated for new camera position
        Vector3 pos = new Vector3(0, 0, 0);
        Ray ray = photoCamera.ScreenPointToRay(pos);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.layer == 6)
            {
                ghostHit = 1;
            }
            else
            {
                ghostHit = 2;
            }
        }
        else
        {
            ghostHit = 3;
        }

    }

    public void SavePhoto()
    {
        SaveTextureToFileUtility.SaveRenderTextureToFile(photort, Application.dataPath + "/Screenshots/screenshot.png");
    }
}






