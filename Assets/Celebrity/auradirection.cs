using System;
using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class auradirection : MonoBehaviour
{
     public Transform target;
     Vector3 vector3;

     void Update()
     {
          /*if(target != null)
          {
            var lookPos = target.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime);
          }*/
          Vector3 newtarget = target.position;
          newtarget.y = transform.position.y;
          var lookPos = target.position - transform.position;
          lookPos.y = 0;
          transform.LookAt(newtarget);
     }
}
