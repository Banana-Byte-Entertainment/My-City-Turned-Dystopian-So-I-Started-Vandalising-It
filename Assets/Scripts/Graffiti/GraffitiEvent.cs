using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class GraffitiEvent : MonoBehaviour
{
  public GameObject graffitiCam;
  public GameObject mainPlayer;
  private float originalSpeed;
  public GameObject grafitiCanvas;
  private bool grafitiEvent = false;

  public void StartEvent()
  {
    Debug.Log("GraffitiEvent: Locking player");
    PlayerMovement2 playerMovement = mainPlayer.GetComponent<PlayerMovement2>();
    grafitiEvent = true;
    // PlayerMovement.instance.Freeze();
    originalSpeed = playerMovement.movementSpeed;
    playerMovement.movementSpeed = 0;
    Debug.Log("GraffitiEvent: Changing camera position to grafiti");
    CinemachineCamera freeCamera = graffitiCam.GetComponent<CinemachineCamera>();
    freeCamera.Priority = 2;
    grafitiCanvas.GetComponent<GraffitiCanvas>().canDraw = true;
  }

  public void EndEvent()
  {
    Debug.Log("GraffitiEvent: Unlocking player");
    PlayerMovement2 playerMovement = mainPlayer.GetComponent<PlayerMovement2>();
    playerMovement.movementSpeed = originalSpeed;
    // PlayerMovement.instance.Unfreeze();
    grafitiEvent = false;
    Debug.Log("GraffitiEvent: Changing camera position to original");
    CinemachineCamera freeCamera = graffitiCam.GetComponent<CinemachineCamera>();
    freeCamera.Priority = 0;
    grafitiCanvas.GetComponent<GraffitiCanvas>().canDraw = false;
    Destroy(gameObject);
  }

  void Update()
  {
    if (Keyboard.current.escapeKey.wasPressedThisFrame)
    {
      EndEvent();
    }
  }
}
