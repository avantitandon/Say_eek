using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class beezakaAI : MonoBehaviour
{
    float period = 6;
    float CurrentTime = 0;
    public bool IsWaiting = false;
    public Vector3 desiredVelocity;
    int CurrentPathIndex =-1;
     public NavMeshAgent bzNavMesh { get; private set; }

    public NPCPatrol patrolPath;
    public int m_PathDestinationNodeIndex = 0;
    public Transform target;
    public Transform player;
        public GameObject Main_Camera;
        public CameraUpOrNot CameraUpOrNot;

    public bool BeezakaSeekCamera;

    public GameObject BeezakaTether;
    public GameObject beezakaportal;

    //bools for  controlling what  beezaka is doing and in what order
    bool BZspawning = false;
    bool BZspawned = false;
    bool BZattending = false;
    
    public int Beehaviour;

    //private State state;
    void Start()
    {
        EventManager.BEEZAKAPOCALYPSENOW += BZisNigh;
        EventManager.GazeboKissing += AttendCeremony;
        bzNavMesh = GetComponent<NavMeshAgent>();
        CameraUpOrNot = Main_Camera.GetComponent<CameraUpOrNot>();
        Beehaviour = 1;

    }
/*
beehaviour tree:
    1. inHELL
    2. spawning
    3. spawned
    4. attending
    
*/
private void WhatShouldBeezDo()
//spawn in
{   
    float velocity = bzNavMesh.velocity.magnitude;
    if (Beehaviour == 2)
        {   
            Debug.Log("beezaka start spawning");
            Vector3 deltaPos = beezakaportal.transform.position - transform.position;
            float spawnTime = 0f;
            while (spawnTime < 1.0f)
            {   
                Debug.Log("beezaka going up the hellevator");
                spawnTime += Time.deltaTime;
                transform.position += deltaPos * Time.deltaTime;
                
            if (velocity == 0 && (transform.position - beezakaportal.transform.position).magnitude <= 0.1f)
                {   
                    Debug.Log("beezaka reached her destination");
                    //BZstate = 1;
                    //BZspawning = false;
                    //BZspawned = true;
                    Beehaviour = 3;
                    return;
                }
                return;
            }
        }

        //patrol paths here
        if (Beehaviour == 3)
        {   Debug.Log("truly she is here....");

            if (CameraUpOrNot.CameraUp)
            {
                BeezakaSeekCamera=true;
            }
            else
            {
                BeezakaSeekCamera=false;
            }

        if (BeezakaSeekCamera)
        {   
            bzNavMesh.speed = 12; 
            if ((transform.position-BeezakaTether.transform.position).magnitude > 6)
            {
                transform.LookAt(BeezakaTether.transform.position);
                bzNavMesh.destination = BeezakaTether.transform.position;              
            }
            else
            {
                
                if ((transform.position-BeezakaTether.transform.position).magnitude < 1)
                {
                    transform.LookAt(player.transform.position);
                    return;
                }
            }
            return;
            }

            if (IsWaiting)
            {
                CurrentTime=CurrentTime+Time.deltaTime;

                if (CurrentTime >= period)
                {
                    CurrentTime = 0;
                    IsWaiting = false;
                    bzNavMesh.speed = 8;
                }
                else
                {
                    return;
                }
            }
                    

            m_PathDestinationNodeIndex = patrolPath.UpdatePathDestination(gameObject.transform, m_PathDestinationNodeIndex);
            
            if (CurrentPathIndex != m_PathDestinationNodeIndex)
            {
                CurrentPathIndex = m_PathDestinationNodeIndex;
                IsWaiting = true;
                CurrentTime = 0;
                return;
            }
            Vector3 nextDestination = patrolPath.GetDestinationOnPath(gameObject.transform, m_PathDestinationNodeIndex);

            transform.LookAt(nextDestination);

            SetNavDestination(nextDestination);        
                    
            
        
       
        }
    }

    public void SetNavDestination(Vector3 destination)
    {


        if (bzNavMesh.enabled)
        {
            bzNavMesh.SetDestination(destination);
        }
        
    }
    private void BZisNigh()
    {
        Debug.Log("(em)(bz is nigh)  she is COMING  ......");
        Beehaviour = 2;
        //BZstate = 0;
        //BZspawning = true;
        //put her spawn-in stuff here? like this function listens for the event to happen, will activate when the event starts, spawns her in, and after that she will  proceed with void update?

    }
    private void AttendCeremony()
    {   
        Debug.Log("i sit my ass down and listen");
        //BZstate = 2;
        //BZattending = true;
        Beehaviour = 4;

    }
    void Update()
    {
        WhatShouldBeezDo();
    }

}
