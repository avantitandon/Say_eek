using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    /* the level is 300 seconds long!! from 6PM to 12AM. each hour is 50 seconds ?
    0-100s = DJ Dom solo
    100s-200s = Idol on stage
    200s-300s = Bhaddie with DJ dom
    137.5-300s = Beezakapocalypse
    250s-300s = Brides Kiss

    i think the order of things is to list out these events, what triggers them in  this script, and then make new script components for each thing listening for these events thats like "IM ALL EARS" 

    */


    public static event Action StageDomSolo;
    void Awake()
    {

    }

    void CheckGameState()
    {
        if (Time.time < 1)
        {
            StageDomSolo?.Invoke();
        }
    }

    /*private void Update()
    {   
        
        if(Time.time < 1 & gameState == GameController.State.Active)
        {
            StageDomSolo?.Invoke();
        }
    }*/
}