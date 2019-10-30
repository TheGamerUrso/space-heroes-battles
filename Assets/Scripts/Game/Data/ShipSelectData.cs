using UnityEngine;

[CreateAssetMenu(menuName = "Create New Ship Element")]
public class ShipSelectData : ScriptableObject
{
    public string ID;
    public int Cost;
    public Sprite Icon;
}