using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(AudioSource))]
public class PlayerStats : MonoBehaviour
{
    public PlayerStatsData statsData;
    
    private float currentHealth;
    private AudioSource audioSource;
    private bool isDead = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (statsData != null)
        {
            currentHealth = statsData.maxHealth;
            UiManager.instance?.UpdateHealthUI(currentHealth, statsData.maxHealth);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        
        // Toca som de dano
        if (statsData != null && statsData.damageSound != null)
            audioSource.PlayOneShot(statsData.damageSound);
        
        // Atualiza UI
        UiManager.instance?.UpdateHealthUI(currentHealth, statsData.maxHealth);
        
        // Verifica morte
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        audioSource.PlayOneShot(statsData != null && statsData.deathSound != null ? statsData.deathSound : null);
        // Desativa movimento
        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null)
            movement.enabled = false;
        
        // Pausa o jogo e mostra game over
        Time.timeScale = 0f;
        UiManager.instance?.ShowGameOver();
        UiManager.instance?.ShowDeathSprite();
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => statsData != null ? statsData.maxHealth : 100f;
    public bool IsDead() => isDead;
}
