using UnityEngine;
using UnityEngine.InputSystem;

public class DragDropHidingSpot : MonoBehaviour
{
    public bool isDragging = false;
    private Vector2 offset;
    private Camera mainCamera;
    private Rigidbody2D rb;

    [Header("Floor Settings")]
    public LayerMask groundLayer; // Pilih layer lantai di Inspector
    private float halfObjectHeight;

    void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();

        // Atur otomatis Rigidbody agar tipenya Kinematic (aman untuk drag-and-drop)
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        // Hitung setengah tinggi objek berdasarkan ukuran Sprite-nya agar pas di atas lantai
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            halfObjectHeight = sr.bounds.extents.y;
        }
        else
        {
            halfObjectHeight = 0.5f; // Nilai default jika tidak ada sprite
        }
    }

    void Update()
    {
        // Deteksi apakah klik kiri mouse sedang ditekan
        bool isMousePressed = Mouse.current.leftButton.isPressed;

        if (isMousePressed)
        {
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mouseWorldPos.z = 0f;

            // Jika baru pertama kali klik mengenai objek kardus
            if (!isDragging)
            {
                RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    isDragging = true;
                    // Hitung jarak antara posisi mouse dan pusat objek agar tidak lompat saat di-drag
                    offset = (Vector2)transform.position - (Vector2)mouseWorldPos;
                }
            }

            // SELAMA DI-DRAG: Pindahkan posisi mengikuti mouse
            if (isDragging)
            {
                Vector2 targetPosition = (Vector2)mouseWorldPos + offset;

                // LOGIKA MATEMATIKA ANTI-TEMBUS LANTAI
                RaycastHit2D groundHit = Physics2D.Raycast(new Vector2(targetPosition.x, targetPosition.y + 2f), Vector2.down, 10f, groundLayer);
                if (groundHit.collider != null)
                {
                    float minY = groundHit.point.y + halfObjectHeight;
                    targetPosition.y = Mathf.Max(targetPosition.y, minY);
                }

                // Aplikasikan pergerakan drag
                transform.position = targetPosition;
            }
        }
        else
        {
            // Jika klik kiri mouse dilepas, status drag selesai
            isDragging = false;
        }
    }
}