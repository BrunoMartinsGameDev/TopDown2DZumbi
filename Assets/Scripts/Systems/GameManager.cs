using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int currentMoney = 0;
    private int currentWave = 0;
    private int enemiesRemainingInWave = 0;

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
        currentMoney = 100; // Define a quantia inicial de dinheiro
        UiManager.instance?.UpdateMoneyUI(currentMoney);
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        UiManager.instance?.UpdateMoneyUI(currentMoney);
    }

    public bool SpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            UiManager.instance?.UpdateMoneyUI(currentMoney);
            return true;
        }
        return false;
    }

    public void StartWave(int waveNumber, int enemyCount)
    {
        currentWave = waveNumber;
        enemiesRemainingInWave = enemyCount;
    }

    public void OnEnemyKilled()
    {
        enemiesRemainingInWave--;
        Debug.Log($"Enemy killed. Enemies remaining in wave: {enemiesRemainingInWave}");
        if (enemiesRemainingInWave <= 0)
        {
            OnWaveCompleted();
        }
    }

    void OnWaveCompleted()
    {
        // Verifica se é a última wave (não mostra painel de wave completada se for)
        if (WaveManager.Instance != null && WaveManager.Instance.currentLevel >= WaveManager.Instance.waves.Length - 1)
        {
            // É a última wave, não mostra painel de wave completada
            // O painel de vitória será mostrado pelo WaveManager
            int reward = CalculateWaveReward(currentWave);
            AddMoney(reward);
            return;
        }
        
        int normalReward = CalculateWaveReward(currentWave);
        AddMoney(normalReward);
        UiManager.instance?.ShowWaveCompleted(currentWave, normalReward);
    }

     // Retorna true se uma wave está ativa (ainda há inimigos na wave)
    public bool IsWaveActive()
    {
        return enemiesRemainingInWave > 0;
    }

    int CalculateWaveReward(int wave)
    {
        return 50 + (wave * 25); // Wave 1 = 75, Wave 2 = 100, etc
    }

    public int GetCurrentMoney() => currentMoney;
    public int GetCurrentWave() => currentWave;
}
