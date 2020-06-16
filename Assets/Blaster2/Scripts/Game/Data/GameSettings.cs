[System.Serializable]
public struct GameSettings
{
    public static float SFXVolume;
    public static float MusicVolume;
    public static bool AutoAttack;
    public static bool mute;
    public static float distance;

    public GameSettings(float SFXVolume, float MusicVolume, bool AutoAttack, bool mute, float distance)
    {
        GameSettings.SFXVolume = SFXVolume;
        GameSettings.MusicVolume = MusicVolume;
        GameSettings.AutoAttack = AutoAttack;
        GameSettings.mute = mute;
        GameSettings.distance = distance;
    }

    public static void ResetToDefault()
    {
        SFXVolume = .7f;
        MusicVolume = .7f;
        AutoAttack = true;
        mute = false;
        distance = 5;
    }



}