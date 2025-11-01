using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [Header("Spawner Settings")]
    public WaveData[] waves;
    public int currentLevel = 0;
    public Transform[] spawnPoints;
    
    [Header("Wave Progression")]
    [SerializeField] private float timeBetweenWaves = 10f;

    private WaveData waveData;
    private Coroutine spawnLoopCoroutine;
    private int activeEnemies = 0;
    private int[] enemiesToSpawn;
    private float currentSpawnInterval;
    private bool allEnemiesSpawned = false;
    private bool firstWaveStarted = false;

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
        // Só permite iniciar manualmente a primeira wave
        if(!firstWaveStarted && Input.GetKeyDown(KeyCode.E))
        {
            firstWaveStarted = true;
            StartSpawnLoop();
        }
    }

    public void OnEnemyDeath()
    {
        activeEnemies = Mathf.Max(0, activeEnemies - 1);
        
        // Verifica se a wave acabou (todos spawnados e todos mortos)
        if (allEnemiesSpawned && activeEnemies <= 0)
        {
            OnWaveComplete();
        }
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
        allEnemiesSpawned = false;
        
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
                {
                    allEnemiesSpawned = true;
                    Debug.Log("Todos os inimigos foram spawnados!");
                    break; // acabou todos os inimigos para spawnar
                }
                yield return new WaitForSeconds(0.2f); // espera antes de tentar de novo
            }
        }
        spawnLoopCoroutine = null;
        
        // Verifica se a wave já acabou (caso todos tenham sido mortos durante o spawn)
        if (activeEnemies <= 0)
        {
            OnWaveComplete();
        }
    }
    
    private void OnWaveComplete()
    {
        Debug.Log($"Wave {currentLevel + 1} completa!");
        
        // Verifica se era a última wave
        if (currentLevel >= waves.Length - 1)
        {
            OnAllWavesComplete();
        }
        else
        {
            // Só mostra o painel de wave completada se não for a última
            // O GameManager vai mostrar o painel via OnEnemyKilled
            
            // Inicia a próxima wave após o timer
            StartCoroutine(StartNextWaveAfterDelay());
        }
    }
    
    private IEnumerator StartNextWaveAfterDelay()
    {
        Debug.Log($"Próxima wave em {timeBetweenWaves} segundos...");
        
        // Aqui você pode mostrar uma UI de contagem regressiva se quiser
        if (UiManager.instance != null)
        {
            UiManager.instance.ShowNextWaveTimer(timeBetweenWaves);
        }
        
        yield return new WaitForSeconds(timeBetweenWaves);
        
        currentLevel++;
        SetWave(currentLevel);
        StartSpawnLoop();
    }
    
    private void OnAllWavesComplete()
    {
        Debug.Log("Todas as waves completadas! Vitória!");
        
        // Mostra painel de vitória
        if (UiManager.instance != null)
        {
            UiManager.instance.ShowVictoryPanel();
        }
        CameraFollow.Instance.StopShake();
        // Pausa o jogo
        Time.timeScale = 0f;
    }
}
