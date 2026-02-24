using UnityEngine;

public class PathAmbienceTrigger : MonoBehaviour
{
    [SerializeField] private AK.Wwise.Event playPathAmbience; // Wwise event
    
    void Start()
    {
        playPathAmbience.Post(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
