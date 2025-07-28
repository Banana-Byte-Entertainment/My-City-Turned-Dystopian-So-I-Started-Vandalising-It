using UnityEngine;
using System.Collections.Generic;
using System; // Required for List

public class PressPause : MonoBehaviour
{
    private PlayerInputActions playerControls;
    public GameObject pause;
    private bool change = false;
    private GameObject[] graffitiEvents;
    private bool tempCanPause = true;
    public bool canPause = true;
    private float timeMax = 0.2f;
    private float time;

    private void Awake()
    {
        playerControls = new PlayerInputActions();
        playerControls.Enable();
        pause.gameObject.SetActive(change);

        time = timeMax;
    }

    void Update()
    {
        graffitiEvents = GameObject.FindGameObjectsWithTag("GraffitiSpot");
        
        if (time > 0)
        {
            time -= Time.deltaTime;
            tempCanPause = true;
        }
        else
        {
            foreach (GameObject g in graffitiEvents)
            {
                canPause = false;
                if (g.GetComponent<GraffitiEvent>().grafitiEvent == true)
                {
                    tempCanPause = false;
                    break;
                }
                else
                {
                    canPause = true;
                }
            }
            time = timeMax;
        }

        if (playerControls.Player.Pause.triggered && canPause == true && tempCanPause == true)
        {
            change = !change;
            pause.gameObject.SetActive(change);
            if (!change)
            {
                if (Cursor.visible != false)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
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
