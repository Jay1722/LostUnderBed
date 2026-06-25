using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class CursorDrawer : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject linePrefab;

    private GameObject currentLine;
    private LineRenderer lineRenderer;
    private EdgeCollider2D edgeCollider;
    private Rigidbody2D lineRb;
    private Camera mainCamera;

    private List<Vector2> fingerPositions = new List<Vector2>();

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // 1. Saat klik kiri BARU DITEKAN
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            CreateLine();
        }

        // 2. Saat klik kiri SEDANG DITAHAN
        if (Mouse.current.leftButton.isPressed)
        {
            Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            if (fingerPositions.Count == 0 || Vector2.Distance(mouseWorldPos, fingerPositions[fingerPositions.Count - 1]) > 0.1f)
            {
                if (currentLine != null)
                {
                    UpdateLine(mouseWorldPos);
                }
            }
        }

        // 3. SAAT KLIK KIRI DILEPAS: Aktifkan Fisika / Jadi Pijakan Static
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (lineRb != null)
            {
                lineRb.bodyType = RigidbodyType2D.Static;
            }
        }
    }

    void CreateLine()
    {
        if (currentLine != null)
        {
            Destroy(currentLine);
        }

        currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        lineRenderer = currentLine.GetComponent<LineRenderer>();
        edgeCollider = currentLine.GetComponent<EdgeCollider2D>();
        lineRb = currentLine.GetComponent<Rigidbody2D>();

        if (lineRb != null)
        {
            lineRb.bodyType = RigidbodyType2D.Kinematic;
            lineRb.linearVelocity = Vector2.zero;
        }

        // --- PERBAIKAN DI SINI ---
        // Memberikan ketebalan fisik bantalan agar tidak tembus dan terdeteksi Ground Check
        if (edgeCollider != null)
        {
            edgeCollider.edgeRadius = 0.05f; // Naikkan ke 0.1f jika deteksi kaki masih kurang peka
        }
        // -------------------------

        fingerPositions.Clear();

        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        fingerPositions.Add(mouseWorldPos);
        fingerPositions.Add(mouseWorldPos);

        lineRenderer.SetPosition(0, fingerPositions[0]);
        lineRenderer.SetPosition(1, fingerPositions[1]);

        if (edgeCollider != null) edgeCollider.points = fingerPositions.ToArray();
    }

    void UpdateLine(Vector2 newFingerPos)
    {
        fingerPositions.Add(newFingerPos);

        lineRenderer.positionCount = fingerPositions.Count;
        lineRenderer.SetPosition(fingerPositions.Count - 1, newFingerPos);

        if (edgeCollider != null) edgeCollider.points = fingerPositions.ToArray();
    }
}