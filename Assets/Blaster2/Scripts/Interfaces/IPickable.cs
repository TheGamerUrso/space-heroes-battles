using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPickable
{
    string ID { get;}
    void PickUp();
}
