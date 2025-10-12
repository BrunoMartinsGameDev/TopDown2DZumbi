using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BulletBehavior : MonoBehaviour
{
    private float damage = 10f;
    private Rigidbody2D rb2D;
    public float speed = 20f;
    public void SetDamage(float dmg)
    {
        damage = dmg;
    }

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        rb2D.linearVelocity = transform.up * speed;
        Destroy(gameObject, 5f); // Destroi a bala após 5 segundos
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IShottable>(out var shottable))
        {
            shottable.GetShot(damage);
        }
        // Aqui você pode adicionar lógica para aplicar dano a inimigos ou outros objetos
        Debug.Log($"Bullet hit {collision.gameObject.name} for {damage} damage.");
        Destroy(gameObject); // Destroi a bala ao colidir
    }

}
