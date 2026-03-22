using UnityEngine;

public class AreaTriggers : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private string areaStateName;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        audioManager.HandleMusic(areaStateName);

    }
}
