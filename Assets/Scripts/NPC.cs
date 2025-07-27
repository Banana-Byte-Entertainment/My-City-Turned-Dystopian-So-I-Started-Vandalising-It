using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System;
using TMPro;

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
    public GameObject dialogueBox; // Assign a UI panel/image for the background
    public TextMeshProUGUI dialogueText; // Assign a TextMeshPro UGUI element
    public float dialogueDisplayTime = 3f;

    private Coroutine _dialogueCoroutine;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            Debug.LogError("CharacterController component not found on NPC. Please add one.");
            enabled = false; // Disable script if no controller
            return;
        }
        if (dialogueBox != null)
        {
            dialogueBox.SetActive(false);
        }
        StartCoroutine(Wander());
    }

    void Update()
    {
        if (isMoving)
        {
            Move();
        }
    }

    void Move()
    {
        if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(targetPosition.x, 0, targetPosition.z)) < controller.radius)
        {
            isMoving = false;
            return;
        }

        Vector3 direction = (targetPosition - transform.position).normalized;
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

        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);

        Vector3 lookDirection = direction;
        lookDirection.y = 0;
        if (lookDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }

    IEnumerator Wander()
    {
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
        NavMesh.SamplePosition(randomDirection, out navHit, walkRadius, -1);
        targetPosition = navHit.position;
    }

    public void DisplayDialogue()
    {
        if (dialogueMessages == null || dialogueMessages.Length == 0 || dialogueBox == null || dialogueText == null)
        {
            return;
        }

        if (_dialogueCoroutine != null)
        {
            StopCoroutine(_dialogueCoroutine);
        }
        _dialogueCoroutine = StartCoroutine(ShowDialogueRoutine());
    }

    private IEnumerator ShowDialogueRoutine()
    {
        dialogueBox.SetActive(true);
        dialogueText.text = dialogueMessages[UnityEngine.Random.Range(0, dialogueMessages.Length)];
        yield return new WaitForSeconds(dialogueDisplayTime);
        dialogueBox.SetActive(false);
        _dialogueCoroutine = null;
    }
}
