using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;




public class MovementStateManager : MonoBehaviour
{
    [SerializeField] private ThirdPersonCamera cameraController;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float groundYOffset = 0.1f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float gravity = -9.81f;


    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 movementDirection;
    private Vector3 spherePosition;


    private void Awake()
    {
        // Cache the CharacterController component for performance
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            Debug.LogError("CharacterController component is missing!");
            enabled = false;
            return;
        }
    }


    private void Update()
    {
        // Calculate movement direction
        GetDirection();


        // Apply gravity
        ApplyGravity();
        // Combine movement and gravity, then move the character
        Vector3 finalMove = movementDirection * moveSpeed + velocity;
        controller.Move(finalMove * Time.deltaTime);
    }




    private bool IsGrounded()
    {
        spherePosition = new Vector3(transform.position.x, transform.position.y - groundYOffset, transform.position.z);
        return Physics.CheckSphere(spherePosition, controller.radius - 0.05f, groundMask);
    }


    private void ApplyGravity()
    {
        if (IsGrounded())
        {
            // Reset gravity when grounded
            if (velocity.y < 0)
            {
                velocity.y = -2f; // Slight negative value to ensure contact with the ground
            }
        }
        else
        {
            // Apply gravity over time when not grounded
            velocity.y += gravity * Time.deltaTime;
        }
    }


    private void OnDrawGizmos()
    {
        // Visualize the ground check sphere in the Scene view
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            new Vector3(transform.position.x, transform.position.y - groundYOffset, transform.position.z),
            controller != null ? controller.radius - 0.05f : 0.5f
        );
    }
    private void GetDirection()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");


        Vector3 camForward = cameraController.GetCameraForward();
        Vector3 camRight = cameraController.GetCameraRight();


        movementDirection = camForward * verticalInput + camRight * horizontalInput;


        if (movementDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }

}


