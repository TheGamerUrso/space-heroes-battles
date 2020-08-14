[System.Serializable]
public struct GameSettings
{
    public static float SFXVolume;
    public static float MusicVolume;
    public static bool AutoAttack;
    public static bool mute;
    public static float controlSceme;

    public GameSettings(float SFXVolume, float MusicVolume, bool AutoAttack, bool mute, float controlSceme)
    {
        GameSettings.SFXVolume = SFXVolume;
        GameSettings.MusicVolume = MusicVolume;
        GameSettings.AutoAttack = AutoAttack;
        GameSettings.mute = mute;
        GameSettings.controlSceme = controlSceme;
    }

    public static void ResetToDefault()
    {
        SFXVolume = .7f;
        MusicVolume = .7f;
        AutoAttack = true;
        mute = false;
        controlSceme = 1;
    }



}