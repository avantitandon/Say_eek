using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimationScript : MonoBehaviour

{
    Animator animator;
    UnityEngine.AI.NavMeshAgent navmeshagent;
    beezakaAI beezakaAI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        navmeshagent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        beezakaAI = GetComponent<beezakaAI>();
    }

    // Update is called once per frame
    void Update()
    {   
        bool isWaiting = beezakaAI.IsWaiting;
        float speed = navmeshagent.velocity.magnitude;
        animator.SetFloat("Speed",speed);
        if (beezakaAI.IsWaiting)
        {
            animator.SetBool("isWaiting",true);
        }
        else
        {
            animator.SetBool("isWaiting",false);
            return;
        }
     
    }
}
