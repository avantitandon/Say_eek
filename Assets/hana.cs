using UnityEngine;
using UnityEngine.AI;
using System;
public class hana : MonoBehaviour
{
    public Transform StageSpotlight;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventManager.StagePlusBhaddie  += OnStage;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnStage()
    {
        transform.position = StageSpotlight.transform.position;
    }
}
