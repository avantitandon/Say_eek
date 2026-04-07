using UnityEngine;
using UnityEngine.AI;

public class ZekeManager : MonoBehaviour
{   
    [SerializeField] private Zeehaviours zeehaviours;
    public enum WhatShouldZekeDo
    {
        WaitStart,
        SeekPlayer,
        SeekEvent,
        GOTOSPECIALSPOT,
        ChillOut,
        Panic,

    }
    
    WhatShouldZekeDo whatShouldZekeDo;
    private int whatDIDzekedo;
    private bool GoToDom = false;
    [SerializeField] Transform fountainlocation;
    [SerializeField] Transform stagelocation;
    [SerializeField] Transform SPECIAL_SPOT;
    public GameObject zeke;
    public Vector3 EventLocation;
    public Transform player;

    // AUDIO //
    [SerializeField] private AudioManager audioManager;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventManager.GazeboKissing += AnnounceCeremony;
        EventManager.StageDomSolo += AnnounceDom;
        EventManager.StagePlusIdol += AnnounceIdol;
        EventManager.StagePlusBhaddie += AnnounceBhaddie;
        EventManager.BEEZAKAPOCALYPSENOW += Panic;
        whatShouldZekeDo = WhatShouldZekeDo.WaitStart;
        whatDIDzekedo = (int)whatShouldZekeDo;
        

        EventLocation = new Vector3(fountainlocation.position.x,fountainlocation.position.y, fountainlocation.position.z);

    }

    // Update is called once per frame
    void Update()
    {   if (whatShouldZekeDo == WhatShouldZekeDo.SeekPlayer && (zeke.transform.position - player.transform.position).magnitude < 10f)
        {
            whatShouldZekeDo = WhatShouldZekeDo.SeekEvent;
            whatDIDzekedo = (int)whatShouldZekeDo;
            Z2Zcommunication();
        }
        if (whatShouldZekeDo == WhatShouldZekeDo.WaitStart && (zeke.transform.position - player.transform.position).magnitude < 10f)
        {
            whatShouldZekeDo = WhatShouldZekeDo.SeekPlayer;
            whatDIDzekedo = (int)whatShouldZekeDo;
            
            
            Z2Zcommunication(); 
        }


        //checking if Zeke is currently chilling and  if he is close to the player
        if (whatShouldZekeDo == WhatShouldZekeDo.ChillOut && (zeke.transform.position - player.transform.position).magnitude < 10f)
        {   
            //checking for if its dom time and then if not check for state change
            
                    
            if (GoToDom)
            {
                EventLocation = new Vector3(stagelocation.position.x, stagelocation.position.y, stagelocation.position.z);
                whatShouldZekeDo = WhatShouldZekeDo.SeekEvent;
                whatDIDzekedo = (int)whatShouldZekeDo;
                GoToDom = false;
            }
            else if ((int)whatShouldZekeDo != whatDIDzekedo)
            {
                
                whatDIDzekedo = (int)whatShouldZekeDo;
            }
            else if ((int)whatShouldZekeDo == whatDIDzekedo)
            {
                whatShouldZekeDo = WhatShouldZekeDo.ChillOut;
            }
            Z2Zcommunication();
            
            
        }
        if (whatShouldZekeDo == WhatShouldZekeDo.SeekEvent && (zeke.transform.position - EventLocation).magnitude < 10f)
        {
            whatShouldZekeDo = WhatShouldZekeDo.ChillOut;
            whatDIDzekedo = (int)whatShouldZekeDo;
            Z2Zcommunication();            
        }
        if ((int)whatShouldZekeDo != whatDIDzekedo)
        {
         //if what zeke should  do changes, and he was seeking,   he should still seek?
            if (whatShouldZekeDo == WhatShouldZekeDo.SeekPlayer)
            {
                zeke.transform.LookAt(player.transform.position);
                whatDIDzekedo = (int)whatShouldZekeDo;
            }             
            
            if (whatShouldZekeDo == WhatShouldZekeDo.Panic)
            {
                Debug.Log("AHHHHHHHHH");
                whatDIDzekedo = (int)whatShouldZekeDo;
            }

            if (whatShouldZekeDo == WhatShouldZekeDo.GOTOSPECIALSPOT)
            {
                Debug.Log("GOGOGOGOGOGOGOGOGO");
                whatDIDzekedo = (int)whatShouldZekeDo;
            }


            Z2Zcommunication();
        }

        //AUDIO ON UPDATE //

        audioManager.HandleZekeHorn(whatShouldZekeDo);


        

/*        //checking for state changes, therefore also checking for event changes
        if ((int)whatShouldZekeDo != whatDIDzekedo += WhatShouldZekeDo.ChillOut)
        {
            //so like, when what zeke should do has changed since the last time AND  the last thing he did
            //was hanging out...  that means that zeke should go find the player bc that means he was just waiting
            //at  the event.......
            
            whatShouldZekeDo = WhatShouldZekeDo.SeekingPlayer;
            whatDIDzekedo = (int)whatShouldZekeDo;
            Z2Zcommunication();
            
        }
        */
        //if he has reached  the event location!!! btw every time  update changes what  zeke should do prob call z2z communication to send state change over
        /*if ((zeke.transform.position - player.transform.position).magnitude < 10f)
        {
            switch (whatShouldZekeDo)
            {
                case WhatShouldZekeDo.SeekPlayer:
                zeke.transform.LookAt(player.transform.position);
                whatShouldZekeDo = WhatShouldZekeDo.SeekEvent;
                whatDIDzekedo = (int)whatShouldZekeDo;
                Z2Zcommunication();
                return;
            }
        }
        if ((zeke.transform.position - EventLocation).magnitude < 10f)
        {
            switch (whatShouldZekeDo)
            {
                case WhatShouldZekeDo.SeekEvent:
                zeke.transform.LookAt(player.transform.position);
                whatShouldZekeDo = WhatShouldZekeDo.ChillOut;
                whatDIDzekedo = (int)whatShouldZekeDo;
                Z2Zcommunication();
                return;
            }
        // if  zeke is close enough, he should CHILL TF OUT!!!!!!!
        }
        if (whatDIDzekedo == (int)whatShouldZekeDo)
        {
            
        }*/
    
    }

    private void Z2Zcommunication()
    {
        //ensures that functions are only called once, so zeehaviours knows what zeke should do when zeke needs to do something
        Debug.Log("zeke to zeke:" + whatShouldZekeDo);
        zeehaviours.Z2Zrecieve(whatShouldZekeDo);
        
    }
    private void AnnounceDom()
    {
        Debug.Log("DOM!!! THIS WAY!!!!!");
        //go here function?  like set a position HERE for zeke to go to and then send that to findevent()
        whatShouldZekeDo = WhatShouldZekeDo.WaitStart;
        GoToDom = true;
        
        
        //ONLY if zeke has already made it to the fountain, then send to whatShouldZekeDo
        //uh maybe actually just call the functions in zeehaviours eehe i think i was fdoing it wrong

    }
    private void AnnounceIdol()
    {
        Debug.Log("IDOL!!! THIS WAY!!!!");
        whatShouldZekeDo = WhatShouldZekeDo.SeekPlayer;
        EventLocation = new Vector3(stagelocation.position.x, stagelocation.position.y, stagelocation.position.z);
        
        
    }

    private void AnnounceBhaddie()
    {
        Debug.Log("BHADDIE!!! THIS WAY!!!!!");
        whatShouldZekeDo = WhatShouldZekeDo.SeekPlayer;
        EventLocation = new Vector3(stagelocation.position.x, stagelocation.position.y, stagelocation.position.z);
        
        
    }
    //ouhhhhhhhh doooyyyyy every tume he has a new specal spot to go to he has to seek player.....
    private void AnnounceCeremony()
    {
        Debug.Log("BRIDES!!!! THIS WAY!!!!!");
        
        EventLocation= new Vector3(SPECIAL_SPOT.position.x, SPECIAL_SPOT.position.y, SPECIAL_SPOT.position.z);
        whatShouldZekeDo = WhatShouldZekeDo.SeekPlayer;
        

        
        
    }

    private void Panic()
    {
        whatShouldZekeDo = WhatShouldZekeDo.Panic;
        Debug.Log("ONOOOOOOOOOOO");
        

    }

}   
