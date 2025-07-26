using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PlayerMovement2 : MonoBehaviour
{
    public GameObject freeCamera;
    public float movementSpeed;
    public float jumpHeight;
    private Rigidbody rb;
    public PlayerInputActions playerControls;
    private InputAction move;
    private InputAction jump;
    private Vector2 directionInput;
    private Vector3 moveDirection;
    private Vector3 cameraForward;
    private Vector3 cameraRight;
    private bool jumpQueued;
    private bool isGrounded;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;

    private void Awake()
    {
        playerControls = new PlayerInputActions();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    void Start()
    {
        move = playerControls.Player.Move;
        jump = playerControls.Player.Jump;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (jump.triggered)
        {
            jumpQueued = true;
        }
    }

    void FixedUpdate()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

        cameraForward = freeCamera.transform.forward;
        cameraRight = freeCamera.transform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        directionInput = move.ReadValue<Vector2>();
        moveDirection = (directionInput.x * cameraRight) + (directionInput.y * cameraForward);

        rb.linearVelocity = new Vector3(moveDirection.x * movementSpeed, rb.linearVelocity.y, moveDirection.z * movementSpeed);

        if (moveDirection != Vector3.zero)
        {
            transform.LookAt(new Vector3(moveDirection.x, 0, moveDirection.z) + transform.position);
        }

        if (jumpQueued && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        }

        jumpQueued = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "GraffitiSpot")
        {
            other.GetComponent<GraffitiEvent>().StartEvent();
        }
    }
}
