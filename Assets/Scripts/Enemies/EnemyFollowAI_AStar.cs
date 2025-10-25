using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyStats))]
[RequireComponent(typeof(Seeker))]
public class EnemyFollowAI_AStar : MonoBehaviour
{
    [Header("AI Settings")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float stopDistance = 1.5f;
    [SerializeField] private float nextWaypointDistance = 0.5f;
    [SerializeField] private float pathUpdateInterval = 0.5f;
    
    [Header("Components")]
    private Rigidbody2D rb2D;
    private Transform target;
    private EnemyStats enemyStats;
    
    [Header("A* Pathfinding")]
    private Seeker seeker;
    private Path path;
    private int currentWaypoint = 0;
    private float lastPathUpdate = 0f;
    
    public float GetStopDistance()
    {
        return stopDistance;
    }
    
    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        if (rb2D == null)
        {
            rb2D = gameObject.AddComponent<Rigidbody2D>();
        }
        rb2D.gravityScale = 0f;
        rb2D.freezeRotation = false;

        enemyStats = GetComponent<EnemyStats>();
        seeker = GetComponent<Seeker>();
        
        seeker ??= gameObject.AddComponent<Seeker>();
        
        if (enemyStats == null)
        {
            Debug.LogError("EnemyStats não encontrado no inimigo!");
        }
        
        // Encontra o jogador automaticamente
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
    }
    
    private void Start()
    {
        // Calcula o primeiro caminho
        if (target != null)
        {
            UpdatePath();
        }
    }
    
    private void FixedUpdate()
    {
        if (target == null || path == null) return;
        
        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        
        // Se está muito longe, não faz nada
        if (distanceToTarget > detectionRange) 
        {
            rb2D.linearVelocity = Vector2.zero;
            return;
        }
        
        // Se está muito perto, para
        if (distanceToTarget <= stopDistance)
        {
            rb2D.linearVelocity = Vector2.zero;
            return;
        }
        
        // Atualiza o caminho periodicamente
        if (Time.time - lastPathUpdate > pathUpdateInterval)
        {
            UpdatePath();
        }
        
        // Move seguindo o caminho
        MoveAlongPath();
    }
    
    private void UpdatePath()
    {
        lastPathUpdate = Time.time;
        
        if (seeker.IsDone())
        {
            seeker.StartPath(transform.position, target.position, OnPathComplete);
        }
    }
    
    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
        else
        {
            Debug.LogWarning("Erro ao calcular caminho: " + p.errorLog);
        }
    }
    
    private void MoveAlongPath()
    {
        if (path == null || currentWaypoint >= path.vectorPath.Count)
        {
            rb2D.linearVelocity = Vector2.zero;
            return;
        }
        
        // Verifica se está perto o suficiente do alvo final para parar
        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        if (distanceToTarget <= stopDistance)
        {
            rb2D.linearVelocity = Vector2.zero;
            return;
        }
        
        Vector2 targetPoint = path.vectorPath[currentWaypoint];
        Vector2 direction = (targetPoint - (Vector2)transform.position).normalized;
        
        // Se chegou perto do waypoint atual, vai para o próximo
        float distanceToWaypoint = Vector2.Distance(transform.position, targetPoint);
        if (distanceToWaypoint < nextWaypointDistance)
        {
            currentWaypoint++;
            return;
        }
        
        // Move na direção do waypoint
        float moveSpeed = enemyStats != null ? enemyStats.GetMoveSpeed() : 3f;
        rb2D.linearVelocity = direction * moveSpeed;
        
        // Rotaciona para a direção do movimento (apenas se estiver se movendo)
        if (direction != Vector2.zero && rb2D.linearVelocity.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
        }
    }
    
    private void OnDrawGizmos()
    {
        // Desenha o caminho atual
        if (path != null && path.vectorPath != null)
        {
            Gizmos.color = Color.cyan;
            for (int i = currentWaypoint; i < path.vectorPath.Count - 1; i++)
            {
                Gizmos.DrawLine(path.vectorPath[i], path.vectorPath[i + 1]);
            }
        }
        
        // Desenha range de detecção
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Desenha distância de parada
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}
