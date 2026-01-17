using UnityEngine;

public class GhostController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float floatAmount = 0.5f;

    private float floatTimer;
    private float baseYPosition;

    void Start()
    {
        baseYPosition = transform.position.y;
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
        float floatOffset = Mathf.Sin(floatTimer) * floatAmount;
        Vector3 newPosition = transform.position;
        newPosition.y = baseYPosition + floatOffset;
        transform.position = newPosition;
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
