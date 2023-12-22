using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateControl : MonoBehaviour
{
    public Animator animator;
    
    public void OpenGate()
    {
        animator.SetBool("IsOpen",true);
    }

    public void CloseGate()
    {
        animator.SetBool("IsOpen",false);
    }
}
