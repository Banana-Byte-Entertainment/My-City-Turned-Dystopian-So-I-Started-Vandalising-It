using System;
using UnityEngine;
using UnityEngine.Audio;
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
    public bool canMove = true;
    public float _rideHeight = 0.001f;
    private bool jumpQueued;
    private bool isGrounded;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.2f;
    private PlayerGrind playerGrind;
    private bool isOnRail = false;
    public Animator hoverboardAnimator;
    public AudioSource hoverBoardSoundsSource;
    public AudioResource[] jumpSounds;
    public AudioSource jumpSoundSource;

    private void Awake()
    {
        playerControls = new PlayerInputActions();

        playerControls.Player.Move.performed += OnMove;
        playerControls.Player.Move.canceled += OnMove;

        playerControls.Player.Jump.performed += OnJump;
        playerControls.Player.Jump.canceled += OnJump;

        playerControls.Player.Sprint.performed += OnSprint;
        playerControls.Player.Sprint.canceled += OnSprint;

        playerControls.Player.AddCallbacks(this);
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
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance, groundLayer, QueryTriggerInteraction.Ignore);

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

        if (rb.linearVelocity.sqrMagnitude > 0.1f && hoverBoardSoundsSource.isPlaying == false)
            hoverBoardSoundsSource.Play();
        else if (rb.linearVelocity.sqrMagnitude < 0.1f)
            hoverBoardSoundsSource.Stop();

        if (jumpQueued && (playerGrind.onRail || isGrounded))
        {
            jumpSoundSource.resource = jumpSounds[UnityEngine.Random.Range(0, jumpSounds.Length)];
            jumpSoundSource.Play();
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
        if (context.performed && !jumpQueued)
        {
            GetComponent<Animator>().CrossFade("SkateboardingPushing", 0.1f);
            hoverboardAnimator.CrossFade("HoverboardPushing", 0.1f);
        }
        else if (context.canceled)
        {
            GetComponent<Animator>().CrossFade("SkateboardingIdle", 0.1f);
            hoverboardAnimator.CrossFade("HoverboardIdle", 0.1f);
        }
    }

    public void OnLook(InputAction.CallbackContext context) { }

    public void OnLeftClickTrick(InputAction.CallbackContext context) { }

    public void OnInteract(InputAction.CallbackContext context) { }

    public void OnCrouch(InputAction.CallbackContext context) { }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && jumpQueued)
        {
            //GetComponent<Animator>().CrossFade("SkateboardingJump", 0.1f);
            //hoverboardAnimator.CrossFade("HoverboardJump", 0.1f);
        }
        else if (context.canceled)
        {
            GetComponent<Animator>().CrossFade("SkateboardingPushing", 0.1f);
            hoverboardAnimator.CrossFade("HoverboardPushing", 0.1f);
        }
    }

    public void OnPrevious(InputAction.CallbackContext context) { }

    public void OnNext(InputAction.CallbackContext context) { }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //GetComponent<Animator>().CrossFade("SkateboardingFast", 0.1f);
            //hoverboardAnimator.CrossFade("HoverboardFast", 0.1f);
        }
        else if (context.canceled)
        {
            GetComponent<Animator>().CrossFade("SkateboardingIdle", 0.1f);
            hoverboardAnimator.CrossFade("HoverboardIdle", 0.1f);
        }
    }

    public void OnRightClickTrick(InputAction.CallbackContext context) { }
}
