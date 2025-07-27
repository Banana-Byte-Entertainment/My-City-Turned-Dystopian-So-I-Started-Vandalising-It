using UnityEngine;

public class NPCAnimationChanger : MonoBehaviour
{
    // Variables
    private Animator animator;

    public void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void Dance()
    {
        // Change the NPC's animation to a dance animation
        animator.CrossFade("NPCDance", 0.1f);
    }

    public void Walk()
    {
        // Change the NPC's animation to a dance animation
        animator.CrossFade("NPCWalking", 0.1f);
    }
}
