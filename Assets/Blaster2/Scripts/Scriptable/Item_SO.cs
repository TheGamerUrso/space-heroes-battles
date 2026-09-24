using UnityEngine;
[CreateAssetMenu(menuName = "Create New Item")]
public class Item_SO : ScriptableObject
{
    public ItemEnum itemType;
    public int ammount;
    public AudioClip CollectedSoundSFX;
}
