using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ApertureShot : MonoBehaviour
{
    [SerializeField] private Image overlay;

    [Header("Timing (increase if you want smoother)")]
    [SerializeField] private float closeTime = 0.12f;
    [SerializeField] private float holdTime  = 0.02f;
    [SerializeField] private float openTime  = 0.18f;

    [Header("Radius")]
    [SerializeField] private float openRadius = 1.2f;
    [SerializeField] private float closedRadius = 0.0f;

    [Header("Soft edge")]
    [SerializeField] private float openSoftness = 0.03f;
    [SerializeField] private float closedSoftness = 0.01f;

    [Header("Easing")]
    [SerializeField] private AnimationCurve closeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve openCurve  = AnimationCurve.EaseInOut(0, 0, 1, 1);

    Material mat;
    Coroutine co;

    int radiusID = -1;
    int softnessID = -1;
    int centerID = -1;

    void Awake()
    {
        if (!overlay || !overlay.material)
        {
            Debug.LogError("ApertureShotController: overlay or overlay.material missing.");
            enabled = false;
            return;
        }

        mat = Instantiate(overlay.material);
        overlay.material = mat;

        radiusID   = FirstProp(mat, "Radius", "_Radius");
        softnessID = FirstProp(mat, "Softness", "_Softness");
        centerID   = FirstProp(mat, "Center", "_Center");

        if (radiusID == -1)
        {
            Debug.LogError($"ApertureShotController: No Radius property found on {mat.shader.name}");
            enabled = false;
            return;
        }

        SetCenterUV(new Vector2(0.5f, 0.5f));
        SetRadius(openRadius);
        SetSoftness(openSoftness);

        overlay.enabled = false;
    }

    int FirstProp(Material m, params string[] names)
    {
        foreach (var n in names)
        {
            int id = Shader.PropertyToID(n);
            if (m.HasProperty(id)) return id;
        }
        return -1;
    }

    void SetRadius(float r) { mat.SetFloat(radiusID, r); }
    void SetSoftness(float s) { if (softnessID != -1) mat.SetFloat(softnessID, s); }
    void SetCenterUV(Vector2 uv) { if (centerID != -1) mat.SetVector(centerID, new Vector4(uv.x, uv.y, 0, 0)); }

    public void PlayShutter() => PlayShutterAt(null);

    public void PlayShutterAt(Vector2? screenPointPx)
    {
        Vector2 uv = screenPointPx.HasValue
            ? new Vector2(screenPointPx.Value.x / Screen.width, screenPointPx.Value.y / Screen.height)
            : new Vector2(0.5f, 0.5f);

        SetCenterUV(uv);

        if (co != null) StopCoroutine(co);
        co = StartCoroutine(Run());
    }

    IEnumerator Animate(float fromR, float toR, float fromS, float toS, float duration, AnimationCurve curve)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime; // per-frame
            float k = Mathf.Clamp01(t / duration);
            float e = Mathf.Clamp01(curve.Evaluate(k));

            SetRadius(Mathf.Lerp(fromR, toR, e));
            SetSoftness(Mathf.Lerp(fromS, toS, e));
            yield return null;
        }
        SetRadius(toR);
        SetSoftness(toS);
    }

    IEnumerator Run()
    {
        overlay.enabled = true;

        yield return Animate(openRadius, closedRadius, openSoftness, closedSoftness, closeTime, closeCurve);
        yield return new WaitForSecondsRealtime(holdTime);
        yield return Animate(closedRadius, openRadius, closedSoftness, openSoftness, openTime, openCurve);

        overlay.enabled = false;
        co = null;
    }

    [ContextMenu("TEST Shutter")]
    void TestShutter() => PlayShutter();
}
