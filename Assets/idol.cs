using UnityEngine;
using UnityEngine.AI;
using System;
public class idol : MonoBehaviour
{   public Transform StageSpotlight;
    public Transform UnderStage;
    //Vector3 startposition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventManager.StagePlusBhaddie += OffStage;
        EventManager.StagePlusIdol += OnStage;
        //startposition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnStage()
    {
        transform.position = StageSpotlight.transform.position;
    }
    void OffStage()
    {
        transform.position = UnderStage.transform.position;
    }
}
