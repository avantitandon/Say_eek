using UnityEngine;
using UnityEngine.InputSystem;

// for saving screenshots
using System.IO;


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



    InputAction lookAction;
    InputAction photoAction;
    InputAction zoomAction;

    InputAction saveAction;


    void Start()
    {
        cameraAudio = GetComponent<AudioSource>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        lookAction = InputSystem.actions.FindAction("Look");
        photoAction = InputSystem.actions.FindAction("Attack");
        zoomAction = InputSystem.actions.FindAction("Zoom");

        saveAction = InputSystem.actions.FindAction("Interact");
    }

    void LateUpdate()
    {
        if (photoAction.WasPressedThisFrame())
        {
            TakePhoto();
        }
        if (saveAction.WasPressedThisFrame())
        {
            SavePhoto();
        }
        Vector3 lookValue = lookAction.ReadValue<Vector2>();
        float mouseX = lookValue.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookValue.y * mouseSensitivity * Time.deltaTime;

        if (zoomAction.WasPressedThisFrame()) {
            playerCamera.fieldOfView -= 10;
            photoCamera.fieldOfView -= 10;
            Debug.Log(playerCamera.fieldOfView);
        }
        if (zoomAction.WasReleasedThisFrame())
        {
            playerCamera.fieldOfView += 10;
            photoCamera.fieldOfView += 10;
            Debug.Log(playerCamera.fieldOfView);
        }

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
    }

    public void SavePhoto()
    {
        SaveTextureToFileUtility.SaveRenderTextureToFile(photort, Application.dataPath + "/Screenshots/screenshot.png");
    }
}






