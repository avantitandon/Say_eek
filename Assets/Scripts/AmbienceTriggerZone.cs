using UnityEngine;

public class AmbienceTriggerZone : MonoBehaviour
{
    [SerializeField] private AK.Wwise.Event playPathAmbience; // Wwise event
    [SerializeField] private GameObject player; // Player reference

    void Start()
    {
        // Optionally play ambience at start
        playPathAmbience.Post(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playPathAmbience.Post(player);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playPathAmbience.Post(player);
        }
    }
}
