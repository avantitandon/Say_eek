using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZekeMovement : MonoBehaviour
{   
    public Transform target;
    private NavMeshAgent navmeshagent;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        navmeshagent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        navmeshagent.destination = target.transform.position;
    }
}
