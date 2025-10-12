using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStatsData", menuName = "Enemies/EnemyStatsData", order = 1)]
public class EnemyStatsData : ScriptableObject
{
    public float maxHealth = 100f;
    public float moveSpeed = 3f;
    public float damage = 10f;
    public int coinDrop = 1;
    public Sprite deathSprite;
    // Adicione outros stats conforme necessário
}
