using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimationScript : MonoBehaviour

{
    Animator animator;
    UnityEngine.AI.NavMeshAgent navmeshagent;
    Beehaviours beehaviours;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        navmeshagent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        beehaviours = GetComponent<Beehaviours>();
    }

    // Update is called once per frame
    void Update()
    {   
        bool isWaiting = beehaviours.IsWaiting;
        float speed = navmeshagent.velocity.magnitude;
        animator.SetFloat("Speed",speed);
        if (beehaviours.IsWaiting)
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
