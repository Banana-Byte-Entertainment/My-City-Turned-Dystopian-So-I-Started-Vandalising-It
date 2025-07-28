using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
  public float interactionRadius = 3.0f;
  public float interactionDelay = 3.0f;
  private float delayTimer = 0;

  void Update()
  {
    // if (Keyboard.current.eKey.wasPressedThisFrame)
    // {
    // TryInteract();
    // }

    if (delayTimer <= 0)
    {
      TryInteract();
      delayTimer = interactionDelay;
    }
    else
    {
      delayTimer -= Time.deltaTime;
    }
  }

  void TryInteract()
  {
    Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRadius);
    NPC closestNpc = null;
    float minDistance = float.MaxValue;

    foreach (var hitCollider in hitColliders)
    {
      NPC npc = hitCollider.GetComponent<NPC>();
      if (npc != null)
      {
        float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
        if (distance < minDistance)
        {
          minDistance = distance;
          closestNpc = npc;
        }
      }
    }

    if (closestNpc != null)
    {
      closestNpc.DisplayDialogue();
    }
  }
}
