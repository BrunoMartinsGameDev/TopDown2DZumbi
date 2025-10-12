using UnityEngine;

[CreateAssetMenu(menuName = "Spawner/WaveData")]
public class WaveData : ScriptableObject
{
    public float initialSpawnInterval = 1f;
    public float spawnRateIncrement = 0.1f;
    public int maxActiveEnemies = 20;
    public EnemySpawnInfo[] enemies;
}

[System.Serializable]
public class EnemySpawnInfo
{
    public EnemyStatsData statsData;
    public GameObject enemyPrefab;
    public int amount;
}
