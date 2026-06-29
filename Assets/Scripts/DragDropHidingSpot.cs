using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DragDropHidingSpot : MonoBehaviour
{
    public bool isDragging = false;
    private Vector2 offset;
    private Camera mainCamera;
    private Rigidbody2D rb;

    [Header("Death Abyss Settings")]
    [Tooltip("Batas kordinat Y di mana Bush dianggap jatuh ke jurang")]
    public float deadZoneY = -10f; 
    
    private Vector3 startPosition;

    void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        
        startPosition = transform.position; 

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic; 
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        // FUNGSI BARU: Mencegah Player dan Musuh menendang/mendorong bush
        IgnoreSolidCollisionWithTag("Player");
        IgnoreSolidCollisionWithTag("Enemy"); 
    }

    void Update()
    {

        bool isMousePressed = Mouse.current.leftButton.isPressed;

        if (isMousePressed)
        {
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mouseWorldPos.z = 0f;

            if (!isDragging)
            {
                RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

                // Pastikan yang diklik adalah Bush ini
                if (hit.collider != null && hit.collider.transform.root.gameObject == gameObject)
                {
                    isDragging = true;
                    offset = (Vector2)transform.position - (Vector2)mouseWorldPos;
                    
                    if (rb != null)
                    {
                        rb.bodyType = RigidbodyType2D.Kinematic;
                        rb.linearVelocity = Vector2.zero; 
                    }
                }
            }

            if (isDragging)
            {
                Vector2 targetPosition = (Vector2)mouseWorldPos + offset;
                transform.position = targetPosition;
            }
        }
        else
        {
            if (isDragging) 
            {
                isDragging = false;
                
                if (rb != null)
                {
                    rb.bodyType = RigidbodyType2D.Dynamic;
                }
            }
        }
    }

    // --- LOGIKA ANTI DORONG ---
    // Mengabaikan tabrakan padat dengan objek tertentu, tapi tetap mendeteksi Trigger
    private void IgnoreSolidCollisionWithTag(string targetTag)
    {
        // Cari semua objek di scene yang memiliki Tag tersebut (Player / Enemy)
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
        Collider2D[] myColliders = GetComponents<Collider2D>(); // Ambil kedua collider milik Bush

        foreach (GameObject target in targets)
        {
            Collider2D[] targetColliders = target.GetComponents<Collider2D>();
            
            foreach (Collider2D tCol in targetColliders)
            {
                foreach (Collider2D myCol in myColliders)
                {
                    // Jika collider Bush ini BUKAN trigger (merupakan fisik padat), abaikan tabrakannya!
                    if (!myCol.isTrigger) 
                    {
                        Physics2D.IgnoreCollision(tCol, myCol, true);
                    }
                }
            }
        }
    }
}