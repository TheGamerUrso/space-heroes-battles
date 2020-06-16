using UnityEngine;
[CreateAssetMenu(menuName = "Create New Item")]
public class ItemData : ScriptableObject
{
    public int m_RewardAmount;
    public bool Shield;
    public bool PowerPack;
    public int m_HealValue;
    public AudioClip CollectedSoundSFX;
    public bool ShowTutorial;
}
