using UnityEngine;

public class Constants
{
    public static string ObjectiveDataPath = Application.persistentDataPath + "/ObjectiveData.save";
    public static string GameDataPath = Application.persistentDataPath + "/gameData.save";
    public static string PlayerUpgradeDataPath = Application.persistentDataPath + "/playerUpgradeData.save";
    public static string LevelProgresssDataPath = Application.persistentDataPath + "/LevelData.save";

    // private static string Horizontal = "Horizontal";
    //private static string Vertical = "Vertical";
    //public static string m_FireButtonString = "Fire1";
    //public static string m_FirePlayer2ButtonString = "Xbox360_X";
    //public static string m_FirePlayer2Joystick1ButtonString = "Xbox360_1_X";

    public static string PLAYERLEVELSTRINGKEY = "Level";
    public static string PLAYERCURRENTXP = "CurrentXP";
    public static string PLAYERXPTOLEVEL = "XpToLevel";
    public static string m_DropItemHolder = "DropItemHolder";

    public static string m_ProjectileHolderString = "ProjectileHolder";
    public static string m_EnemyHolderString = "EnemyHolder";

    //Normal Controls
    public static string HorizontalAxisStringKey = "Horizontal";

    public static string VerticalAxisStringKey = "Vertical";

    //Xbox Controller
    //public static string m_Xbox360HorizontalAxisString = "Xbox360Horizontal";

    //public static string m_Xbox360VerticalAxisString = "Xbox360Vertical";

    //Player 2
    //public static string m_Player2HorizontalAxisString = "Horizontal2";

    // public static string m_Player2VerticalAxisString = "Vertical2";

    //PlayerAnimationStringKeys
    public static string PLAYERENTERSTRINGKEY = "Enter";

    public static string PLAYEREXITSTRINGKEY = "Exit";

    //Tag Names
    public static string ENEMYTAG = "Enemy";

    public static string PLAYTERTAG = "Player";
    public static string FRIENDLYPROJECTILETAG = "PlayerProjectile";
    public static string ENEMYPROJECTILETAG = "EnemyProjectile";
    public static string ENEMYLAISERTAG = "EnemyLaiser";

    //WEBGL
    //public static float m_TiltSpeed = 25f;
    //public static float m_XMax = 60;
    //public static float m_XMin = -60;
    //public static float m_ZMax = 80;
    //public static float m_ZMin = -80;

    //Android
    public static float m_TiltSpeed = 25f;

    public static float m_XMax = 20;
    public static float m_XMin = -20;
    public static float m_ZMax = 150;
    public static float m_ZMin = -12;
    //Missions
    public static string KillObjectiveKey = "KillObjective1";

    public static string KillObjectiveKeyTwo = "KillObjective2";
    public static string UseObjectiveKey = "UseObjective1";
    public static string UseObjectiveKeyTwo = "UseObjective2";
    public static string UnharmedObjectiveKey = "UnharmedObjective1";
    public static string UnharmedObjectiveKeyTwo = "UnharmedObjective2";
    public static string SurviveObjectiveKey = "SurviveObjective1";
    public static string SurviveObjectiveKeyTwo = "SurviveObjective2";
    public static string SpendObjectiveKey = "SpendObjective1";
    public static string SpendObjectiveKeyTwo = "SpendObjective2";



    /**
     * Upgrade Element
     */
    public static string UpgradeMaxedOut = "Maxed";

    public static string OutOfStock = "Installed";

    public static string UnlockedAtLvl = "Unlocked at Lv ";

    public static string CannotAffordIt = "Insufficient funds";

}

