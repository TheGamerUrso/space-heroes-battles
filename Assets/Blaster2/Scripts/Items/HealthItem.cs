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
            player.Heal(itemData.m_HealValue);
        }

        AudioManager.PlaySound(itemData.CollectedSoundSFX);

        Invoke("DestroyNow", .2f);
    }
}
