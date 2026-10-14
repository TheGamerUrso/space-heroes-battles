using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPickable
{
    ItemEnum ID { get;}
    bool IsPulled { get; set; }
    GameObject gameObject { get; }
    void PickUp();
}
