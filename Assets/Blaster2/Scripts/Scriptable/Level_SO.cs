using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Level", menuName = "Create Level")]
public class Level_SO : ScriptableObject
{

    public bool HasBoss;
    public int numberOfEnemiesEachWave;
    [Range(1, 16)]
    public int waves;
    [Range(1, 7)]
    public int availableEnemies;
    [Range(1, 20)]
    public int LevelDifficulty = 1;
    public GameObject BossPrefab;

}
