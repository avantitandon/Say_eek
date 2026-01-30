using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

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

    [Header("Camera Mesh")]
    [SerializeField] private GameObject cameraMeshPrefab;
    [SerializeField] private Vector3 cameraMeshLocalPosition = new Vector3(0.25f, -0.15f, 0.4f);
    [SerializeField] private Vector3 cameraMeshLocalEuler = new Vector3(0f, 180f, 0f);
    private GameObject cameraMeshInstance;

    [Header("Photo Preview")]
    [SerializeField] private RawImage photoPreview;
    [SerializeField] private float previewDuration = 1f;
    private Coroutine previewRoutine;
    private Texture2D previewTexture;



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
        if (photoCamera.targetTexture != photort)
        {
            photoCamera.targetTexture = photort;
        }
        photoCamera.Render();
        Debug.Log("Screenshot captured");
        cameraAudio.PlayOneShot(shutter);

        if (photoPreview != null)
        {
            if (previewRoutine != null)
            {
                
                  StopCoroutine(previewRoutine);
            }
            previewRoutine = StartCoroutine(ShowPreview());
        }
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







