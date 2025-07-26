using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTrick : MonoBehaviour
{
    private PlayerInputActions playerControls;
    private PlayerGrind playerGrind;
    private PlayerScore playerScore;
    private bool isTrickActive;

    private void Awake()
    {
        playerControls = new PlayerInputActions();
        playerControls.Enable();

        playerGrind = GetComponent<PlayerGrind>();

        playerScore = GetComponent<PlayerScore>();

        isTrickActive = false;
    }

    private void OnLeftClickTrick(InputValue val)
    {
        if (playerGrind.onRail && val.isPressed && !isTrickActive) {
            isTrickActive = true;
            doLeftClickTrick(); 
            
        }
    }

    private void OnRightClickTrick(InputValue val)
    {
        if (playerGrind.onRail && val.isPressed && !isTrickActive) {
            isTrickActive = true;
            doRightClickTrick();
        }
    }

    void doLeftClickTrick()
    {
        Debug.Log("Left click trick performed on rail!");
        transform.Rotate(0, 180, 0, Space.Self);
        playerScore.PerformTrick(300, "Left Click Trick");
        isTrickActive = false;
    }

    void doRightClickTrick()
    {
        Debug.Log("Right click trick performed on rail!");
        transform.Rotate(0, -180, 0, Space.Self);
        playerScore.PerformTrick(300, "Right Click Trick");
        isTrickActive = false;
    }
}
