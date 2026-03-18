using System;
using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class auradirection : MonoBehaviour
{
     public Transform target;

     void Update()
     {
          if(target != null)
          {
            var lookPos = target.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime);
          }
     }
}
