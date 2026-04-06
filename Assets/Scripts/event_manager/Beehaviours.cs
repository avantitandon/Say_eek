using UnityEngine;

public class Beehaviours : MonoBehaviour

{       float period = 6;
    float CurrentTime = 0;
    public bool IsWaiting = false;
    public Vector3 desiredVelocity;
    public int CurrentPathIndex =-1;
     public UnityEngine.AI.NavMeshAgent bzNavMesh { get; private set; }

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
    float spawnTime = 0f;
    private bool IsCreeping = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bzNavMesh = GetComponent<UnityEngine.AI.NavMeshAgent>();
        CameraUpOrNot = Main_Camera.GetComponent<CameraUpOrNot>();
    }
    
    // Update is called once per frame
    void Update()
    {
        if(IsCreeping)
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
            
            
            Debug.Log("beez is creeping");
            
            m_PathDestinationNodeIndex = patrolPath.UpdatePathDestination(gameObject.transform, m_PathDestinationNodeIndex);

            Vector3 nextDestination = patrolPath.GetDestinationOnPath(gameObject.transform, m_PathDestinationNodeIndex);

            transform.LookAt(nextDestination);

            SetNavDestination(nextDestination); 

            
            
            
            if (CurrentPathIndex != m_PathDestinationNodeIndex)
            {   
                Debug.Log("truly she is here....");
                CurrentPathIndex = m_PathDestinationNodeIndex;
                IsWaiting = true;
                CurrentTime = 0;
                
            }   
        }
    }
    
    public void WhatIsBeezDoing(BeezakaManager.WhatShouldBeezDo whatShouldBeezDo)
    {
        switch (whatShouldBeezDo)
        {
            case BeezakaManager.WhatShouldBeezDo.InHell:
                Debug.Log("she certainly is in hell");
                break;
            case BeezakaManager.WhatShouldBeezDo.Spawning:
                Debug.Log("you're gonna spawn alright");
                break;
            case BeezakaManager.WhatShouldBeezDo.Creeping:
                Debug.Log("you're gonna creep alright");
                IsCreeping = true;

                break;
            case BeezakaManager.WhatShouldBeezDo.Stalking:
                Debug.Log("you're gonna stalk alright");
                break;

        }
    }
        
       
            
        
        public void SetNavDestination(Vector3 destination)
    {


        if (bzNavMesh.enabled)
        {
            bzNavMesh.SetDestination(destination);
        }
        
    }
}
