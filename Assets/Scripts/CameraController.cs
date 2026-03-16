using UnityEngine;

public class CameraController : MonoBehaviour
{
    // CONSTANTS //

    private const float CAM_HEIGHT_FROM_PLAYER = 6f;

    // AUDIO //

    // GAME COMPONENTS //

    [SerializeField] private CameraUpScript upScript;

    [SerializeField] private ZoomManager zoomManager;

    [SerializeField] private PhotoManager photoManager;

    // VARIABLES //
    [SerializeField] private float mouseSensitivity = 150f;
    [SerializeField] private float minY = -35f;
    [SerializeField] private float maxY = 60f;

    private float xRotation;
    private float yRotation;



    // LOGGING //


    public bool IsCameraUp()
    {
        return upScript.IsCameraUp();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // get the direction the camera object is facing
    public Vector3 GetCameraForward()
    {
        Vector3 forward = transform.forward;
        forward.y = 0;
        return forward.normalized;
    }
    // get the right (?) of the camera
    public Vector3 GetCameraRight()
    {
        Vector3 right = transform.right;
        right.y = 0;
        return right.normalized;
    }

    // call every frame to handle the direction of the camera
    public void Look(Vector2 lookValue, Vector3 playerPosition)
    {
        float mouseX = lookValue.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookValue.y * mouseSensitivity * Time.deltaTime;
        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minY, maxY);
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        transform.position = playerPosition + Vector3.up * CAM_HEIGHT_FROM_PLAYER;
    }

    // call every frame to hold the camera up
    public void Hold()
    {
        upScript.Hold();
    }

    // call every frame to let the camera fall
    public void Release()
    {
        upScript.Release();
    }
    
    // call every frame to zoom the camera in
    public void ZoomIn()
    {
        zoomManager.ZoomIn();
    }

    // call every frame to zoom the camera out
    public void ZoomOut()
    {
        zoomManager.ZoomOut();
    }

    // take a photo
    public (int, Texture2D) TakePhoto()
    {
        return photoManager.TakePhoto();
    }
}
