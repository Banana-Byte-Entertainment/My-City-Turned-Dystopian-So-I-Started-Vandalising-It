using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerOpenMap : MonoBehaviour
{
    public Camera[] cameras;
    private int currentCameraIndex;

    private PlayerInputActions playerControls;

    private void Awake()
    {
        playerControls = new PlayerInputActions();
    }

    void Start()
    {
        currentCameraIndex = 0;

        // Disable all cameras except the first
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(i == currentCameraIndex);
        }
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    void Update() // Use Update instead of FixedUpdate for input
    {
        if (playerControls.Player.Interact.triggered)
        {
            // Disable current camera
            cameras[currentCameraIndex].gameObject.SetActive(false);

            // Increment and wrap around if needed
            currentCameraIndex = (currentCameraIndex + 1) % cameras.Length;

            // Enable new camera
            cameras[currentCameraIndex].gameObject.SetActive(true);
        }
    }
}