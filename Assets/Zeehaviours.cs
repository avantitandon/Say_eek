using UnityEngine;
using UnityEngine.AI;
using System;
public class Zeehaviours : MonoBehaviour
{   
    public UnityEngine.AI.NavMeshAgent zekeNavMesh { get; private set; }


    public static event Action BlowBigHorn;
    public static event Action BlowLittleHorn;
    public static event Action PanicHorn;
    public static event Action SPECIALHORN;
    public static event Action overHERE;
    public static event Action WelcomeHorn;
    public Transform player;
    public Transform ZekeTether;

    private Vector3 targetlocation;
    public bool FoundPlayer = false;
    bool atEvent = false;
    Vector3 goherezeke;
    ZekeManager zekeManager;
    public GameObject ZekeManagerobj;
    bool GameStarted = false;
    public int zekeisdoing;
    float distancetoplayer;
    int whatwaszekedoing;
    float zeketimer;
    bool welcome =  false;
    float zeketurn;
    public bool Waving = false;
//1. find player
//2. found player
//3. find event
//4. found event
//5. find special spot
//6. panic
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        zekeNavMesh = GetComponent <UnityEngine.AI.NavMeshAgent>();
        zekeManager = ZekeManagerobj.GetComponent<ZekeManager>();
        GameController.GAMESTART += UnleashZeke;
        whatwaszekedoing = zekeisdoing;
        
        goherezeke = new Vector3(zekeManager.EventLocation.x, 0, zekeManager.EventLocation.z);
    }

    // Update is called once per frame

    public void Z2Zrecieve(ZekeManager.WhatShouldZekeDo whatShouldZekeDo)
    {
        switch (whatShouldZekeDo)
        {
            case ZekeManager.WhatShouldZekeDo.WaitStart:
                zekeisdoing = 1;
                //1 will stand for waitstart
                return;
            case ZekeManager.WhatShouldZekeDo.SeekPlayer:
                zekeisdoing = 2;
                return;
            case ZekeManager.WhatShouldZekeDo.SeekEvent:
                zekeisdoing = 3;
                return;
            case ZekeManager.WhatShouldZekeDo.ChillOut:
                zekeisdoing = 4;
                return;
            case ZekeManager.WhatShouldZekeDo.GOTOSPECIALSPOT:
                zekeisdoing = 5;
                return;
            case ZekeManager.WhatShouldZekeDo.Panic:
                zekeisdoing = 6;
                return;
        }
    }
    public void UnleashZeke()
    {
        GameStarted = true;
        transform.LookAt(player.transform.position);
        
        WelcomeHorn?.Invoke();
    }
    public void WhatIsZekeDoing()
    {   
        //if (zekeisdoing == 1)
        
        
        if (zekeisdoing == 2)
        {   Debug.Log("checkingforplayerposition");
            
            if (!FoundPlayer)
            {
                FindPlayer();              
            }
            if (FoundPlayer)
            {
                transform.LookAt(player.transform.position);
                //zekeNavMesh.isStopped = true;
                //zeketimer = Time.time;
                //if (Time.time ==zeketimer + 3.5f)
                {
                    //zekeNavMesh.isStopped = false;
                }
            }
            
        }
        if (zekeisdoing ==3)
        {
            Debug.Log("little horn brrt!!!");
            transform.LookAt(player.transform.position);
            BlowLittleHorn?.Invoke();
            //zekeNavMesh.isStopped = true;
            FindEvent();
            //zeketimer = Time.time;
                //if (Time.time == zeketimer + 3.5f)
                {
                    //zekeNavMesh.isStopped = false;
                }   
            
        }    
        if (zekeisdoing == 4)
        {
            overHERE?.Invoke();
            transform.LookAt(player.transform.position);
            //zekeNavMesh.isStopped = true;
            Waving = true;
            zeketimer = Time.time;
                if (Time.time == zeketimer + 6f)
                {
                    //zekeNavMesh.isStopped = false;
                    Waving = false;
                }
            

        }
        if (zekeisdoing == 5)
            Debug.Log("going to brides!!!");
            if ((transform.position - player.transform.position).magnitude > 10f)
            {
                
                FindPlayer();
                
                if (FoundPlayer)
                {
                    transform.LookAt(player.transform.position);
                    SPECIALHORN?.Invoke();
                    FindEvent();
                    //zeketimer = Time.time;
                    //zekeNavMesh.isStopped = true;
                    //if (Time.time == zeketimer + 4f)
                    {
                        
                        //zekeNavMesh.isStopped = false;
                    }
                }
                //return;
            }
            
        if (zekeisdoing == 6)
        {   Debug.Log("ITS OVERRR");
            transform.LookAt(player.transform.position);
            PanicHorn?.Invoke();
            
            //return;
        }
    }
    void Update()
    {   
        distancetoplayer = (player.transform.position-transform.position).magnitude;
        if (GameStarted)
        {   goherezeke = new Vector3(zekeManager.EventLocation.x, 0, zekeManager.EventLocation.z);
            
    //checking  to  see if zeke is close enough  to be at the event, so FindEvent()knows  to trigger its if statement and blow his  horn
            if ((goherezeke - transform.position).magnitude < 10f)
            {
                atEvent = true;
                WhatIsZekeDoing();
            }
            
            if (zekeisdoing == 2)
            {  
                if (distancetoplayer < 10)
                {
                    Debug.Log("foundplayer!!!!");
                    transform.LookAt(player.transform.position);
                    
                    FoundPlayer = true;
                    WhatIsZekeDoing();
                }
                else
                {   
                    Debug.Log("where is  player...");
                    FoundPlayer = false;
                }
                WhatIsZekeDoing();
            }
            if (zekeisdoing == 5)
            {  
                if (distancetoplayer < 10)
                {
                    Debug.Log("foundplayer!!!!");
                    transform.LookAt(player.transform.position);
                    
                    FoundPlayer = true;
                    WhatIsZekeDoing();
                }
                else
                {   
                    Debug.Log("where is  player...");
                    FoundPlayer = false;
                }
                WhatIsZekeDoing();
                }
            if (zekeisdoing != whatwaszekedoing)
            {
                WhatIsZekeDoing();
                whatwaszekedoing = zekeisdoing;
            }
        } 
    }
   
   //i think it  would be neat if all of zeehaviours' switch cases triggered functions instead...
    private void FindPlayer()
    {
        //soo .. sets his  destination and then 
        Debug.Log("lookingforplayer");
        
        zekeNavMesh.destination = ZekeTether.transform.position;
        WhatIsZekeDoing();
    }
    private void FindEvent()
    {
        //find the location of the current event

        atEvent =  false;
        zekeNavMesh.destination = goherezeke;
        if (atEvent)
        {
            transform.LookAt(player.transform.position);
            BlowBigHorn?.Invoke();
            WhatIsZekeDoing();
            //zekeNavMesh.isStopped = true;
            //zeketimer = Time.time;
                //if (Time.time == zeketimer + 3.5f)
                {
                    //zekeNavMesh.isStopped = false;
                }
            return;   
        }
        //if
        
            //REMEMBER to make the OVERHERE!! animation play after the horn animations (not the beezaka one) play 
           
    }
    
}