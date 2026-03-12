
using UnityEngine;

// note this is just for a physical trigger
// will extend for timer trigger

public class BossTriggerController : MonoBehaviour
{

// takes boss text script
    [SerializeField] private BossTextScript bossText;
    // each game object will have its own text and only be triggered once
    [SerializeField] private string text = "Take as many well framed photos as you can!";
    [SerializeField] private float duration = 3f;
    
    [SerializeField] private bool triggerOnce = true;

// extra check for triggered for later 
    private bool hasTriggered;

    private void OnTriggerEnter(Collider other)
    {


        if (triggerOnce && hasTriggered) return;
        if (!other.CompareTag("Player")) return;
        if (bossText == null) return;

        bossText.SetTextMessage(text, duration);
        hasTriggered = true;
    }
}
