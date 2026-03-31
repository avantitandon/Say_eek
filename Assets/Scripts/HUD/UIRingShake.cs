using UnityEngine;

public class UIRingShake : MonoBehaviour
{
    [SerializeField] private RectTransform target;
    [SerializeField] private float amplitude = 8f;
    [SerializeField] private float speed = 30f;
    [SerializeField] private float rotationAmplitude = 3f;
    [SerializeField] private bool playOnEnable = true;

    private Vector2 startAnchoredPosition;
    private Vector3 startEulerAngles;
    private bool isRinging;

    void Awake()
    {
        if (target == null)
        {
            target = GetComponent<RectTransform>();
        }

        CacheRestingTransform();
    }

    void OnEnable()
    {
        CacheRestingTransform();
        isRinging = playOnEnable;
        ApplyRestingTransform();
    }

    void OnDisable()
    {
        isRinging = false;
        ApplyRestingTransform();
    }

    void Update()
    {
        if (target == null)
        {
            return;
        }

        if (!isRinging)
        {
            ApplyRestingTransform();
            return;
        }

        float time = Time.unscaledTime * speed;
        float xOffset = Mathf.Sin(time) * amplitude;
        float rotationZ = Mathf.Sin(time * 0.9f) * rotationAmplitude;

        target.anchoredPosition = startAnchoredPosition + new Vector2(xOffset, 0f);
        target.localEulerAngles = startEulerAngles + new Vector3(0f, 0f, rotationZ);
    }

    public void SetRinging(bool ringing)
    {
        isRinging = ringing;

        if (!isRinging)
        {
            ApplyRestingTransform();
        }
    }

    private void CacheRestingTransform()
    {
        if (target == null)
        {
            return;
        }

        startAnchoredPosition = target.anchoredPosition;
        startEulerAngles = target.localEulerAngles;
    }

    private void ApplyRestingTransform()
    {
        if (target == null)
        {
            return;
        }

        target.anchoredPosition = startAnchoredPosition;
        target.localEulerAngles = startEulerAngles;
    }
}
