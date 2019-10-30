[System.Serializable]
public class GameSettings
{
    public float SFXVolume;
    public float MusicVolume;
    public bool AutoAttack;
    public bool mute;
    public float distance;

    public GameSettings()
    {
        SFXVolume = .7f;
        MusicVolume = .7f;
        AutoAttack = true;
        this.mute = false;
        this.distance = 5;
    }

    public GameSettings(float sFXVolume, float musicVolume, bool autoAttack, bool mute, float distance)
    {
        SFXVolume = sFXVolume;
        MusicVolume = musicVolume;
        AutoAttack = autoAttack;
        this.mute = mute;
        this.distance = distance;
    }

}