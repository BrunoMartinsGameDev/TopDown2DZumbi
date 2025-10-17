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
        if (enemiesRemainingInWave <= 0)
        {
            OnWaveCompleted();
        }
    }

    void OnWaveCompleted()
    {
        int reward = CalculateWaveReward(currentWave);
        AddMoney(reward);
        UiManager.instance?.ShowWaveCompleted(currentWave, reward);
    }

    int CalculateWaveReward(int wave)
    {
        return 50 + (wave * 25); // Wave 1 = 75, Wave 2 = 100, etc
    }

    public int GetCurrentMoney() => currentMoney;
    public int GetCurrentWave() => currentWave;
}
