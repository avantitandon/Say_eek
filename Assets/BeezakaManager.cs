using UnityEngine;

public class BeezakaManager : MonoBehaviour
{   
    [SerializeField] private Beehaviours beehaviours;
    public UnityEngine.AI.NavMeshAgent bzNavMesh { get; private set; }
    public enum WhatShouldBeezDo
    {
        InHell,
        Spawning,
        Creeping,
        Stalking,
    }
    private WhatShouldBeezDo whatShouldBeezDo;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //spawned should totally be a bool 
    float spawnTime = 0f;
    public GameObject beezakaportal;
    float velocity;
    void Start()
    {
        EventManager.BEEZAKAPOCALYPSENOW += BZisNigh;
        EventManager.GazeboKissing += AttendCeremony;
        bool BeezSpawned = false;
        whatShouldBeezDo = WhatShouldBeezDo.InHell;
        bzNavMesh = GetComponent<UnityEngine.AI.NavMeshAgent>();
        
    }

    // Update is called once per frame
    private void BZisNigh()
    {
        whatShouldBeezDo = WhatShouldBeezDo.InHell;
        Debug.Log("beezaka start spawning");
        Vector3 deltaPos = beezakaportal.transform.position - transform.position;
        Debug.Log("bz manager bzisnigh!!!");
        switch(whatShouldBeezDo)
        {
            case (WhatShouldBeezDo.Spawning):
                break;
            case (WhatShouldBeezDo.InHell):
                while (spawnTime < 1.0f)
                    
                        Debug.Log("beezaka going up the hellevator");
                        spawnTime += Time.deltaTime;
                        transform.position += deltaPos * Time.deltaTime;
                            
                        if (velocity > 0f)
                        {    Debug.Log("beezaka still going up the elevator");
                            return;
                        }
                        else
                        {
                            Debug.Log("beezaka has surely made it to the top floor by now");
                            whatShouldBeezDo = WhatShouldBeezDo.Creeping;
                            break;
                        }
                
        }
    }
    private void AttendCeremony()
    {   
        Debug.Log("i sit my ass down and listen");
        whatShouldBeezDo = WhatShouldBeezDo.Stalking;
        //BZstate = 2;
        //BZattending = true;
        //Beehaviour = 4;

    }
    void Update()
    {
        
        beehaviours.WhatIsBeezDoing(whatShouldBeezDo);
        velocity = bzNavMesh.velocity.magnitude;

        switch(whatShouldBeezDo)
        {
            case (WhatShouldBeezDo.Spawning):
                Debug.Log("void update spawning confirm");
                break;
            case (WhatShouldBeezDo.Creeping):
                Debug.Log("void update creeping confirm");
                break;
            case (WhatShouldBeezDo.Stalking):
                Debug.Log("void update stalking confirm");
                break;
            
        }
    }
}

/*
            switch (WhatShouldBeezDo.whatShouldBeezDo)
        {   case (WhatShouldBeezDo.InHell):
                whatShouldBeezDo = WhatShouldBeezDo.Spawning;
                //start to spawn her in  here//
                behaviours.WhatIsBeezDoing(whatShouldBeezDo);
            case (WhatShouldBeezDo.Spawning):
        }

        {   if(WhatShouldBeezDo.InHell && spawnTime <1.0f)
            {   Debug.Log("beezaka start spawning");
                Vector3 deltaPos = beezakaportal.transform.position - transform.position;
                
            
                while (spawnTime < 1.0f)
                {   
                    
                    Debug.Log("beezaka going up the hellevator");
                    spawnTime += Time.deltaTime;
                    transform.position += deltaPos * Time.deltaTime;
                    
                    if (velocity > 0f)
                        Debug.Log("beezaka still going up the elevator");
                        return;
                }
            }
        }          
         //put in her spawning here? and then afterwards set to false. when its set to calse, send a value to beehaviour() which will  then know what to do according to the cases? like how dominik does the tutorial step > audio manager
    }
    private void Update()
    {
        beehaviours.WhatIsBeezDoing(whatShouldBeezDo);
    }

    // dom said to make a bunch of custom functions like this !!! private void Beehaviour() and stuff and they can be called and stuff by other scripts 
    /*private void BeezSpawning()
    {
        BeezIsCreeping()
    }
    private void*/

