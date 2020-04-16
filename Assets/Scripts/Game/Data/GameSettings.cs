[System.Serializable]
public class GameSettings
{
    public static float SFXVolume;
    public static float MusicVolume;
    public static bool AutoAttack;
    public static bool mute;
    public static float distance;

    public static void Initialize()
    {
        SFXVolume = .7f;
        MusicVolume = .7f;
        AutoAttack = true;
        mute = false;
        distance = 5;
    }

    public static void Initialize(float sFXVolume, float musicVolume, bool autoAttack, bool mute, float distance)
    {
        GameSettings.SFXVolume = sFXVolume;
        GameSettings.MusicVolume = musicVolume;
        GameSettings.AutoAttack = autoAttack;
        GameSettings.mute = mute;
        GameSettings.distance = distance;
    }

}