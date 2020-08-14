using System;

[Serializable]
public struct LevelObjectiveData
{

    public int ID;
    public string description;
    public bool completed;

    public LevelObjectiveData(int id, string desc, bool completed = false)
    {
        ID = id;
        description = desc;
        this.completed = completed;
    }
}
