using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

// for saving screenshots
using System.IO;

// for ui text
using TMPro;



// Camera Controller Monolith
// functionality still needs to be split into other components
public class CameraControllerMonolith : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float mouseSensitivity = 150f;
    [SerializeField] private float minY = -35f;
    [SerializeField] private float maxY = 60f;
    [SerializeField] private ApertureShot apertureFx;
    // audioSource;

    private float xRotation;
    private float yRotation;


    public AudioClip shutter;
    AudioSource cameraAudio;

    public Camera playerCamera;
    public Camera photoCamera;
    public RenderTexture photort;

    public float normalFov = 60f;
    public float zoomFov = 50f;
    public float zoomInTime = 0.15f;
    public float zoomOutTime = 0.20f;
    [Header("Camera Mesh")]
    [SerializeField] private GameObject cameraMeshPrefab;
    [SerializeField] private Vector3 cameraMeshLocalPosition = new Vector3(0.25f, -0.15f, 0.4f);
    [SerializeField] private Vector3 cameraMeshLocalEuler = new Vector3(0f, 180f, 0f);
    private GameObject cameraMeshInstance;

    private float _playerFovVel;
    private float _photoFovVel;
    [Header("Photo Preview")]
    [SerializeField] private RawImage photoPreview;
    [SerializeField] private float previewDuration = 1f;
    private Coroutine previewRoutine;
    private Texture2D previewTexture;

    public PanelScript panelScript;
    [SerializeField] private PhoneUIController phoneUIController;


    InputAction lookAction;
    InputAction zoomAction;

    InputAction saveAction;
    InputAction ghostAction;

    bool ghostsOn = false;


    public int photoScore = 0;
    public TMP_Text UI_text;


    int ghostScore = 0;

    public LayerMask ignoreLayers;


    void Start()
    {
        cameraAudio = GetComponent<AudioSource>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        lookAction = InputSystem.actions.FindAction("Look");
        zoomAction = InputSystem.actions.FindAction("Zoom");

        saveAction = InputSystem.actions.FindAction("Interact");
        ghostAction = InputSystem.actions.FindAction("GhostDbg");

        playerCamera.fieldOfView = normalFov;
        photoCamera.fieldOfView  = normalFov;


        if (cameraMeshPrefab != null && playerCamera != null)
        {
            cameraMeshInstance = Instantiate(cameraMeshPrefab, playerCamera.transform);
            cameraMeshInstance.transform.localPosition = cameraMeshLocalPosition;
            cameraMeshInstance.transform.localEulerAngles = cameraMeshLocalEuler;
        }

        if (photoPreview != null)
        {
            photoPreview.texture = null;
            photoPreview.gameObject.SetActive(false);
        }
        else
        {
            CreatePreviewUI();
        }

        if (phoneUIController == null)
        {
            phoneUIController = FindFirstObjectByType<PhoneUIController>();
        }
    }

    void LateUpdate()
    {
        if ((panelScript != null && panelScript.IsPhoneOpen) ||
            (phoneUIController != null && phoneUIController.IsOpen))
        {
            return;
        }

        //UI_text.text = "Last Photo Score: " + photoScore.ToString() + "\nGhost Hit: " + ghostScore.ToString();
        UI_text.text = "Last Photo Score: " + ghostScore.ToString();

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



    public int TakePhoto()
    {
        if (photoCamera.targetTexture != photort)
        {
            photoCamera.targetTexture = photort;
        }
        photoCamera.Render();
        Debug.Log("Screenshot captured");
        apertureFx?.PlayShutter();
        cameraAudio.PlayOneShot(shutter);

        Texture2D capturedPhoto = CapturePhotoTexture();
        if (phoneUIController == null)
        {
            phoneUIController = FindFirstObjectByType<PhoneUIController>();
        }

        if (capturedPhoto != null && phoneUIController != null)
        {
            phoneUIController.AddPhoto(capturedPhoto);
        }


        int score = 0;

        int step = Screen.width / 30;
        Debug.Log(step);
        Debug.Log(Screen.width);
        Debug.Log(Screen.height);

        // needs to be recalculated for new camera position
        Vector3 pos = new Vector3(0, 0, 0);
        Vector3 center = new Vector3(Screen.width, Screen.height, 0);
        float radius = Screen.width / 4;
        int pixel = 0;
        Ray ray;
        RaycastHit hit;

        while (pos.x < (Screen.width * 2) && pos.y < (Screen.height * 2))
        {

            ray = photoCamera.ScreenPointToRay(pos);
            if (Physics.Raycast(ray, out hit, 1000f, layerMask: ~ignoreLayers))
            {

                if (hit.collider.gameObject.layer == 6)
                {
                    score = score + 1;

                    if (hit.collider.gameObject.TryGetComponent<ballscript>(out ballscript ball))
                    {
                        if (ball.spinning)
                        {
                            score = score + 2;
                        }
                    }

                }
            }

            if ((pos - center).magnitude < radius)
            {
                //Debug.Log("centerhit");
                pixel = pixel + (step / 3);
                pos.x = pixel % (Screen.width * 2);
                pos.y = step * (pixel / (Screen.width * 2));
            }
            else
            {
                pixel = pixel + step;
                pos.x = pixel % (Screen.width * 2);
                pos.y = step * (pixel / (Screen.width * 2));
            }

        }


        //RaycastHit hit;

        //if (Physics.Raycast(ray, out hit))
        //{
        //    if (hit.collider.gameObject.layer == 6)
        //    {
        //        ghostHit = 1;
        //    }
        //    else
        //    {
        //        ghostHit = 2;
        //    }
        //}
        //else
        //{
        //    ghostHit = 3;
        //}
        if (photoPreview != null)
        {
            if (previewRoutine != null)
            {

                StopCoroutine(previewRoutine);
            }
            previewRoutine = StartCoroutine(ShowPreview());
        }

        ghostScore = score;

        return score;
    }

    public void SavePhoto()
    {
        SaveTextureToFileUtility.SaveRenderTextureToFile(photort, Application.dataPath + "/Screenshots/screenshot.png");

        Texture2D screenshot = CapturePhotoTexture();

        if (panelScript != null)
        {
            Debug.Log("Calling DisplayScreenshot");
            panelScript.DisplayScreenshot(screenshot);
        }
        else
        {
            Debug.LogError("panelScript is null! Assign it in the inspector.");
        }
    }

    private Texture2D CapturePhotoTexture()
    {
        if (photort == null)
        {
            Debug.LogError("photort is null! Assign the RenderTexture in the inspector.");
            return null;
        }

        Texture2D screenshot = new Texture2D(photort.width, photort.height, TextureFormat.RGB24, false);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = photort;
        screenshot.ReadPixels(new Rect(0, 0, photort.width, photort.height), 0, 0);
        screenshot.Apply();
        RenderTexture.active = previous;
        return screenshot;
    }

    private IEnumerator ShowPreview()
    {
        EnsurePreviewTexture();
        var previous = RenderTexture.active;
        RenderTexture.active = photort;
        previewTexture.ReadPixels(new Rect(0, 0, photort.width, photort.height), 0, 0);
        previewTexture.Apply();
        RenderTexture.active = previous;
        photoPreview.texture = previewTexture;
        photoPreview.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(previewDuration);
        photoPreview.gameObject.SetActive(false);
        previewRoutine = null;
    }

    private void EnsurePreviewTexture()
    {


        if (photort == null)
        {
            return;
        }

        if (previewTexture != null && previewTexture.width == photort.width && previewTexture.height == photort.height)
        {
            return;
        }

        if (previewTexture != null)
        {
            Destroy(previewTexture);
        }

        previewTexture = new Texture2D(photort.width, photort.height, TextureFormat.RGB24, false);
    }

    private void CreatePreviewUI()
    {
        var canvasGO = new GameObject("PhotoPreviewCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGO.GetComponent<Canvas>();

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        var scaler = canvasGO.GetComponent<CanvasScaler>();

        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        var previewGO = new GameObject("PhotoPreview", typeof(RawImage));
        previewGO.transform.SetParent(canvasGO.transform, false);

        photoPreview = previewGO.GetComponent<RawImage>();

        var rect = photoPreview.rectTransform;
        rect.anchorMin = new Vector2(1f, 0f);
        rect.anchorMax = new Vector2(1f, 0f);
        rect.pivot = new Vector2(1f, 0f);
        rect.sizeDelta = new Vector2(256f, 144f);
        rect.anchoredPosition = new Vector2(-140f, 100f);

        photoPreview.texture = null;
        photoPreview.gameObject.SetActive(false);
    }
}
