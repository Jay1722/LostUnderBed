using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Hiding Spot Transparency")]
    [Range(0f, 1f)] public float targetAlpha = 0.4f;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public bool isHiding = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HidingSpot"))
        {
            isHiding = true;

            EnemyPatrol[] enemies = FindObjectsByType<EnemyPatrol>(
                FindObjectsSortMode.None
            );

            foreach (EnemyPatrol enemy in enemies)
            {
                if (enemy.IsChasing())
                {
                    enemy.StopChasing();
                }
            }

            spriteRenderer.sortingOrder = -1;

            Physics2D.IgnoreLayerCollision(
                LayerMask.NameToLayer("Player"),
                LayerMask.NameToLayer("Enemy"),
                true
            );

            SpriteRenderer spotRenderer = collision.GetComponent<SpriteRenderer>();

            if (spotRenderer != null)
            {
                Color spotColor = spotRenderer.color;
                spotColor.a = targetAlpha;
                spotRenderer.color = spotColor;
            }

            Debug.Log("Player otomatis tersembunyi di dalam bayangan.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Jika Player berjalan keluar dari area objek ber-tag "HidingSpot"
        if (collision.CompareTag("HidingSpot"))
        {
            isHiding = false;

            
            spriteRenderer.sortingOrder = 1;
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
            SpriteRenderer spotRenderer = collision.GetComponent<SpriteRenderer>();
            if(spotRenderer != null)
            {
                Color spotColor = spotRenderer.color;
                spotColor.a = 1f; // Kembalikan ke transparansi penuh
                spotRenderer.color = spotColor;
            }
            Debug.Log("Player keluar ke area terang! Bahaya deteksi aktif.");
        }
    }

    public void SetHiding(bool state)
    {
        isHiding = state;

        if (isHiding)
        { 
            Debug.Log("Player tersembunyi di dalam bayangan.");
        }
        else
        {
            Debug.Log("Player keluar ke area terang! Bahaya deteksi aktif.");
        }
    }
}