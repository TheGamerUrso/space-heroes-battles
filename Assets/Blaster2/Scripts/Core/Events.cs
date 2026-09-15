using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShakeCameraEvent
{

    public float duration;
    public ShakeCameraEvent(float duration = .5f)
    {
        this.duration = duration;
    }
}


public class FinishedSceneLoadEvent
{
    public string LevelName;
    public FinishedSceneLoadEvent(string LevelName)
    {
        this.LevelName = LevelName;
    }
}

public class LoadingProgressEvent
{
    public float progress;
    public LoadingProgressEvent(float progress)
    {
        this.progress = progress;
    }
}


