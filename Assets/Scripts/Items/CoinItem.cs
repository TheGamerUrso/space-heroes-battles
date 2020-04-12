public class CoinItem : Items, IPickable
{

    public void Action(PlayerShip player)
    {
        if (boxCollider)
        {
            boxCollider.enabled = false;
        }

        animator.SetTrigger(CollectKey);

        if (itemData.m_RewardAmount > 0)
        {
            GameSession.CoinEarnInGame+=10;
        }

        if (AudioManager.Instance)
        {
            AudioManager.PlaySound(null, itemData.CollectedSoundSFX, 2);
        }

        Invoke("DestroyNow", .2f);
    }
}
