using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class EnemyStats : MonoBehaviour, IShottable
{
    public EnemyStatsData statsData;
    public AudioClip idleSound;
    public UnityEvent onDeath;
    private int _currentHealth = 100;
    private AudioSource audioSource;
    public int CurrentHealth {
        get{ return _currentHealth; }
        set{
            _currentHealth = value;
            if(_currentHealth <= 0)
            {
                // Lógica para quando o inimigo morre
                Debug.Log($"{gameObject.name} died.");
                ChangeSprite(statsData != null ? statsData.deathSprite : null);
                audioSource.Stop();
                
                // Para o ataque antes de invocar onDeath
                EnemyAttack attack = GetComponent<EnemyAttack>();
                if (attack != null)
                    attack.OnEnemyDeath();
                
                // Desativa a IA
                EnemyFollowAI ai = GetComponent<EnemyFollowAI>();
                if (ai != null)
                    ai.enabled = false;
                
                // Notifica o GameManager
                if (GameManager.Instance != null)
                    GameManager.Instance.OnEnemyKilled();
                
                onDeath?.Invoke();
            }
        } }

    void Start()
    {
        if (statsData != null)
            CurrentHealth = Mathf.RoundToInt(statsData.maxHealth);
        else
            CurrentHealth = 100; // Valor padrão se statsData não estiver definido
        audioSource = GetComponent<AudioSource>();
        if (idleSound != null && audioSource != null)
        {
            audioSource.clip = idleSound;
            audioSource.loop = true;
            audioSource.Play();
            audioSource.volume = 0.5f;
            audioSource.pitch = Random.Range(0.8f, 1.2f);
        }
    }

    public void GetShot(float damage)
    {
        CurrentHealth -= Mathf.RoundToInt(damage);
        Debug.Log($"{gameObject.name} was shot and took {damage} damage.");
    }

    public float GetMoveSpeed()
    {
        return statsData != null ? statsData.moveSpeed : 3f;
    }

    public void ChangeSprite(Sprite newSprite)
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = newSprite;
            sr.sortingOrder = -10;
        }
    }
}
