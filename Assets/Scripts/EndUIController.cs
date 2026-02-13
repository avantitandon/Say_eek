using UnityEngine;

// for ui text
using TMPro;



public class EndUIController : MonoBehaviour
{

    public TMP_Text ResultsText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetScoreText(int[] scores, int numPhotos)
    {
        string resultsText = "Popularity Scores: \n";

        for (int i = 0; i < numPhotos; i++) 
        {
            resultsText = string.Concat(resultsText, "\n\n Photo ", (i+1).ToString(), " : ", scores[i].ToString(), "K likes \n");
        }

        ResultsText.text = resultsText;
    }
}
