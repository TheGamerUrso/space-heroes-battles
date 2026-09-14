using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [SerializeField] private GameObject LevelUpPrefab;

    private void Start()
    {
      //  Events.OnLevelValueChanged += OnLevelValueChanged;
    }
    protected void OnDestroy()
    {

        //Events.OnLevelValueChanged -= OnLevelValueChanged;
    }

    public void OnLevelValueChanged(int Level)
    {
        Instantiate(LevelUpPrefab);
    }
}
