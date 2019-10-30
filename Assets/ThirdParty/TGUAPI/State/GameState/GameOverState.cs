using UnityEngine;

public class GameOverState : State
{
    public GameOverState(GameManager gameManager) : base(gameManager)
    {
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();

        SpawnEnemies.GameOver = true;
        Player player = PlayerManager.GetPlayer();
        PlayerAnimation playerAnimation = player.PlayerAnimation();

        if (player != null)
        {
            AudioManager.PlaySound("Victory", 3);
            playerAnimation.Exit();
        }
        else if (player == null)
        {
            AudioManager.PlaySound("GameOver", 3);
        }


 
    }

    public override void OnStateExit()
    {
        base.OnStateExit();
    }

    public override void Tick()
    {

    }
}
