using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Achievement
{
    public int ID;
    public string Name;
    public string Description;
    public Sprite Icon;
    public bool completed;
    public int progress;
    public int requirement;

    public bool Check()
    {
        if (requirement >= progress)
        {
            return true;
        }
        return false;
    }

    public void Complete()
    {
        completed = true;
    }
}
