using UnityEngine;

using TMPro;

public class StatusUIController : MonoBehaviour
{
    // CONSTANTS //

    private const int START_HOUR = 6;

    // chose this so that time ends at midnight from 6pm across 5 irl minutes
    private const float SECONDS_PER_15_MINS = 12.5f;

    // AUDIO //

    // GAME COMPONENTS //

    [SerializeField] private TMP_Text PhotosLeft;
    [SerializeField] private TMP_Text TimerText;

    // VARIABLES //

    // LOGGING //


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetStatusUI (float timeElapsed, int photosLeft)
    {
        // update photos left count
        PhotosLeft.text = string.Concat("Photos left: ", photosLeft);
        
        // update timer



        // calculate how many 15 minutes have passed
        int quarters_passed = Mathf.FloorToInt(timeElapsed / SECONDS_PER_15_MINS);

        // calculate current hour
        int hour = START_HOUR + Mathf.FloorToInt(quarters_passed / 4);
        int minutes = 15 * (quarters_passed - (Mathf.FloorToInt(quarters_passed / 4) * 4));

        // actually update the text   
        TimerText.text = string.Concat("Time: ", hour.ToString("00"), ":", minutes.ToString("00"));
    }
}
