using UnityEngine;

using System.Collections.Generic;

using TMPro;

public class FinalScoreController : MonoBehaviour
{
    // CONSTANTS


    // GAME COMPONENTS

    [SerializeField] private GameObject FinalScoreCanvas;
    [SerializeField] private TMP_Text scoreText;

    [SerializeField] private GameObject advanceText;

    // VARIABLES


    public void Init()
    {
        advanceText.SetActive(false);
    }
    public bool attemptContinue(bool submitAction)
    {
        return submitAction;
    }
    public void toggleCanvas(bool enabled)
    {
        FinalScoreCanvas.SetActive(enabled);
    }
    public void toggleAdvanceText(bool enabled)
    {
        advanceText.SetActive(enabled);
    }
    public void updateScoreText(List<int> scores)
    {
        string resultsText = "";

        for (int i = 0; i < scores.Count; i++) 
        {
            if (i < 10)
            {
                resultsText = string.Concat(resultsText, "Photo ", (i+1).ToString(), " : ", scores[i].ToString(), "K likes \n");
            }
        }

        scoreText.text = resultsText;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
