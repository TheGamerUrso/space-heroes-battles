using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPickable
{
    ItemEnum ID { get;}
    void PickUp();
}
