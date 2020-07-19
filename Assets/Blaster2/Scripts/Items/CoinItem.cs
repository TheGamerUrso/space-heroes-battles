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
            Game.CoinPicked += 1;
        }


        audioSource.PlayOneShot(itemData.CollectedSoundSFX);
        

        Invoke("DestroyNow", .2f);
    }
}
