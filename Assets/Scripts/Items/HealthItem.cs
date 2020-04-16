using UnityEngine;

public class HealthItem:Items,IPickable
{
    public void Action(PlayerShip player)
    {
        if (boxCollider)
        {
            boxCollider.enabled = false;
        }

        animator.SetTrigger(CollectKey);

        if (itemData.m_HealValue > 0)
        {
            player.Heal(player.Level * itemData.m_HealValue);
        }

        if (AudioManager.Instance)
        {
            AudioManager.PlaySound(null, itemData.CollectedSoundSFX, 2);
        }

        Invoke("DestroyNow", .2f);
    }
}
