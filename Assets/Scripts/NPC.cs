using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System;
using TMPro;
using UnityEngine.Audio;

public class NPC : MonoBehaviour
{
    public float movementSpeed = 3.0f;
    public float rotationSpeed = 120.0f;
    public float walkRadius = 10.0f;
    public float minIdleTime = 1.0f;
    public float maxIdleTime = 5.0f;

    private Vector3 targetPosition;
    private bool isMoving = false;
    private CharacterController controller;
    private float verticalVelocity = 0.0f;
    public string[] dialogueMessages;

    [Header("Dialogue")]
    public TextMeshPro dialogueTextPrefab; // Assign a TextMeshPro prefab for the text
    public Vector3 dialogueOffset = new Vector3(0, 2, 0); // Offset above the NPC's head
    public float dialogueDisplayTime = 3f;
    private NPCAnimationChanger animationChanger;

    private Coroutine _dialogueCoroutine;
    private float loadTimer = 1f;
    public AudioResource[] voiceLines;
    private AudioSource audioSource;

    void Start()
    {
        animationChanger = GetComponent<NPCAnimationChanger>();
        controller = GetComponent<CharacterController>();
        audioSource = GetComponentInChildren<AudioSource>();
        audioSource.volume = 0.8f;
        if (controller == null)
        {
            Debug.LogError("CharacterController component not found on NPC. Please add one.");
            enabled = false; // Disable script if no controller
            return;
        }
        StartCoroutine(Wander());
    }

    private TextMeshPro dialogueInstance;

    void Update()
    {
        if (isMoving)
        {
            Move();
        }

        // Make the dialogue always face the camera
        if (dialogueInstance != null && Camera.main != null)
        {
            dialogueInstance.transform.rotation = Quaternion.LookRotation(dialogueInstance.transform.position - Camera.main.transform.position);
        }
    }

    void Move()
    {
        if (loadTimer > 0)
        {
            loadTimer -= Time.deltaTime;
            return;
        }

        // Use a 2D distance check on the XZ plane for arrival.
        float distanceToTarget = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(targetPosition.x, 0, targetPosition.z));
        if (distanceToTarget < controller.radius)
        {
            isMoving = false;
            if (gameObject.tag != "Cow") animationChanger.Dance();
            return;
        }

        Vector3 direction = targetPosition - transform.position;

        // Prevent normalization of a zero vector if we are already at the target.
        if (direction.sqrMagnitude < 0.001f)
        {
            isMoving = false;
            return;
        }

        direction.Normalize();
        Vector3 move = new Vector3(direction.x * movementSpeed, 0, direction.z * movementSpeed);

        // Apply gravity
        if (controller.isGrounded)
        {
            verticalVelocity = -0.5f; // Small downward force to keep grounded
        }
        else
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }

        // Clamp vertical velocity to prevent extreme values.
        verticalVelocity = Mathf.Clamp(verticalVelocity, -100f, 100f);

        move.y = verticalVelocity;

        // Final check for NaN before passing the vector to controller.Move().
        if (float.IsNaN(move.x) || float.IsNaN(move.y) || float.IsNaN(move.z))
        {
            Debug.LogWarning("Move vector is NaN, skipping movement to prevent error.");
            isMoving = false; // Stop moving to reset state.
            return;
        }

        controller.Move(move * Time.deltaTime);

        Vector3 lookDirection = direction;
        lookDirection.y = 0;
        if (lookDirection.sqrMagnitude > 0.001f) // Use sqrMagnitude for efficiency
        {
            Quaternion toRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }

    IEnumerator Wander()
    {
        yield return new WaitForSeconds(loadTimer);

        while (true)
        {
            SetNewRandomDestination();
            isMoving = true;

            float stuckTimer = 0;
            float maxStuckTime = 5.0f; // 5 seconds timeout

            yield return new WaitUntil(() =>
            {
                stuckTimer += Time.deltaTime;
                if (stuckTimer > maxStuckTime)
                {
                    return true; // Timed out, break the wait
                }
                return !isMoving;
            });

            if (stuckTimer > maxStuckTime)
            {
                Debug.Log("NPC was stuck, finding new destination.");
                isMoving = false;
                if (gameObject.tag != "Cow") animationChanger.Dance();
            }

            float idleTime = UnityEngine.Random.Range(minIdleTime, maxIdleTime);
            yield return new WaitForSeconds(idleTime);
        }
    }

    void SetNewRandomDestination()
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * walkRadius;
        randomDirection += transform.position;
        NavMeshHit navHit;
        if (NavMesh.SamplePosition(randomDirection, out navHit, walkRadius, -1))
        {
            targetPosition = navHit.position;
        }
        else
        {
            // If we can't find a new position, just stay put for this cycle.
            targetPosition = transform.position;
        }
        if (gameObject.tag != "Cow") animationChanger.Walk();
    }

    public void DisplayDialogue()
    {
        if (dialogueMessages == null || dialogueMessages.Length == 0 || dialogueTextPrefab == null)
        {
            return;
        }

        if (_dialogueCoroutine != null)
        {
            StopCoroutine(_dialogueCoroutine);
            if (dialogueInstance != null)
            {
                Destroy(dialogueInstance.gameObject);
            }
        }
        _dialogueCoroutine = StartCoroutine(ShowDialogueRoutine());
    }

    private IEnumerator ShowDialogueRoutine()
    {
        var rand = UnityEngine.Random.Range(0, dialogueMessages.Length);
        dialogueInstance = Instantiate(dialogueTextPrefab, transform.position + dialogueOffset, Quaternion.identity, transform);
        dialogueInstance.text = dialogueMessages[rand];

        audioSource.resource = voiceLines[rand];
        audioSource.Play();

        yield return new WaitForSeconds(dialogueDisplayTime);

        if (dialogueInstance != null)
        {
            Destroy(dialogueInstance.gameObject);
        }
        _dialogueCoroutine = null;
    }
}
