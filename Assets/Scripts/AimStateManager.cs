using UnityEngine;


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
    AudioSource camera;

    bool photoready = false;
    float phototime;


    void Start()
    {
        body1.SetActive(true);
        body2.SetActive(true);
        camera = GetComponent<AudioSource>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void LateUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        if (Input.GetButtonDown("Fire1")) {
            Debug.LogError("Screenshot captured");
            camera.PlayOneShot(shutter);
            ScreenCapture.CaptureScreenshot("Assets/Screenshots/screenshot.png");
            body1.SetActive(true);
            body2.SetActive(true);
        }
        else {
            body1.SetActive(false);
            body2.SetActive(false);
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
}







