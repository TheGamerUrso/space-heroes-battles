using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateControl : MonoBehaviour
{
    public Animator animator;
    
    public void OpenGate()
    {
        animator.ResetTrigger("Close");
        animator.SetTrigger("Open");
    }

    public void CloseGate()
    {
        animator.ResetTrigger("Open");
        animator.SetTrigger("Close");
    }
}
