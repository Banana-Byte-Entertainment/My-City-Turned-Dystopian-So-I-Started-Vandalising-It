using UnityEngine;
using System.Collections.Generic; // Required for List

public class PickUpObj : MonoBehaviour // typo fix: Monobehaviour → MonoBehaviour
{
    private PlayerInputActions playerControls;
    private List<int> songs = new List<int>();
    private int showcaseCounter = 0;

    private void Awake()
    {
        playerControls = new PlayerInputActions();
    }

    void Start()
    {
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) //playerControls.Player.Crouch.triggered && 
        {
            Debug.Log("Interact");
            Playlist.instance.Add(showcaseCounter.ToString());
            showcaseCounter++;
            this.gameObject.SetActive(false);
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

/*
public GameObject FlashLightOnPlayer;
@ Unity Message | 0 references void Start)
FlashLightOnPlayer SetActive(false);
旦
© Unity Message | 0 references
private void OnTriggerStay(Collider other)
if(other gameObject.tag -= "Player")
if (Input.GetKey(KeyCode.E))
｛
this gameObject.SetActive(false);
FlashLightOnPlayer.SetActive(true);
*/