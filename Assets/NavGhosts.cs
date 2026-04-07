using UnityEngine;
using System;
using UnityEngine.AI;


public class NavGhosts : MonoBehaviour
{   float randomtime;
    [SerializeField] Vector3 Gazebo;
    bool GoingToBrides = false;
    public UnityEngine.AI.NavMeshAgent ghostNavMesh {get ; private set;}
    float time;
    Vector3 min;
    Vector3 max;
    bool GameStarted  =  false;
    bool EEK = false;
    public Transform player;
    Transform targetrotation;
    float speed;
    Transform defaultposition;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameController.GAMESTART += StartTimer;
        ghostNavMesh = GetComponent <UnityEngine.AI.NavMeshAgent>();
        //CameraUpScript.CameraUP += sayEEK;
        //CameraUpScript.CameraDOWN += resume;
        //min = new Vector3(121,0,156);
        //max = new Vector3(58,0,100);
        //Gazebo = new Vector3(121,0,156);
        Gazebo  = new  Vector3(UnityEngine.Random.Range(121.0f,58.0f), 0, UnityEngine.Random.Range(156.0f,100.0f));
        //Gazebo = new Vector3(UnityEngine.Random.Range(min.x,max.x),UnityEngine.Random.Range(min.y,max.y),UnityEngine.Random.Range(min.z,max.z));
        
    }
    void Update()
    {   
        //if (!EEK)
        {
         //   GetDefaultPosition();
        }
        if (GameStarted && !GoingToBrides)
        {
            if(Time.time >= time + randomtime)
            {
                GoToBrides();
                GoingToBrides = true;
            }
        }
       // if ((player.transform.position - transform.position).magnitude <= 40 && EEK)
        {
       //     transform.rotation = Quaternion.Slerp(transform.rotation,targetrotation,speed * Time.deltaTime);
        }
        //if (!EEK)
        {
        //    transform.rotation = Quaternion.Slerp(defaultposition ,targetrotation,speed * Time.deltaTime);

        }

    }
    private void GetDefaultPosition()
    {
        var defaultposition = transform.position;
    }
    private void StartTimer()
    {
        time = Time.time;

        randomtime = UnityEngine.Random.Range(100f, 230f);
        //ghostNavMesh.stoppingDistance = UnityEngine.Random.Range(5,35);
        GameStarted  = true;
        

        
    }

    // Update is called once per frame

    private void GoToBrides()
    {   
        Debug.Log("imgo to brides");
        //Console.WriteLine(randomtime);
        ghostNavMesh.destination = Gazebo;

    }
    /*private void sayEEK()
    {   
        float waittime = UnityEngine.Random.Range(2f,6f);
        float speed = (player.transform.position - transform.position).magnitude;
        var targetrotation = Quaternion.LookRotation(player.transform.position - transform.position);
        EEK = true;

    }
    private void resume()
    {
        EEK = false;
    }*/
}
