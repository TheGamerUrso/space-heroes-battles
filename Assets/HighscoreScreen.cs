using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighscoreScreen : MonoBehaviour
{
    public LeaderboardScreen leaderboardScreen;
   public void LoadLeaderboard()
   {
        leaderboardScreen.LoadLeaderboard();
   }
}
