using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private float zRot;
    private float yRot;
    private Vector3 TargetRot;
    public float rotVel;
    public float distFromCenter;

    public bool GetAnimationState(string id)
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        return animator.GetCurrentAnimatorStateInfo(0).IsName(id);
    }

    public void Exit()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        animator.SetTrigger("Exit");
    }
}