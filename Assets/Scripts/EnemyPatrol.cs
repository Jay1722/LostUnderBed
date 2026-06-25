using System.Collections;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] waypoints;
    public float speed = 3f;
    public float waitTime = 2f;

    [Header("Chase Settings")]
    public float chaseSpeed = 4f;
    private bool isChasing = false;
    private GameObject playerTarget;

    private int currentWaypointIndex = 0;
    private bool isWaiting = false;
    private bool facingRight = true;

    // Kunci Rahasia: Menyimpan koordinat tempat musuh pertama kali ditaruh di map
    //private Vector2 spawnPosition;
    //private bool isReturningToSpawn = false;

    //void Start()
    //{
    //    // Catat posisi awal musuh saat game baru mulai
    //    spawnPosition = transform.position;
    //}

    void Update()
    {
        if (isChasing && playerTarget != null)
        {
            ChaseLogic();
        }
        //else if (isReturningToSpawn)
        //{
        //    ReturnToSpawnLogic();
        //}
        else
        {
            PatrolLogic();
        }
    }

    void PatrolLogic()
    {
        if (isWaiting || waypoints.Length == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        transform.position = Vector2.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);

        HandleFlip(targetWaypoint.position.x);

        if (Vector2.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            StartCoroutine(WaitAtWaypoint());
        }
    }

    void ChaseLogic()
    {
        Vector2 targetPos = new Vector2(playerTarget.transform.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPos, chaseSpeed * Time.deltaTime);

        HandleFlip(playerTarget.transform.position.x);
    }

    // Logika paksa pulang kampung saat player sembunyi
    //void ReturnToSpawnLogic()
    //{
    //    transform.position = Vector2.MoveTowards(transform.position, spawnPosition, speed * Time.deltaTime);
    //    HandleFlip(spawnPosition.x);

    //    // Kalau sudah sampai di posisi semula, baru boleh lanjut patroli normal
    //    if (Vector2.Distance(transform.position, spawnPosition) < 0.2f)
    //    {
    //        isReturningToSpawn = false;
    //        if (waypoints.Length > 0) currentWaypointIndex = 0; // Mulai dari waypoint pertama lagi
    //        Debug.Log("Musuh sudah sampai di rumah, lanjut patroli biasa.");
    //    }
    //}

    void HandleFlip(float targetX)
    {
        if (targetX > transform.position.x && !facingRight) Flip();
        else if (targetX < transform.position.x && facingRight) Flip();
    }

    public void ChaseTarget(GameObject target)
    {
        Debug.Log("chase target dipanggil! Musuh mulai mengejar player yang terlihat.");
        playerTarget = target;
        isChasing = true;
        //isReturningToSpawn = false; // Batalkan pulang kalau ketemu player lagi

        StopAllCoroutines();
        isWaiting = false;
    }

    public void StopChasing()
    {   
        Debug.Log("Musuh berhenti mengejar karena player sembunyi! Memulai proses pulang kampung...");
        if (!isChasing) return; // Biar gak kepanggil berulang-ulang tiap frame

        Debug.Log("Hiding terdeteksi! Musuh dipaksa pulang ke posisi spawn awal!");
        isChasing = false;
        playerTarget = null;

        StopAllCoroutines();
        isWaiting = false;

        // Nyalakan mode pulang kampung
        //isReturningToSpawn = true;
    }

    public bool IsChasing()
    {
        return isChasing;
    }

    IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        isWaiting = false;
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
}