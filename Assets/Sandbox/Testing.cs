using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.Utils;
using UnityEngine;

public class Testing : MonoBehaviour
{
    private FunctionTimer eventHandler;
    private void Start()
    {
        FunctionTimer.Create(TestingAction, 3f, "Timer");
        FunctionTimer.Create(TestingAction_2, 4f, "Timer_2");


        FunctionTimer.StopTimer("Timer");
    }

    private void TestingAction()
    {
        //Debug.Log("Testing!");
    }

    private void TestingAction_2()
    {
        //Debug.Log("Testing2!");
    }
}