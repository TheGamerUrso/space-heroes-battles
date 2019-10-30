using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class LevelObjectiveData {

    public int   ID;
    public string   description;
    public bool     completed;

    public bool Completed
    {
        get
        {
            return completed;
        }
    }

    public LevelObjectiveData(int id,string desc,bool completed = false)
    {
        ID = id;
        description = desc;
        this.completed = completed;
    }
}
