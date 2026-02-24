using UnityEngine;

// for ui text
using TMPro;


public class HUDController : MonoBehaviour
{
    [SerializeField] private GameController gameController;

    public TMP_Text PhotosLeft;
    // adding this for timer?
    public TMP_Text TimerText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PhotosLeft.text = string.Concat("Photos left: ", GameController.MAX_PHOTOS - gameController.photosTaken);
        if (TimerText != null)
        {
            // seconds and minutes

            int totalSeconds = Mathf.Max(0, Mathf.CeilToInt(gameController.TimeRemainingSeconds));
            // no neg secomds 

            // csc110 flashbacks
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            TimerText.text = string.Concat("Time left: ", minutes.ToString("00"), ":", seconds.ToString("00"));
        }
    }
}
