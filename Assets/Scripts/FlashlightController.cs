using UnityEngine;
// WAJIB: Tambahkan namespace Input System di paling atas
using UnityEngine.InputSystem;

public class FlashlightController : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Cek apakah perangkat Mouse terdeteksi dan aktif
        if (Mouse.current != null)
        {
            // CARA BARU: Ambil posisi mouse menggunakan Input System Package
            Vector2 mousePixelPosition = Mouse.current.position.ReadValue();

            // Ubah koordinat pixel mouse di layar menjadi koordinat dunia game 2D
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePixelPosition.x, mousePixelPosition.y, mainCamera.nearClipPlane));
            mousePosition.z = 0f; // Kunci sumbu Z agar senter tidak maju-mundur

            // Update posisi objek senter agar nempel di kursor mouse
            transform.position = mousePosition;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Tetap sama: Cek jika objek yang kena senter adalah musuh world baru
        if (other.CompareTag("Enemy"))
        {
            EnemyFlee fleeMonster = other.GetComponent<EnemyFlee>();
            if (fleeMonster != null)
            {
                fleeMonster.GetScaredByLight();
            }
        }
    }
}