// using UnityEngine;
// using UnityEngine.InputSystem;
// using UnityEngine.UI;
// using System.Collections;
// using System;

// // for saving screenshots
// using System.IO;

// // for ui text
// using TMPro;



// // Camera Controller Monolith
// // functionality still needs to be split into other components
// public class CameraControllerMonolith : MonoBehaviour
// {
//     [SerializeField] private Transform player;

//     [SerializeField] private ApertureShot apertureFx;
//     // audioSource;



//     public AudioClip shutter;
//     AudioSource cameraAudio;

//     public Camera playerCamera;
//     public Camera photoCamera;
//     public RenderTexture photort;


//     [Header("Camera Mesh")]
//     [SerializeField] private GameObject cameraMeshPrefab;
//     [SerializeField] private Vector3 cameraMeshLocalPosition = new Vector3(0.25f, -0.15f, 0.4f);
//     [SerializeField] private Vector3 cameraMeshLocalEuler = new Vector3(0f, 180f, 0f);
//     private GameObject cameraMeshInstance;


//     [Header("Photo Preview")]
//     [SerializeField] private RawImage photoPreview;
//     [SerializeField] private float previewDuration = 1f;
//     private Coroutine previewRoutine;
//     private Texture2D previewTexture;

//     public PanelScript panelScript;
//     [SerializeField] private PhoneUIController phoneUIController;

//     [Header("Camera Pull-Up Animation")]
//     [SerializeField] private Animator cameraPullUpAnimator;
//     [SerializeField] private string cameraUpTrigger = "CameraUp";
//     private bool _zoomWasHeldLastFrame = false;




//     InputAction saveAction;
//     InputAction ghostAction;

//     bool ghostsOn = false;


//     public int photoScore = 0;
//     public GameObject scoreui;

//     public GameObject photopreviewui;
//     public TMP_Text UI_text;


//     int ghostScore = 0;

//     public LayerMask ignoreLayers;


//     [SerializeField] private GameObject t1_border;
//     [SerializeField] private GameObject t2_border;
//     [SerializeField] private GameObject t3_border;
//     [SerializeField] private GameObject heartsprite;

//     [SerializeField] private GameObject emptyheart;
//     [SerializeField] private GameObject heartCanvas;
//     [SerializeField] private GameObject backplate;

//     [Header("Wwise Events")]
//     [SerializeField] private AK.Wwise.Event playCameraCapture;
//     [SerializeField] private AK.Wwise.Event playCamerViewOpen;
//     [SerializeField] private AK.Wwise.Event playCameraViewClose;
//     [SerializeField] private AK.Wwise.Event playMusic;
//     [SerializeField] private AK.Wwise.RTPC CameraViewRTPC;
//     [SerializeField] private AK.Wwise.Event playAmbience1;
 
//     [Header("Playtest Logging")]
//     [SerializeField] private bool enablePlaytestLogs = true;



//     void Start()
//     {
//         playMusic.Post(gameObject);
//         playAmbience1.Post(gameObject);
//         cameraAudio = GetComponent<AudioSource>();
//         Cursor.lockState = CursorLockMode.Locked;
//         Cursor.visible = false;

//         saveAction = InputSystem.actions.FindAction("Interact");
//         ghostAction = InputSystem.actions.FindAction("GhostDbg");



//         if (cameraMeshPrefab != null && playerCamera != null)
//         {
//             cameraMeshInstance = Instantiate(cameraMeshPrefab, playerCamera.transform);
//             cameraMeshInstance.transform.localPosition = cameraMeshLocalPosition;
//             cameraMeshInstance.transform.localEulerAngles = cameraMeshLocalEuler;
//             if (cameraPullUpAnimator == null)
//                 cameraPullUpAnimator = cameraMeshInstance.GetComponentInChildren<Animator>();
//         }

//         if (photoPreview != null)
//         {
//             photoPreview.texture = null;
//             photoPreview.gameObject.SetActive(false);
//         }
//         else
//         {
//             CreatePreviewUI();
//         }

//         if (phoneUIController == null)
//         {
//             phoneUIController = FindFirstObjectByType<PhoneUIController>();
//         }
//     }

//     void LateUpdate()
//     {
//         if ((panelScript != null && panelScript.IsPhoneOpen) ||
//             (phoneUIController != null && phoneUIController.IsOpen))
//         {
//             return;
//         }

//         //UI_text.text = "Last Photo Score: " + photoScore.ToString() + "\nGhost Hit: " + ghostScore.ToString();
//         UI_text.text = "Last Photo Score: " + ghostScore.ToString();

//         if (saveAction.WasPressedThisFrame())
//         {
//             SavePhoto();
//         }

//         if (ghostAction.WasPressedThisFrame())
//         {
//             if (!ghostsOn)
//             {
//                 playerCamera.cullingMask = 127;
//                 ghostsOn = true;
//             }
//             else
//             {
//                 playerCamera.cullingMask = 63;
//                 ghostsOn = false;
//             }
//         }



//         // _zoomWasHeldLastFrame = zooming;

//         // //for wwise cameraView events
//         // bool zoomingOn = zoomAction.WasPressedThisFrame();
//         // bool zoomingOff = zoomAction.WasReleasedThisFrame();
//         // if (zoomingOn)
//         // {
//         //     playCamerViewOpen.Post(gameObject);
//         //     CameraViewRTPC.SetGlobalValue(1f);

//         // }
//         // if (zoomingOff)
//         // {
//         //     playCameraViewClose.Post(gameObject);
//         //     CameraViewRTPC.SetGlobalValue(0f);
//         // }


//     }


//     public int TakePhoto()
//     {
//         //Debug.Log("CAMERA PIXELHEIGHT: " + photoCamera.pixelHeight);
//         //Debug.Log("CAMERA PIXELWIDTH: " + photoCamera.pixelWidth);

//         if (photoCamera.targetTexture != photort)
//         {
//             photoCamera.targetTexture = photort;
//         }
//         photoCamera.Render();
//         LogPlaytest("Screenshot captured to RenderTexture");
//         playCameraCapture.Post(gameObject);
//         apertureFx?.PlayShutter();
//         cameraAudio.PlayOneShot(shutter);

//         Texture2D capturedPhoto = CapturePhotoTexture();
//         if (phoneUIController == null)
//         {
//             phoneUIController = FindFirstObjectByType<PhoneUIController>();
//         }

//         if (capturedPhoto != null && phoneUIController != null)
//         {
//             phoneUIController.AddPhoto(capturedPhoto);
//         }



//         ghostScore = score;
//         LogPlaytest($"Photo scored. ghostScore={ghostScore}");

//         if (photoPreview != null)
//         {
//             if (previewRoutine != null)
//             {

//                 StopCoroutine(previewRoutine);
//             }
//             previewRoutine = StartCoroutine(ShowPreview());
//         }

        

//         return score;
//     }

//     public void SavePhoto()
//     {
//         SaveTextureToFileUtility.SaveRenderTextureToFile(photort, Application.dataPath + "/Screenshots/screenshot.png");

//         Texture2D screenshot = CapturePhotoTexture();

//         if (panelScript != null)
//         {
//             LogPlaytest("Saving screenshot and pushing to panel UI.");
//             panelScript.DisplayScreenshot(screenshot);
//         }
//         else
//         {
//             Debug.LogError("panelScript is null! Assign it in the inspector.");
//         }
//     }

//     private Texture2D CapturePhotoTexture()
//     {
//         if (photort == null)
//         {
//             Debug.LogError("photort is null! Assign the RenderTexture in the inspector.");
//             return null;
//         }

//         Texture2D screenshot = new Texture2D(photort.width, photort.height, TextureFormat.RGB24, false);
//         RenderTexture previous = RenderTexture.active;
//         RenderTexture.active = photort;
//         screenshot.ReadPixels(new Rect(0, 0, photort.width, photort.height), 0, 0);
//         screenshot.Apply();
//         RenderTexture.active = previous;
//         return screenshot;
//     }

//     private IEnumerator ShowPreview()
//     {
//         //Debug.Log("PHOTO RT HEIGHT: " + photort.height);
//         //Debug.Log("PHOTO RT WIDTH: " + photort.width);
//         EnsurePreviewTexture();
//         var previous = RenderTexture.active;
//         RenderTexture.active = photort;
//         previewTexture.ReadPixels(new Rect(0, 0, photort.width, photort.height), 0, 0);
//         previewTexture.Apply();
//         RenderTexture.active = previous;
//         photoPreview.texture = previewTexture;

//         photoPreview.gameObject.SetActive(true);

//         generateHearts();
//         // need to simplify this code
//         if (ghostScore > 200) { t1_border.SetActive(false); t2_border.SetActive(false); t3_border.SetActive(true); }
//         else if (ghostScore > 100) { t1_border.SetActive(false); t2_border.SetActive(true); t3_border.SetActive(false); }
//         else if (ghostScore > 50) { t1_border.SetActive(true); t2_border.SetActive(false); t3_border.SetActive(false); }
//         else { t1_border.SetActive(false); t2_border.SetActive(false); t3_border.SetActive(false); }
//         backplate.SetActive(true);

//         yield return new WaitForSecondsRealtime(previewDuration);

//         photoPreview.gameObject.SetActive(false);
//         t1_border.SetActive(false);
//         t2_border.SetActive(false);
//         t3_border.SetActive(false);
//         backplate.SetActive(false);

//         previewRoutine = null;
//     }

//     private void LogPlaytest(string message)
//     {
//         if (!enablePlaytestLogs && !PlaytestLogWriter.RuntimeLoggingEnabled)
//         {
//             return;
//         }

//         PlaytestLogWriter.Log("CameraController", message);
//     }

//     private Tuple<float, float> generatePoint()
//     {
//         float x1 = UnityEngine.Random.Range(-900f, -700f);
//         float x2 = UnityEngine.Random.Range(700f, 900f);
//         RectTransform canvasRect = heartCanvas.GetComponent<RectTransform>();
//         float x;
//         if (UnityEngine.Random.Range(0f, 1f) < 0.5f) {
//             x = x1;
//         } else {
//             x = x2;
//         }
//         float y = UnityEngine.Random.Range(-canvasRect.rect.height / 3f, canvasRect.rect.height / 3f);
//         return new Tuple<float, float>(x, y);
//     }

//     private void generateHearts()
//     {
//         int count = 0;

//         if (ghostScore > 600) count = 3;
//         else if (ghostScore > 200) count = 2;
//         else if (ghostScore > 50) count = 1;

//         for (int i = 0; i < count; i++)
//         {
//             var p = generatePoint();
//             GameObject heart = Instantiate(heartsprite, emptyheart.transform, false);
//             RectTransform rt = heart.GetComponent<RectTransform>();

//             rt.anchoredPosition = new Vector2(p.Item1, p.Item2);
//             rt.localPosition = new Vector3(rt.localPosition.x, rt.localPosition.y, 0f);
//             //rt.position.z = 0;

//             heart.transform.localScale = Vector3.one;
//             heart.transform.SetAsLastSibling();
//         }
//     }

//     private void EnsurePreviewTexture()
//     {


//         if (photort == null)
//         {
//             return;
//         }

//         if (previewTexture != null && previewTexture.width == photort.width && previewTexture.height == photort.height)
//         {
//             return;
//         }

//         if (previewTexture != null)
//         {
//             Destroy(previewTexture);
//         }

//         previewTexture = new Texture2D(photort.width, photort.height, TextureFormat.RGB24, false);
//     }

//     private void CreatePreviewUI()
//     {
//         var canvasGO = new GameObject("PhotoPreviewCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
//         var canvas = canvasGO.GetComponent<Canvas>();

//         canvasGO.transform.SetParent(photopreviewui.transform, false);

//         canvas.renderMode = RenderMode.ScreenSpaceOverlay;

//         // want hearts appearing ontop
//         canvas.overrideSorting = true; // this canvas is a child of photopreviewui for now, so we need this
//         canvas.sortingOrder = 5;


//         var scaler = canvasGO.GetComponent<CanvasScaler>();

//         scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
//         scaler.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.Expand;
//         scaler.referenceResolution = new Vector2(1920f, 1080f);

//         var previewGO = new GameObject("PhotoPreview", typeof(RawImage));
//         previewGO.transform.SetParent(canvasGO.transform, false);

//         photoPreview = previewGO.GetComponent<RawImage>();

//         var rect = photoPreview.rectTransform;
//         rect.anchorMin = new Vector2(0.5f, 0.5f);
//         rect.anchorMax = new Vector2(0.5f, 0.5f);
//         rect.pivot = new Vector2(0.5f, 0.5f);
//         rect.sizeDelta = new Vector2(1350f * 21/32, 1080f * 21/32); // 5:4 resolution, but scaled smaller
//         rect.anchoredPosition = new Vector2(0, 0);

//         photoPreview.texture = null;
//         photoPreview.gameObject.SetActive(false);
//     }
// }
