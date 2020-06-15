using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public struct Mission
{
    public int ID;
    public int SpriteID;
    public int Level;
    public string Title;
    public string Description;

    public Mission(int iD, int spriteID, int level, string title, string description)
    {
        ID = iD;
        SpriteID = spriteID;
        Level = level;
        Title = title;
        Description = description;
    }
}
