using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class beezakaAI : MonoBehaviour
{
    float period = 3;
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
    void Start()
    {
        bzNavMesh = GetComponent<NavMeshAgent>();
        CameraUpOrNot = Main_Camera.GetComponent<CameraUpOrNot>();
    }

    void Update()
    {   
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

    public void SetNavDestination(Vector3 destination)
    {


        if (bzNavMesh.enabled)
        {
            bzNavMesh.SetDestination(destination);
        }
        
    }
}