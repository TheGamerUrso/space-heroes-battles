using System;
using UnityEngine;

public interface ILoadingService 
{
    public static Action<int> OnLevelValueChanged;
}
