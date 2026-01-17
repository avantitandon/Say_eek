using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;
    
    [Header("Camera Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 5f, -10f);
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private bool lookAtTarget = true;

    void Start()
    {
        if (target == null)
        {
            // Try to find the ghost if not assigned
            GameObject ghost = GameObject.FindGameObjectWithTag("Player");
            if (ghost != null)
            {
                target = ghost.transform;
            }
        }
    }

    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        // Smoothly move camera to follow target
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Optionally look at the target
        if (lookAtTarget)
        {
            transform.LookAt(target);
        }
    }
}
