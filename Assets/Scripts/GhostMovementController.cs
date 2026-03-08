using UnityEngine;

public class GhostMovementController : MonoBehaviour
{
    // Varibles, just randon
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float roamRadius = 4f;
    [SerializeField] private float arrivalDistance = 0.2f;
    [SerializeField] private float minRetargetTime = 1.2f;
    [SerializeField] private float maxRetargetTime = 3.2f;
    [SerializeField] private float turnSpeed = 6f;
    [SerializeField] private float bobAmplitude = 0.1f;
    [SerializeField] private float bobFrequency = 1.6f;
    [Header("Grid/Ground Constraint")]


    [SerializeField] private LayerMask groundMask = ~0;
    [SerializeField] private float groundProbeHeight = 3f;
    [SerializeField] private float groundProbeDistance = 10f;
    [SerializeField] private int maxTargetPickAttempts = 20;

    private Vector3 _spawnPosition;
    private Vector3 _targetPosition;
    private float _baseY;
    private float _nextRetargetAt;
    private float _bobOffset;

    private void Awake()
    {
        // Cache spawn/base values, so we don't wander offf
        _spawnPosition = transform.position;
        _baseY = transform.position.y;
        _bobOffset = Random.Range(0f, 1000f);
        PickNewTarget();
    }

    private void Update()
    {
        // Pick a new roam point on a timer or after arriving at the current point.
        if (Time.time >= _nextRetargetAt || IsAtTarget())
        {
            PickNewTarget();
        }

        Vector3 position = transform.position;
        Vector3 targetOnPlane = new Vector3(_targetPosition.x, position.y, _targetPosition.z);
        Vector3 delta = targetOnPlane - position;
        float distance = delta.magnitude;

        if (distance > 0.001f)
        {
            Vector3 direction = delta / distance;
            float moveStep = moveSpeed * Time.deltaTime;
            Vector3 nextPosition = position + direction * Mathf.Min(moveStep, distance);

            // ground has to be validdddd
            if (!IsOnValidGround(nextPosition))
            {
                PickNewTarget();
                return;
            }

            // Apply hovering bob while moving in XZ.
            // This isn't working for somereason 

            // never mind varianle issue
            nextPosition.y = _baseY + Mathf.Sin((Time.time + _bobOffset) * bobFrequency) * bobAmplitude;
            transform.position = nextPosition;

            // Rotate smoothly toward movement direction.
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }

    private bool IsAtTarget()
    {
        Vector2 flatSelf = new Vector2(transform.position.x, transform.position.z);
        Vector2 flatTarget = new Vector2(_targetPosition.x, _targetPosition.z);
        return Vector2.Distance(flatSelf, flatTarget) <= arrivalDistance;
    }

    private void PickNewTarget()
    {
        Vector3 chosen = _spawnPosition;
        bool found = false;

        // Random poinrs trails
        for (int i = 0; i < maxTargetPickAttempts; i++)
        {
            Vector2 offset = Random.insideUnitCircle * roamRadius;
            Vector3 candidate = new Vector3(_spawnPosition.x + offset.x, _baseY, _spawnPosition.z + offset.y);

            if (IsOnValidGround(candidate))
            {
                chosen = candidate;
                found = true;
                break;
            }
        }

        // If no valid point is found, hold near current position instead of drifting.
        _targetPosition = found ? chosen : new Vector3(transform.position.x, _baseY, transform.position.z);
        _nextRetargetAt = Time.time + Random.Range(minRetargetTime, maxRetargetTime);
    }

// just checking whether we on ground v. imp don't mess with this

    private bool IsOnValidGround(Vector3 worldPosition)
    {
        //vertical proble
        Vector3 origin = worldPosition + Vector3.up * groundProbeHeight;
        return Physics.Raycast(origin, Vector3.down, groundProbeDistance, groundMask, QueryTriggerInteraction.Ignore);
    }

}
