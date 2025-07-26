using UnityEngine;
using System.Collections.Generic; // Required for List

public class PressPause : MonoBehaviour
{
    private PlayerInputActions playerControls;
    public GameObject pause;
    private bool change = false;

    private void Awake()
    {
        playerControls = new PlayerInputActions();
        playerControls.Enable();
        pause.gameObject.SetActive(change);
    }

    void Update()
    {
        if (playerControls.Player.Previous.triggered)
        {
            pause.gameObject.SetActive(change);
            if (change)
            {
                change = false;
            }
            else
            {
                change = true;
            }
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
}
