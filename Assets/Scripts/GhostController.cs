using UnityEngine;

public class GhostController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float floatAmount = 0.5f;

    private Vector3 startPosition;
    private float floatTimer;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        HandleMovement();
        HandleFloating();
    }

    private void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);
        transform.Translate(movement * moveSpeed * Time.deltaTime);
    }

    private void HandleFloating()
    {
        // Create a floating effect for the ghost
        floatTimer += Time.deltaTime * floatSpeed;
        float newY = startPosition.y + Mathf.Sin(floatTimer) * floatAmount;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectible"))
        {
            GameManager.Instance?.CollectItem();
            Destroy(other.gameObject);
        }
    }
}
