using UnityEngine;

public class ShieldItem : Items, IPickable
{
    public void Action(PlayerShip player)
    {
        if (boxCollider)
        {
            boxCollider.enabled = false;
        }

        animator.SetTrigger(CollectKey);

        player.InstallShieldModule();


       AudioManager.PlaySound(itemData.CollectedSoundSFX);

        Invoke("DestroyNow", .2f);
    }
}
