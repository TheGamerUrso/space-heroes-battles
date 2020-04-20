using System;
using UnityEngine;

public class PowerPack : Items, IPickable
{
    public void Action(PlayerShip player)
    {
        if (boxCollider)
        {
            boxCollider.enabled = false;
        }

        animator.SetTrigger(CollectKey);

        player.PowerUpCollected();


        if (AudioManager.Instance)
        {
            AudioManager.PlaySound(null, itemData.CollectedSoundSFX, 2);
        }

        GuiManager.Instance.PickUpItem(itemData);
        Invoke("DestroyNow", .2f);
    }
}
