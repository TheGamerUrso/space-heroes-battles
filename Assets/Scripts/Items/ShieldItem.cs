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


        if (AudioManager.Instance)
        {
            AudioManager.PlaySound(null, itemData.CollectedSoundSFX, 2);
        }

        Invoke("DestroyNow", .2f);
    }
}
