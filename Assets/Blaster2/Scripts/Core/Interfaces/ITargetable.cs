using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetable
{
    GameObject target { get; }
    bool Targetable { get; }
}
