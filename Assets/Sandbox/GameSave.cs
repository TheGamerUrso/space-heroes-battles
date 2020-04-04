using System;
using System.Collections.Generic;

[Serializable]
public class GameSave
{
    public static float[] Score;
    public static float[] HighScore;
    public static int Coins;
    public static int TotalKills;
    public static int LevelUnlocked;
    public static int TotalMoneySpend;
    public static int TotalSuperUsed;
    public static int WaveSurvived;
    public static int m_EnemyKilled;
    public static bool GotHitInGame;
    public static bool PlayedGame;
    public static int currentSelectedShip;
    public static int[] UnlockedHeroes;

    public static Dictionary<string, LevelObjectiveData[]> ListOfLevelChallenges;
    public static List<ObjectiveData> ListOfOnGoingObjectives;

    public static PlayerShipData[] playerShipData;

    public static float SFXVolume;
    public static float MusicVolume;
    public static bool AutoAttack;
    public static bool mute;
    public static float distance;
    public static int[] Upgrades;
}
