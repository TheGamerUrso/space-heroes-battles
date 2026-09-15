using UnityEngine;

public class ItemPickedUpEvent
{
    public ItemEnum ItemType;
    public object Ammount;

    public ItemPickedUpEvent(ItemEnum itemType, object Ammount)
    {
        this.ItemType = itemType;
        this.Ammount = Ammount;
    }
}
