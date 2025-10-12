using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyStats))]
public class EnemyFollowAI : MonoBehaviour
{
    [Header("AI Settings")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float stopDistance = 1.5f;
    [SerializeField] private LayerMask obstacleLayerMask = 1;
    [SerializeField] private float pathUpdateInterval = 0.5f;
    [SerializeField] private float gridSize = 1f;
    
    [Header("Components")]
    private Rigidbody2D rb2D;
    private Transform target;
    private EnemyStats enemyStats;
    
    [Header("Pathfinding")]
    private List<Vector2> currentPath = new List<Vector2>();
    private int currentPathIndex = 0;
    private float lastPathUpdate = 0f;
    private Vector2 lastTargetPosition;
    
    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        if (rb2D == null)
        {
            rb2D = gameObject.AddComponent<Rigidbody2D>();
        }
        rb2D.gravityScale = 0f;
        rb2D.freezeRotation = false; // Permite rotação para virar na direção

        enemyStats = GetComponent<EnemyStats>();
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
    
    private void FixedUpdate()
    {
        if (target == null) return;
        
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
        
        // Atualiza o caminho se necessário
        if (ShouldUpdatePath())
        {
            UpdatePath();
        }
        
        // Move seguindo o caminho
        MoveAlongPath();
    }
    
    private bool ShouldUpdatePath()
    {
        // Atualiza se passou o tempo ou se o target se moveu significativamente
        return Time.time - lastPathUpdate > pathUpdateInterval ||
               Vector2.Distance(lastTargetPosition, target.position) > gridSize;
    }
    
    private void UpdatePath()
    {
        lastPathUpdate = Time.time;
        lastTargetPosition = target.position;
        
        Vector2 startPos = transform.position;
        Vector2 targetPos = target.position;
        
        // Primeiro tenta caminho direto
        if (HasDirectPath(startPos, targetPos))
        {
            currentPath.Clear();
            currentPath.Add(targetPos);
            currentPathIndex = 0;
        }
        else
        {
            // Usa A* simplificado
            currentPath = FindPathAStar(startPos, targetPos);
            currentPathIndex = 0;
        }
    }
    
    private bool HasDirectPath(Vector2 start, Vector2 end)
    {
        RaycastHit2D hit = Physics2D.Raycast(start, (end - start).normalized, 
                                            Vector2.Distance(start, end), obstacleLayerMask);
        return hit.collider == null;
    }
    
    private void MoveAlongPath()
    {
        if (currentPath.Count == 0) return;
        
        if (currentPathIndex >= currentPath.Count)
        {
            rb2D.linearVelocity = Vector2.zero;
            return;
        }
        
        Vector2 targetPoint = currentPath[currentPathIndex];
        Vector2 direction = (targetPoint - (Vector2)transform.position).normalized;
        
        // Se chegou perto do ponto atual, vai para o próximo
        if (Vector2.Distance(transform.position, targetPoint) < gridSize * 0.5f)
        {
            currentPathIndex++;
            return;
        }
        float moveSpeed = enemyStats != null ? enemyStats.GetMoveSpeed() : 3f;
        rb2D.linearVelocity = direction * moveSpeed;
        
        // Rotaciona para a direção do movimento
        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward); // -90f para ajustar orientação do sprite
        }
    }
    
    private List<Vector2> FindPathAStar(Vector2 start, Vector2 goal)
    {
        List<Vector2> path = new List<Vector2>();
        
        // A* simplificado para performance
        List<Node> openSet = new List<Node>();
        List<Node> closedSet = new List<Node>();
        
        Node startNode = new Node(start);
        Node goalNode = new Node(goal);
        
        openSet.Add(startNode);
        
        int maxIterations = 100; // Limita para evitar travamento
        int iterations = 0;
        
        while (openSet.Count > 0 && iterations < maxIterations)
        {
            iterations++;
            
            // Encontra o nó com menor F cost
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost)
                    currentNode = openSet[i];
            }
            
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);
            
            // Se chegou ao objetivo
            if (Vector2.Distance(currentNode.position, goal) < gridSize)
            {
                return RetracePath(startNode, currentNode);
            }
            
            // Verifica vizinhos (8 direções)
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;
                    
                    Vector2 neighborPos = currentNode.position + new Vector2(x * gridSize, y * gridSize);
                    
                    // Verifica se é walkable
                    if (Physics2D.OverlapCircle(neighborPos, gridSize * 0.4f, obstacleLayerMask))
                        continue;
                    
                    // Verifica se já está na closed list
                    if (IsInClosedSet(neighborPos, closedSet))
                        continue;
                    
                    Node neighbor = new Node(neighborPos);
                    neighbor.gCost = currentNode.gCost + Vector2.Distance(currentNode.position, neighborPos);
                    neighbor.hCost = Vector2.Distance(neighborPos, goal);
                    neighbor.parent = currentNode;
                    
                    // Se não está na open list ou tem um G cost melhor
                    Node existingNode = GetNodeFromOpenSet(neighborPos, openSet);
                    if (existingNode == null)
                    {
                        openSet.Add(neighbor);
                    }
                    else if (neighbor.gCost < existingNode.gCost)
                    {
                        existingNode.gCost = neighbor.gCost;
                        existingNode.parent = currentNode;
                    }
                }
            }
        }
        
        // Se não encontrou caminho, vai direto
        path.Add(goal);
        return path;
    }
    
    private List<Vector2> RetracePath(Node startNode, Node endNode)
    {
        List<Vector2> path = new List<Vector2>();
        Node currentNode = endNode;
        
        while (currentNode != startNode)
        {
            path.Add(currentNode.position);
            currentNode = currentNode.parent;
        }
        
        path.Reverse();
        return path;
    }
    
    private bool IsInClosedSet(Vector2 position, List<Node> closedSet)
    {
        foreach (Node node in closedSet)
        {
            if (Vector2.Distance(node.position, position) < gridSize * 0.1f)
                return true;
        }
        return false;
    }
    
    private Node GetNodeFromOpenSet(Vector2 position, List<Node> openSet)
    {
        foreach (Node node in openSet)
        {
            if (Vector2.Distance(node.position, position) < gridSize * 0.1f)
                return node;
        }
        return null;
    }
    
    private void OnDrawGizmos()
    {
        // Desenha o caminho atual
        if (currentPath.Count > 0)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < currentPath.Count - 1; i++)
            {
                Gizmos.DrawLine(currentPath[i], currentPath[i + 1]);
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

public class Node
{
    public Vector2 position;
    public float gCost;
    public float hCost;
    public float fCost { get { return gCost + hCost; } }
    public Node parent;
    
    public Node(Vector2 pos)
    {
        position = pos;
    }
}
