using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using System.Collections.Generic;

public class PhoneUIController : MonoBehaviour
{
    [Header("Behavior")]
    [SerializeField] private bool pauseGameWhenOpen = true;
    // gamepad toggling
    [SerializeField] private bool allowControllerToggle = true;
    [SerializeField] private string controllerToggleActionName = "PhoneToggle";

    [Header("UI")]
    // on screen button? unsure if we need but there if we do
    [SerializeField] private bool showOpenButton = false;
    [SerializeField] private string openButtonLabel = "Phone";
    [SerializeField] private string titleText = "Photo Library";
    // memory cap -> change to 15/25/30/ whatever 
    [SerializeField] private int maxStoredPhotos = 30;

    private Func<bool> canOpenPredicate;
    private Action<bool> onPhoneToggled;

    private Canvas rootCanvas;
    private GameObject phonePanel;
    private Button openButton;
    private bool initialized;

    private float previousTimeScale = 1f;
    private bool timeScaleOverridden;
    private RectTransform galleryContent;
    private Text emptyText;
    // Stored the photos here, destroy end
    private readonly List<Texture2D> storedPhotos = new List<Texture2D>();
    // each photo needs a ui card-> store here
    private readonly List<GameObject> galleryEntries = new List<GameObject>();
    private InputAction controllerToggleAction;

    public bool IsOpen => phonePanel != null && phonePanel.activeSelf;

    public void Initialize(Func<bool> canOpen, Action<bool> onToggle = null)
    {


        canOpenPredicate = canOpen;
        onPhoneToggled = onToggle;

        if (!initialized)
        {
            
        // all ui elements build
            BuildUi();
            initialized = true;
        }

        SetPhoneOpen(false);
        RefreshButtonVisibility();
    }

    private void Start()
    {

        // toggles
        if (!string.IsNullOrWhiteSpace(controllerToggleActionName))
        {
            controllerToggleAction = InputSystem.actions.FindAction(controllerToggleActionName);
        }

        if (!initialized)
        {
            Initialize(() => true, null);
        }
    }

    private void Update()
    {
        RefreshButtonVisibility();

        bool togglePressed = false;

        if (Keyboard.current != null)
        {
            //  I'm not happy w this, but unsure about alternatives
            togglePressed = Keyboard.current.tabKey.wasPressedThisFrame;

            if (IsOpen && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                // Keep Esc as a hard close shortcut.
                ClosePhone();
                return;
            }
        }

        if (!togglePressed && allowControllerToggle)
        {
            if (controllerToggleAction != null)
            {
                // Preferred path: dedicated input action.
                togglePressed = controllerToggleAction.WasPressedThisFrame();
            }
            else if (Gamepad.current != null)
            {
                // Fallback for projects that haven't added a custom action yet.
                togglePressed = Gamepad.current.startButton.wasPressedThisFrame ||
                                Gamepad.current.selectButton.wasPressedThisFrame;
            }
        }

        if (!togglePressed)
        {
            return;
        }

        if (IsOpen)
        {
            ClosePhone();
        }
        else
        {
            OpenPhone();
        }
    }

    public void OpenPhone()
    {
        if (IsOpen)
        {
            return;
        }

        if (canOpenPredicate != null && !canOpenPredicate())
        {
            return;
        }

        SetPhoneOpen(true);
    }

    public void ClosePhone()
    {
        if (!IsOpen)
        {
            return;
        }

        SetPhoneOpen(false);
    }

    public void AddPhoto(Texture2D photo)
    {
        if (photo == null)
        {
            return;
        }

        if (!initialized)
        {
            Initialize(() => true, null);
        }

        // Append photo and creates ui card
        storedPhotos.Add(photo);
        CreateGalleryEntry(photo, storedPhotos.Count);

        // right now it just deletes old photos. Will change to max out/ stop gameplay
        while (storedPhotos.Count > maxStoredPhotos && storedPhotos.Count > 0)
        {
            RemoveOldestPhoto();
        }

        UpdateEmptyState();
        ScrollToBottom();
    }

    private void SetPhoneOpen(bool isOpen)
    {
        if (phonePanel != null)
        {
            phonePanel.SetActive(isOpen);
        }

        if (isOpen)
        {
            // i'm not sure how to make this not cursor wise??
            // will need to change input state accordingly-> controller fuck fuck 
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (pauseGameWhenOpen && !Mathf.Approximately(Time.timeScale, 0f))
            {
                // Preserve previous timescale so resume is exact.
                previousTimeScale = Time.timeScale;
                Time.timeScale = 0f;
                timeScaleOverridden = true;
            }
        }
        else
        {
            // Closing the phone returns control to gameplay camera/movement.
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (timeScaleOverridden)
            {
                Time.timeScale = previousTimeScale;
                timeScaleOverridden = false;
            }
        }

        onPhoneToggled?.Invoke(isOpen);
    }

    private void RefreshButtonVisibility()
    {
        if (openButton == null)
        {
            return;
        }
        bool canOpenNow = canOpenPredicate == null || canOpenPredicate();
        openButton.gameObject.SetActive(canOpenNow && !IsOpen);
    }

    private void BuildUi()
    {
        // Phone UI needs an EventSystem to receive clicks/scroll.
        EnsureEventSystemExists();

        var canvasGO = new GameObject("PhoneUICanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        rootCanvas = canvasGO.GetComponent<Canvas>();
        rootCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        rootCanvas.sortingOrder = 1100;

        var scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        if (showOpenButton)
        {
            BuildOpenButton(canvasGO.transform);
        }
        // Main phone window and gallery content.
        BuildPhonePanel(canvasGO.transform);
    }

    private void BuildOpenButton(Transform parent)
    {
        var buttonGO = new GameObject("OpenPhoneButton", typeof(Image), typeof(Button));
        buttonGO.transform.SetParent(parent, false);

        var rect = buttonGO.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1f, 0f);
        rect.anchorMax = new Vector2(1f, 0f);
        rect.pivot = new Vector2(1f, 0f);
        rect.sizeDelta = new Vector2(180f, 64f);
        rect.anchoredPosition = new Vector2(-24f, 24f);

        var img = buttonGO.GetComponent<Image>();
        img.color = new Color(0.15f, 0.15f, 0.15f, 0.9f);

        openButton = buttonGO.GetComponent<Button>();
        openButton.targetGraphic = img;
        openButton.onClick.AddListener(OpenPhone);

        CreateLabel(buttonGO.transform, openButtonLabel, 24, TextAnchor.MiddleCenter);
    }

    private void BuildPhonePanel(Transform parent)
    {
        // Dark panel representing the phone body.
        phonePanel = new GameObject("PhonePanel", typeof(Image));
        phonePanel.transform.SetParent(parent, false);

        var panelRect = phonePanel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(520f, 860f);
        panelRect.anchoredPosition = Vector2.zero;

        var panelImg = phonePanel.GetComponent<Image>();
        panelImg.color = new Color(0.05f, 0.06f, 0.08f, 0.97f);

        var titleGO = new GameObject("PhoneTitle", typeof(Text));
        titleGO.transform.SetParent(phonePanel.transform, false);
        var titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.sizeDelta = new Vector2(0f, 90f);
        titleRect.anchoredPosition = new Vector2(0f, -16f);
        var title = titleGO.GetComponent<Text>();
        title.text = titleText;
        title.font = GetDefaultFont();
        title.fontSize = 34;
        title.color = Color.white;
        title.alignment = TextAnchor.MiddleCenter;

        BuildGallery(phonePanel.transform);

        var closeButtonGO = new GameObject("ClosePhoneButton", typeof(Image), typeof(Button));
        closeButtonGO.transform.SetParent(phonePanel.transform, false);
        var closeRect = closeButtonGO.GetComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(1f, 1f);
        closeRect.anchorMax = new Vector2(1f, 1f);
        closeRect.pivot = new Vector2(1f, 1f);
        closeRect.sizeDelta = new Vector2(96f, 56f);
        closeRect.anchoredPosition = new Vector2(-16f, -16f);

        var closeImg = closeButtonGO.GetComponent<Image>();
        closeImg.color = new Color(0.8f, 0.2f, 0.2f, 0.95f);
        var closeButton = closeButtonGO.GetComponent<Button>();
        closeButton.targetGraphic = closeImg;
        closeButton.onClick.AddListener(ClosePhone);

        CreateLabel(closeButtonGO.transform, "Close", 20, TextAnchor.MiddleCenter);

        phonePanel.SetActive(false);
    }

    private void BuildGallery(Transform parent)
    {
        // Scroll container for all captured photos.
        var scrollRoot = new GameObject("GalleryScrollRoot", typeof(Image), typeof(ScrollRect));
        scrollRoot.transform.SetParent(parent, false);
        var scrollRectTransform = scrollRoot.GetComponent<RectTransform>();
        scrollRectTransform.anchorMin = new Vector2(0f, 0f);
        scrollRectTransform.anchorMax = new Vector2(1f, 1f);
        scrollRectTransform.offsetMin = new Vector2(24f, 24f);
        scrollRectTransform.offsetMax = new Vector2(-24f, -104f);

        var scrollRootImage = scrollRoot.GetComponent<Image>();
        scrollRootImage.color = new Color(0.1f, 0.12f, 0.15f, 0.85f);

        var viewport = new GameObject("Viewport", typeof(Image), typeof(Mask));
        viewport.transform.SetParent(scrollRoot.transform, false);
        var viewportRect = viewport.GetComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = new Vector2(10f, 10f);
        viewportRect.offsetMax = new Vector2(-10f, -10f);
        var viewportImage = viewport.GetComponent<Image>();
        viewportImage.color = new Color(0f, 0f, 0f, 0.08f);
        viewport.GetComponent<Mask>().showMaskGraphic = false;

        var content = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        content.transform.SetParent(viewport.transform, false);
        galleryContent = content.GetComponent<RectTransform>();
        galleryContent.anchorMin = new Vector2(0f, 1f);
        galleryContent.anchorMax = new Vector2(1f, 1f);
        galleryContent.pivot = new Vector2(0.5f, 1f);
        galleryContent.anchoredPosition = Vector2.zero;
        galleryContent.sizeDelta = new Vector2(0f, 0f);

        var layout = content.GetComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(8, 8, 8, 8);
        layout.spacing = 10f;
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;

        var fitter = content.GetComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var scrollRect = scrollRoot.GetComponent<ScrollRect>();
        scrollRect.viewport = viewportRect;
        scrollRect.content = galleryContent;
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.scrollSensitivity = 28f;

        var emptyGO = new GameObject("EmptyState", typeof(Text));
        emptyGO.transform.SetParent(viewport.transform, false);
        var emptyRect = emptyGO.GetComponent<RectTransform>();
        emptyRect.anchorMin = Vector2.zero;
        emptyRect.anchorMax = Vector2.one;
        emptyRect.offsetMin = new Vector2(20f, 20f);
        emptyRect.offsetMax = new Vector2(-20f, -20f);
        emptyText = emptyGO.GetComponent<Text>();
        emptyText.text = "No photos yet. Take pictures, then open phone and scroll.";
        emptyText.font = GetDefaultFont();
        emptyText.fontSize = 22;
        emptyText.color = new Color(0.82f, 0.82f, 0.82f, 1f);
        emptyText.alignment = TextAnchor.MiddleCenter;
        UpdateEmptyState();
    }

    private void CreateGalleryEntry(Texture2D photo, int photoIndex)
    {
        if (galleryContent == null)
        {
            return;
        }

        // one photo card
        var card = new GameObject($"Photo_{photoIndex}", typeof(Image), typeof(LayoutElement));
        card.transform.SetParent(galleryContent, false);
        var cardImage = card.GetComponent<Image>();
        cardImage.color = new Color(0.17f, 0.19f, 0.23f, 0.95f);
        var layout = card.GetComponent<LayoutElement>();
        layout.preferredHeight = 270f;



        var indexLabel = new GameObject("Index", typeof(Text));
        indexLabel.transform.SetParent(card.transform, false);
        var labelRect = indexLabel.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0f, 1f);
        labelRect.anchorMax = new Vector2(1f, 1f);
        labelRect.pivot = new Vector2(0.5f, 1f);
        labelRect.sizeDelta = new Vector2(0f, 34f);
        labelRect.anchoredPosition = new Vector2(0f, -10f);
        var label = indexLabel.GetComponent<Text>();
        label.text = $"Photo {photoIndex}";
        label.font = GetDefaultFont();
        label.fontSize = 18;
        label.color = Color.white;
        label.alignment = TextAnchor.MiddleCenter;

        var imageGO = new GameObject("Image", typeof(RawImage), typeof(AspectRatioFitter));
        imageGO.transform.SetParent(card.transform, false);
        var imageRect = imageGO.GetComponent<RectTransform>();
        imageRect.anchorMin = new Vector2(0f, 0f);
        imageRect.anchorMax = new Vector2(1f, 1f);
        imageRect.offsetMin = new Vector2(14f, 14f);
        imageRect.offsetMax = new Vector2(-14f, -52f);
        var rawImage = imageGO.GetComponent<RawImage>();
        rawImage.texture = photo;
        rawImage.color = Color.white;

        var aspect = imageGO.GetComponent<AspectRatioFitter>();
        aspect.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
        aspect.aspectRatio = photo.height > 0 ? (float)photo.width / photo.height : 16f / 9f;

        galleryEntries.Add(card);
    }

    private void RemoveOldestPhoto()
    {
        if (storedPhotos.Count == 0)
        {
            return;
        }

        // Delete texture + UI card together to avoid leaks and index drift.
        var oldest = storedPhotos[0];
        storedPhotos.RemoveAt(0);
        if (oldest != null)
        {
            Destroy(oldest);
        }

        if (galleryEntries.Count > 0)
        {
            var entry = galleryEntries[0];
            galleryEntries.RemoveAt(0);
            if (entry != null)
            {
                Destroy(entry);
            }
        }
    }

    private void UpdateEmptyState()
    {
        if (emptyText != null)
        {
            // Empty message only when no photos exist.
            emptyText.gameObject.SetActive(storedPhotos.Count == 0);
        }
    }

    private void ScrollToBottom()
    {
        if (galleryContent == null)
        {
            return;
        }

        // Force layout before changing normalized position.
        Canvas.ForceUpdateCanvases();
        var scrollRect = galleryContent.GetComponentInParent<ScrollRect>();
        if (scrollRect != null)
        {
            // Newest photos are appended at the bottom.
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }

    private static void CreateLabel(Transform parent, string textValue, int fontSize, TextAnchor alignment)
    {
        var labelGO = new GameObject("Label", typeof(Text));
        labelGO.transform.SetParent(parent, false);

        var rect = labelGO.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        var text = labelGO.GetComponent<Text>();
        text.text = textValue;
        text.font = GetDefaultFont();
        text.fontSize = fontSize;
        text.color = Color.white;
        text.alignment = alignment;
    }

    private static Font GetDefaultFont()
    {
        var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font != null)
        {
            return font;
        }

        return Resources.GetBuiltinResource<Font>("Arial.ttf");
    }

    private static void EnsureEventSystemExists()
    {
        if (FindFirstObjectByType<EventSystem>() != null)
        {
            return;
        }

        // Create one runtime EventSystem if the scene doesn't have one.
        var eventSystemGO = new GameObject("EventSystem", typeof(EventSystem));

        if (Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem") != null)
        {
            eventSystemGO.AddComponent<InputSystemUIInputModule>();
        }
        else
        {
            eventSystemGO.AddComponent<StandaloneInputModule>();
        }
    }
}
