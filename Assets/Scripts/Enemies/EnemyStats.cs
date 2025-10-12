using UnityEngine;
using UnityEngine.Events;

public class EnemyStats : MonoBehaviour, IShottable
{
    public EnemyStatsData statsData;
    public UnityEvent onDeath;
    private int _currentHealth = 100;
    public int CurrentHealth {
        get{ return _currentHealth; }
        set{
            if(_currentHealth <= 0)
            {
                // Lógica para quando o inimigo morre
                Debug.Log($"{gameObject.name} died.");
                ChangeSprite(statsData != null ? statsData.deathSprite : null);
                onDeath?.Invoke();
            }
            _currentHealth = value;
        } }

    void Start()
    {
        if (statsData != null)
            CurrentHealth = Mathf.RoundToInt(statsData.maxHealth);
        else
            CurrentHealth = 100; // Valor padrão se statsData não estiver definido
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
