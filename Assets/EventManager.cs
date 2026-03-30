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
    [SerializeField] private GameController gameController;
    
    public static event Action StageDomSolo;
    public static event Action StagePlusIdol;
    public static event Action StagePlusBhaddie;
    public static event Action BEEZAKAPOCALYPSENOW;
    public static event Action GazeboKissing;

    public GameObject controllerscript;
    public float TimeElapsed;
    private float time;
    bool djdom = false;
    bool idol = false;
    bool bhaddie = false;
    bool brides = false;
    bool beezaka = false;
    
    void Awake()
    {
        time = Time.time;
        gameController = controllerscript.GetComponent<GameController>();
        TimeElapsed = Time.time - gameController.roundStartTime;

    }
    /*private void HandleGame()
    {
        // only run if the game is active
        if (gameController.gameState != gameController.State.Active)
        {
            return;
        }

        float time_elapsed = Time.time - gameController.roundStartTime;
    }*/
    void Update()
    
    {   
        if (Time.time >= time + 1f && !djdom)
        {
            StageDomSolo?.Invoke();
            Debug.Log("(em)calling  all dj  DOMS!!!!!");
            djdom = true;
        }   
        
        if (Time.time >= time + 100f && !idol)
        {
            //i  dont think EndInvoke works? just make the events last some seconds in their managers idt it matters
            StagePlusIdol?.Invoke();
            Debug.Log("(em)IDOL TIME!!!!! HAIIIIII!!!! :333");
            idol = true;
            
        }
        if (Time.time >= time + 200f && !bhaddie)
        {
            
            StagePlusBhaddie?.Invoke();
            Debug.Log("(em)catch me out side,  how about it?");
            bhaddie = true;
            
        }
        if (Time.time >= time + 137.5f && !beezaka)
        {
            BEEZAKAPOCALYPSENOW?.Invoke();
            Debug.Log("(em)BEEZAKAPOCALYPSENOW");
            beezaka = true;
            
        }
        if (Time.time >= time + 250f && !brides)
        {
            GazeboKissing?.Invoke();
            Debug.Log("(em)YURIIIIII");
            brides = true;
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