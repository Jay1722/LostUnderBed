using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyFlee : MonoBehaviour
{
    [Header("Movement Settings")]
    public float chaseSpeed = 4f;       // Kecepatan pas ngejar player (agresif)
    public float fleeSpeed = 2f;        // Kecepatan pas kabur/mundur karena takut senter

    private Rigidbody2D rb;
    private Transform playerTransform;
    private float lightCheckTimer;
    private bool isScared = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Otomatis cari objek Player di dalam scene
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // Hitung arah dari musuh ke player
        Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;

        if (isScared)
        {
            // --- LOGIKA KABUR (Kena Senter) ---
            // Arahnya dibalik (negatif) menjauh dari player
            Vector2 fleeDirection = -directionToPlayer;
            rb.linearVelocity = new Vector2(fleeDirection.x * fleeSpeed, rb.linearVelocity.y);

            // Balik arah hadap sprite menjauh dari player
            if (fleeDirection.x > 0.1f) transform.localScale = new Vector3(1, 1, 1);
            else if (fleeDirection.x < -0.1f) transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            // --- LOGIKA NGEJAR (Kondisi Gelap / Normal) ---
            rb.linearVelocity = directionToPlayer *chaseSpeed;

            // Balik arah hadap sprite menghadap ke player
            if (directionToPlayer.x > 0.1f) transform.localScale = new Vector3(1, 1, 1);
            else if (directionToPlayer.x < -0.1f) transform.localScale = new Vector3(-1, 1, 1);
        }

        // Timer buat ngecek apakah player masih nyenterin atau udah enggak
        if (lightCheckTimer > 0)
        {
            lightCheckTimer -= Time.deltaTime;
            if (lightCheckTimer <= 0)
            {
                isScared = false; // Senter lepas, waktunya ngejar lagi!
                Debug.Log("Senter lepas! Monster ngejar lagi!");
            }
        }
    }

    // Fungsi ini dipanggil terus-menerus oleh FlashlightController selama kena sorot
    public void GetScaredByLight()
    {
        if (!isScared)
        {
            isScared = true;
            Debug.Log("Monster kesorot senter! Kabuuur!");
        }

        // Kunci timer di angka 0.2 detik selama masih di dalam area senter
        lightCheckTimer = 0.2f;
    }
}