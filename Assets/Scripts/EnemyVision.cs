using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [Header("Detection Settings")]
    public LayerMask playerLayer;
    public LayerMask hidingSpotLayer;

    public Color visionColor = Color.red;
    private PolygonCollider2D visionCollider;
    private MeshFilter meshFilter;
    private Mesh mesh;
    private EnemyPatrol patrolScript;

    void Start()
    {
        patrolScript = GetComponentInParent<EnemyPatrol>();
        if (patrolScript == null)
        {
            patrolScript = GetComponent<EnemyPatrol>();
        }

        visionCollider = GetComponent<PolygonCollider2D>();
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if(meshRenderer != null && meshRenderer.material != null)
        {
            meshRenderer.material.color = visionColor;
            meshRenderer.sortingOrder = -1;
        }
    }

    void Update()
    {
        if (visionCollider != null && mesh != null)
        {
            Vector2[] points = visionCollider.points;
            Vector3[] vertices = new Vector3[points.Length];

            for (int i = 0; i < points.Length; i++)
            {
                vertices[i] = new Vector3(points[i].x, points[i].y, 0);
            }
            int[] triangles = new int[] { 0, 1, 2 };
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
        }
    }
    void OnTriggerStay2D(Collider2D other)
    {
        // 1. CEK PLAYER
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                Debug.Log("isHiding: " + player.isHiding);
                if (player.isHiding)
                {
                    Debug.Log("Player is hiding! Enemy stops chasing.");
                    if (patrolScript != null) patrolScript.StopChasing();
                    return; // KELUAR dari fungsi, musuh tidak jadi mengejar
                }

                // Jika tidak sembunyi, baru boleh ngejar
                Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
                bool isPlayerMoving = playerRb != null && playerRb.linearVelocity.magnitude > 0.1f;

                if (isPlayerMoving)
                {
                    patrolScript.ChaseTarget(player.gameObject);
                }
            }
        }
    }

}