using UnityEngine;

// for ui text
using TMPro;



public class EndUIController : MonoBehaviour
{

    public TMP_Text ResultsText;
    public TMP_Text ResultsText2;

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
        string resultsText2 = "\n";

        for (int i = 0; i < numPhotos; i++) 
        {
            if (i < 10)
            {
                resultsText = string.Concat(resultsText, "\n Photo ", (i+1).ToString(), " : ", scores[i].ToString(), "K likes");
            }
            else
            {
                resultsText2 = string.Concat(resultsText2, "\n Photo ", (i+1).ToString(), " : ", scores[i].ToString(), "K likes");
            }
        }

        ResultsText.text = resultsText;
        ResultsText2.text = resultsText2;
    }
}
