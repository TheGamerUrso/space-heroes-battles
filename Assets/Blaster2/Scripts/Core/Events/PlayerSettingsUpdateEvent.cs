using UnityEngine;

public class PlayerSettingsUpdateEvent 
{
    public enum StatType
    {
        None,
        SFXVolume,
        MusicVolume,
        AutoAttack,
        mute,
        ControlScene
    }

    public StatType type;
    public float value;
}
