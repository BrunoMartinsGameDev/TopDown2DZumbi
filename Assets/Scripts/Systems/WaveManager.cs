using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [Header("Spawner Settings")]
    public WaveData[] waves;
    public int currentLevel = 0;
    public Transform[] spawnPoints;

    private WaveData waveData;
    private Coroutine spawnLoopCoroutine;
    private int activeEnemies = 0;
    private int[] enemiesToSpawn;
    private float currentSpawnInterval;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        SetWave(currentLevel);
    }

    public void SetWave(int level)
    {
        if (waves == null || waves.Length == 0 || level < 0 || level >= waves.Length) return;
        waveData = waves[level];
        SetupWave();
    }

    void SetupWave()
    {
        if (waveData == null || waveData.enemies == null) return;
        enemiesToSpawn = new int[waveData.enemies.Length];
        int totalEnemies = 0;
        for (int i = 0; i < waveData.enemies.Length; i++)
        {
            enemiesToSpawn[i] = waveData.enemies[i].amount;
            totalEnemies += waveData.enemies[i].amount;
        }
        currentSpawnInterval = Mathf.Max(0.1f, waveData.initialSpawnInterval);
        activeEnemies = 0;
        
        // Notifica o GameManager
        if (GameManager.Instance != null)
            GameManager.Instance.StartWave(currentLevel + 1, totalEnemies);
    }

    //SERA REMOVIDO DEPOIS
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            StartSpawnLoop();
        }
    }

    public void OnEnemyDeath()
    {
        activeEnemies = Mathf.Max(0, activeEnemies - 1);
    }

    // Spawna um inimigo de um tipo específico
    public void SpawnEnemy(int enemyTypeIndex)
    {
        if (waveData == null || waveData.enemies == null || enemyTypeIndex < 0 || enemyTypeIndex >= waveData.enemies.Length) return;
        if (waveData.enemies[enemyTypeIndex].enemyPrefab == null || spawnPoints == null || spawnPoints.Length == 0) return;
        if (activeEnemies >= waveData.maxActiveEnemies) return;
        if (enemiesToSpawn[enemyTypeIndex] <= 0) return;
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(waveData.enemies[enemyTypeIndex].enemyPrefab, point.position, point.rotation);
        activeEnemies++;
        enemiesToSpawn[enemyTypeIndex]--;
    }

    // Inicia o loop de spawn
    public void StartSpawnLoop()
    {
        spawnLoopCoroutine ??= StartCoroutine(SpawnLoop());
    }

    // Para o loop de spawn
    public void StopSpawnLoop()
    {
        if (spawnLoopCoroutine != null)
        {
            StopCoroutine(spawnLoopCoroutine);
            spawnLoopCoroutine = null;
        }
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            bool spawned = false;
            for (int i = 0; i < enemiesToSpawn.Length; i++)
            {
                if (enemiesToSpawn[i] > 0 && activeEnemies < waveData.maxActiveEnemies)
                {
                    SpawnEnemy(i);
                    spawned = true;
                    yield return new WaitForSeconds(currentSpawnInterval);
                }
            }
            // Se não spawnou ninguém, mas ainda há inimigos para spawnar, espera um pouco e tenta de novo
            if (!spawned)
            {
                bool anyLeft = false;
                for (int i = 0; i < enemiesToSpawn.Length; i++)
                    if (enemiesToSpawn[i] > 0)
                        anyLeft = true;
                if (!anyLeft)
                    break; // acabou todos os inimigos para spawnar
                yield return new WaitForSeconds(0.2f); // espera antes de tentar de novo
            }
        }
        spawnLoopCoroutine = null;
    }
}
