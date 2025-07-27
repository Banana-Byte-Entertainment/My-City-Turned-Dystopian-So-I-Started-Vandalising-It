using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour, PlayerInputActions.IPlayerActions
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
    private bool canMove = true;
    public float _rideHeight = 0.1f;
    private bool jumpQueued;
    private bool isGrounded;
    public LayerMask groundLayer;
    public float groundCheckDistance = 1f;
    private PlayerGrind playerGrind;
    private bool isOnRail = false;

    private void Awake()
    {
        playerControls = new PlayerInputActions();
    }

    private void OnEnable()
    {
        playerControls.Enable();
        playerControls.Player.Move.performed += OnMove;
        playerControls.Player.Move.canceled += OnMove;

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
        playerGrind = GetComponent<PlayerGrind>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (jump.triggered)
        {
            jumpQueued = true;
        }
    }

    public void Freeze()
    {
        canMove = false;
    }

    public void Unfreeze()
    {
        canMove = true;
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance, groundLayer);

        if (isGrounded && hit.collider.CompareTag("Rail"))
            isOnRail = true;
        else
            isOnRail = false;

        cameraForward = freeCamera.transform.forward;
        cameraRight = freeCamera.transform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        if (canMove)
        {
            directionInput = move.ReadValue<Vector2>();
            moveDirection = (directionInput.x * cameraRight) + (directionInput.y * cameraForward);

            rb.linearVelocity = new Vector3(moveDirection.x * movementSpeed, rb.linearVelocity.y, moveDirection.z * movementSpeed);

            transform.LookAt(new Vector3(moveDirection.x, 0, moveDirection.z) + transform.position);
        }

        if (jumpQueued && isGrounded)
        {
            if (isOnRail)
                playerGrind.ThrowOffRail();
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

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            GetComponentInChildren<Animator>().CrossFade("Idle1", 0);
        }
        else if (context.performed)
        {
            GetComponentInChildren<Animator>().CrossFade("Walk", 0);
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnPrevious(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnNext(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnRightClickTrick(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }
}
