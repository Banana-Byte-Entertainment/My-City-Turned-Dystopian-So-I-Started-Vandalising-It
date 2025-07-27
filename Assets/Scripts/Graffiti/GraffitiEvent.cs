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
    PlayerMovement playerMovement = mainPlayer.GetComponent<PlayerMovement>();
    grafitiEvent = true;
    // playerMovement.Freeze();
    originalSpeed = playerMovement.movementSpeed;
    playerMovement.movementSpeed = 0;
    Debug.Log("GraffitiEvent: Changing camera position to grafiti");
    CinemachineCamera freeCamera = graffitiCam.GetComponent<CinemachineCamera>();
    freeCamera.Priority = 2;
    grafitiCanvas.GetComponent<GraffitiCanvas>().canDraw = true;
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
  }

  public void EndEvent()
  {
    if (grafitiEvent == false)
      return;
    Debug.Log("GraffitiEvent: Unlocking player");
    PlayerMovement playerMovement = mainPlayer.GetComponent<PlayerMovement>();
    playerMovement.movementSpeed = originalSpeed;
    // playerMovement.Unfreeze();
    grafitiEvent = false;
    Debug.Log("GraffitiEvent: Changing camera position to original");
    CinemachineCamera freeCamera = graffitiCam.GetComponent<CinemachineCamera>();
    freeCamera.Priority = 0;
    grafitiCanvas.GetComponent<GraffitiCanvas>().canDraw = false;
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
    mainPlayer.GetComponent<GraffitiCounter>().addOneCount();
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
