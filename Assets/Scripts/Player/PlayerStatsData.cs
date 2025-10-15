using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatsData", menuName = "Player/PlayerStatsData", order = 1)]
public class PlayerStatsData : ScriptableObject
{
    public float maxHealth = 100f;
    public AudioClip damageSound;
    public AudioClip deathSound;
}
