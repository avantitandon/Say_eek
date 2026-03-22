using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;




public class MovementStateManager : MonoBehaviour
{
    // CONSTANTS //

    public const float WALK_SPEED = 15f;
    public const float CAMERA_UP_WALK_SPEED = 7f;

    // AUDIO //
    [SerializeField] private AudioManager audioManager;

    // GAME COMPONENTS //
    [SerializeField] private CameraController cameraController;

    // coming from the player object itself
    [SerializeField] private CharacterController characterController;

    // VARIABLES //

    // LOGGING //
   
    [SerializeField] public float moveSpeed;

    [SerializeField] private float groundYOffset = 0.1f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float gravity = -9.81f;

    private Vector3 velocity;
    private Vector3 movementDirection;
    private Vector3 spherePosition;


    // LOGGING //

    [Header("Playtest Logging")]
    [SerializeField] private bool enablePlaytestLogs = true;
    // Flag to track if footsteps are currently playing
    private bool isPlayingFootsteps = false;



    private void Awake()
    {
        if (characterController == null)
        {
            Debug.LogError("CharacterController component is missing!");
            enabled = false;
            return;
        }
    }

    private void Start()
    {
        moveSpeed = WALK_SPEED;
    }


    private void Update()
    {

    }

    public void MovePlayer(Vector2 input)
    {
        // Calculate movement direction
        GetDirection(input);


        // Apply gravity
        ApplyGravity();
        // Combine movement and gravity, then move the character
        Vector3 finalMove = movementDirection * moveSpeed + velocity;
        characterController.Move(finalMove * Time.deltaTime);
        // Handle footstep sounds
        audioManager.HandleFootsteps(movementDirection);
    }

    public void SetWalkSpeed()
    {
        moveSpeed = WALK_SPEED;
    }

    public void SetCameraUpWalkSpeed()
    {
        moveSpeed = CAMERA_UP_WALK_SPEED;
    }


    private bool IsGrounded()
    {
        spherePosition = new Vector3(transform.position.x, transform.position.y - groundYOffset, transform.position.z);
        return Physics.CheckSphere(spherePosition, characterController.radius - 0.05f, groundMask);
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
            characterController != null ? characterController.radius - 0.05f : 0.5f
        );
    }
    private void GetDirection(Vector2 moveValue)
    {
        Vector3 camForward = cameraController.GetCameraForward();
        Vector3 camRight = cameraController.GetCameraRight();


        movementDirection = camForward * moveValue.y + camRight * moveValue.x;


        if (movementDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }

    private void LogPlaytest(string message)
    {
        if (!enablePlaytestLogs && !PlaytestLogWriter.RuntimeLoggingEnabled)
        {
            return;
        }

        PlaytestLogWriter.Log("Movement", message);
    }

}
