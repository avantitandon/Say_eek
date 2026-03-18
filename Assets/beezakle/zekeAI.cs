using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class zekeAI : MonoBehaviour
{
    public NavMeshAgent ai;
    public Transform player;
    public Animator zekeAnim;
    public GameObject ZekeTetherfront;
    public GameObject ZekeTetherup;
    public GameObject ZekeTetherback;
    public GameObject SPECIAL_SPOT;

        public GameObject Main_Camera;
        public CameraUpOrNot CameraUpOrNot;
    Vector3 dest;

    public int ZekeBehaviour;

    private GameObject[]POI;
    private bool[] Visited;

    public float aggro = 80;
    public bool ZekeSeekCamera = false;
    public bool TimedEvent = false;

    /*
     * call this function every frame, and it will decide what zeke does
     * returns an int code to represent what zeke should do
     If nothing is happening and there's nothing interesting around.... he should just hangout

     if there's something interesting in the vicinity, he should blow his little horn and start moving to it

     if zeke was going somewhere and now he's there, he should wait and emote

     if zeke was waiting, and the player approaches, he should hangout with them

     if an EVENT says that Zeke should blow his big horn, he'll do that and fly to the special point at the end of the level
     */
    void Start()
    {
        POI = GameObject.FindGameObjectsWithTag("POI");
        Visited = new bool [POI.Length];
        for (int i=0;i<Visited.Length;i++)
        {
            Visited[i]=false;
        }

        zekeAnim = GetComponent<Animator>();
        CameraUpOrNot = Main_Camera.GetComponent<CameraUpOrNot>();
        
        
    
    }


    private void WhatShouldZekeDo()
    {   if (ZekeBehaviour == 1)
        {
            for (int i = 0; i < POI.Length; i++)
            {
                GameObject Point = POI[i];
                if (Visited[i])
                {
                    break;
                }
                if ((Point.transform.position-transform.position).magnitude < aggro)
                {
                    ai.destination = Point.transform.position;
                    //blow littlehorn here
                    Visited[i]=true;
                    ZekeBehaviour = 2;
                    return;
                }
            }
        }
        if (ZekeBehaviour == 2 && !TimedEvent&&(transform.position-ai.destination).magnitude<20)
        {
            ZekeBehaviour = 3;
            transform.LookAt(player.transform.position);
            return;
        }
        if (ZekeBehaviour == 3)
        {
            float distancetoplayer = (player.transform.position - transform.position).magnitude;
            if (distancetoplayer < 10)
            {
                ZekeBehaviour = 1;
                return;
            }
            
            else 
            {
                ZekeBehaviour = 3;
                transform.LookAt(player.transform.position);
                return;
            }

        }
        if (ZekeBehaviour != 2 && TimedEvent)
        {
            //BLOW THE BIG HORN
            ZekeBehaviour = 2;
            ai.destination = SPECIAL_SPOT.transform.position;
            return;
        }
        if (TimedEvent &&(transform.position - SPECIAL_SPOT.transform.position).magnitude <= 20)
        {
            ZekeBehaviour = 3;
            
                TimedEvent=false;
                transform.LookAt(player.transform.position);

            return;
        }
        if (ZekeBehaviour == 2)
        {
            return;
        }
//this  is where we hang out! hangout zone !! 
        ZekeBehaviour = 1;
        
        float a,b,c;
        a = (ZekeTetherfront.transform.position - transform.position).magnitude;
        b = (ZekeTetherup.transform.position - transform.position).magnitude;
       // c = (ZekeTetherback.transform.position - transform.position).magnitude;
        if (a < b)
        {
            ai.destination = ZekeTetherfront.transform.position;
        }
        else
        {
            ai.destination = ZekeTetherup.transform.position;
        }
//smoke break time
        if (ZekeSeekCamera && ZekeBehaviour == 1)
        {
            ai.destination = ZekeTetherback.transform.position;
        }
    }

    void Update()
    {   WhatShouldZekeDo();

        zekeAnim.SetInteger("WhatIsZekeDoing",ZekeBehaviour);
        zekeAnim.SetBool("TimedEvent",TimedEvent);

        if (CameraUpOrNot.CameraUp)
        {
            ZekeSeekCamera = true;
        }
        else
        {
            ZekeSeekCamera = false;
        }


        /*
            int zekeBehaviour = WhatShouldZekeDo()
            //1 = hangoutinvision
            //2 =  gotoPOI
            //3 = hangoutatPOI

            if reunite then go to closest tether point

            if hanging out in vision then go to closest point in front of player and animate

            if go to poi
                if no destination, findDestination()
                if has destination, move to destination
                if at destination, move on to hanging out

            if hanging out at poi
                animate and annoy
                if player approaches, reunite with player

        */

        
        //dest = player.position;
        //ai.destination = ZekeTetherfront.transform.position;

    }


}


