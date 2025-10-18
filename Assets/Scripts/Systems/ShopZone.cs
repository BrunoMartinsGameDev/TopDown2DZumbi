using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ShopZone : MonoBehaviour
{
    public Sprite openShopSprite;
    public Sprite closeShopSprite;
    private SpriteRenderer spriteRenderer;
    private bool playerInZone = false;
    private bool shopOpen = true;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateShopSprite();
    }

    void Update()
    {
        UpdateShopSprite();

        if (playerInZone && Input.GetKeyDown(KeyCode.E) && shopOpen)
        {
            ShopManager.Instance.OpenShop();
        }
    }

    void UpdateShopSprite()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsWaveActive())
        {
            spriteRenderer.sprite = closeShopSprite;
            shopOpen = false;
        }
        else
        {
            spriteRenderer.sprite = openShopSprite;
            shopOpen = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInZone = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInZone = false;
    }
}
