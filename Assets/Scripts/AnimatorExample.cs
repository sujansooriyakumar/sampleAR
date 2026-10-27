using UnityEngine;

public class AnimatorExample : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    [ContextMenu("SetTrigger")]
    public void TriggerAnimation()
    {
        animator.SetTrigger("twerkTrigger");
        animator.SetInteger("walkSpeed", 5);
        animator.SetBool("isWalking", true);
    }
}
