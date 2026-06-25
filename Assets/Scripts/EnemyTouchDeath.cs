using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyTouchDeath : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Jika mendeteksi tabrakan fisik dengan Player (artinya player TIDAK sedang sembunyi)
        if (collision.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}