using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] waypoints; // Titik A dan B di udara
    public float patrolSpeed = 2f;
    private int currentWaypointIndex = 0;

    [Header("Chase Settings")]
    public float chaseSpeed = 4f;
    private bool isChasing = false;
    private GameObject playerTarget;

    private bool facingRight = true;

    void Update()
    {
        if (isChasing && playerTarget != null)
        {
            ChasePlayer();
        }
        else
        {
            PatrolLogic();
        }
    }

    void PatrolLogic()
    {
        if (waypoints.Length == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        
        // Bergerak ke arah waypoint di udara
        transform.position = Vector2.MoveTowards(transform.position, targetWaypoint.position, patrolSpeed * Time.deltaTime);

        HandleFlip(targetWaypoint.position.x);

        if (Vector2.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    void ChasePlayer()
    {
        // Berbeda dengan musuh darat, musuh terbang mengejar posisi X DAN Y pemain secara langsung
        transform.position = Vector2.MoveTowards(transform.position, playerTarget.transform.position, chaseSpeed * Time.deltaTime);
        
        HandleFlip(playerTarget.transform.position.x);
    }

    // Fungsi ini dipanggil saat musuh melihat player (Bisa dihubungkan dengan script Vision milikmu)
    public void ChaseTarget(GameObject target)
    {
        Debug.Log("Musuh Terbang melihat player! Mulai mengejar!");
        playerTarget = target;
        isChasing = true;
    }

    // Fungsi ini dipanggil saat player masuk tempat sembunyi
    public void StopChasing()
    {
        Debug.Log("Player sembunyi! Musuh Terbang kembali patroli.");
        isChasing = false;
        playerTarget = null;
    }

    public bool IsChasing()
    {
        return isChasing;
    }

    void HandleFlip(float targetX)
    {
        if (targetX > transform.position.x && !facingRight) Flip();
        else if (targetX < transform.position.x && facingRight) Flip();
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
}