using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private EnemyStats enemyStats;
    private float lastAttackTime = 0f;
    private const float attackInterval = 0.2f;
    private PlayerStats targetPlayer;
    private bool isAttacking = false;
    private bool isDead = false;

    void Awake()
    {
        enemyStats = GetComponentInParent<EnemyStats>();
        if (enemyStats == null)
        {
            Debug.LogError($"EnemyAttack on {gameObject.name}: EnemyStats not found!");
        }
        else if (enemyStats.statsData == null)
        {
            Debug.LogError($"EnemyAttack on {gameObject.name}: statsData is null!");
        }
        
        // Garante que existe um trigger collider para detectar o jogador
        CircleCollider2D attackCollider = GetComponent<CircleCollider2D>();
        if (attackCollider == null)
        {
            attackCollider = gameObject.AddComponent<CircleCollider2D>();
        }
        attackCollider.isTrigger = true;
        attackCollider.radius = enemyStats.gameObject.GetComponent<EnemyFollowAI_AStar>().GetStopDistance(); // Raio de ataque
    }

    void Update()
    {
        if (isDead) return;
        
        if (isAttacking && targetPlayer != null && !targetPlayer.IsDead())
        {
            if (Time.time >= lastAttackTime + attackInterval)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }
    }

    void Attack()
    {
        if (isDead) return;
        
        if (targetPlayer != null && enemyStats != null && enemyStats.statsData != null)
        {
            float damage = enemyStats.statsData.damage;
            targetPlayer.TakeDamage(damage);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;
        
        PlayerStats player = other.GetComponent<PlayerStats>();
        if (player != null)
        {
            StartAttacking(player);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        PlayerStats player = other.GetComponent<PlayerStats>();
        if (player != null && player == targetPlayer)
        {
            StopAttacking();
        }
    }

    public void StartAttacking(PlayerStats player)
    {
        if (isDead) return;
        
        targetPlayer = player;
        isAttacking = true;
        lastAttackTime = Time.time;
    }

    public void StopAttacking()
    {
        isAttacking = false;
        targetPlayer = null;
    }

    public void OnEnemyDeath()
    {
        isDead = true;
        StopAttacking();
    }
}
