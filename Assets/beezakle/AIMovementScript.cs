using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovementScript : MonoBehaviour
{   
    public float movementSpeed = 10f;
    public float rotationSpeed = 50f;

    private bool isWandering = false;
    private bool isRotatingLeft = false;
    private bool isRotatingRight = false;
    public bool isWalking = false;

    Rigidbody rb;
    Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isWandering == false)
        {
            StartCoroutine(Wander());
        }
        if (isRotatingRight == true)
        {
            transform.Rotate(transform.up * Time.deltaTime * rotationSpeed);
        }
        if (isRotatingLeft == true)
        {
            transform.Rotate (transform.up * Time.deltaTime * -rotationSpeed);
        }
        if (isWalking == true)
        {
            rb.AddForce(transform.forward * movementSpeed);
            animator.SetBool("isRunning", true);
        }
        if (isWalking == false)
        {
            animator.SetBool("isRunning", false);
        }
    }
    IEnumerator Wander()
    {
        int rotationTime = Random.Range(1,1);
        int rotateWait = Random.Range(2,3);
        int rotateDirection =Random.Range(1,2);
        int walkWait = Random.Range(2,3);
        int walkTime = Random.Range(1,3);

        isWandering = true;

        yield return new WaitForSeconds (walkWait);

        isWalking = true;

        yield return new WaitForSeconds (walkTime);

        isWalking = false;

        yield return new WaitForSeconds (rotateWait);

        if(rotateDirection == 1)
        {
            isRotatingLeft = true;
            yield return new WaitForSeconds (rotationTime);
            isRotatingLeft = false;
        }
        
        if(rotateDirection == 2)
        {
            isRotatingRight = true;
            yield return new WaitForSeconds (rotationTime);
            isRotatingRight = false;
        }

        isWandering = false;
    }
}
