using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // Pastikan kamu menggunakan TextMeshPro untuk Teks
using UnityEngine.Rendering.Universal; // Untuk URP 2D Light

public class LoreTransition : MonoBehaviour
{
    [Header("Lore UI Settings")]
    public TextMeshProUGUI loreTextUI;
    [TextArea(3, 5)] public string halaman1Text;
    [TextArea(3, 5)] public string halaman2Text;
    public float textFadeDuration = 2f;
    public float textReadTime = 4f;

    [Header("Environment Morphing Settings")]
    [Tooltip("Background seram W1 yang akan memudar menjadi transparan")]
    public SpriteRenderer scaryBackground;
    [Tooltip("Background selimut W2 yang sudah diposisikan di belakang scaryBackground")]
    public SpriteRenderer warmBackground;
    
    [Header("Lighting Settings (URP 2D)")]
    public Light2D globalLight;
    public Color warmLightColor = new Color(1f, 0.8f, 0.6f); // Warna jingga/hangat
    public float lightTransitionDuration = 3f;

    [Header("Scene Transition Settings")]
    public string nextSceneName = "W2";
    public Image fadeBlackScreen;
    
    private bool isTransitioning = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Pastikan hanya player yang memicu transisi dan hanya terjadi sekali
        if (collision.CompareTag("Player") && !isTransitioning)
        {
            StartCoroutine(TransitionSequence());
        }
    }

    private IEnumerator TransitionSequence()
    {
        isTransitioning = true;

        // 1. Mulai mengubah pencahayaan dan environment secara perlahan (Berjalan bersamaan)
        StartCoroutine(MorphEnvironment());
        StartCoroutine(MorphLighting());

        // 2. Tampilkan Halaman 1: The Explorer
        yield return StartCoroutine(FadeText(halaman1Text, 1f)); // Fade in
        yield return new WaitForSeconds(textReadTime);           // Waktu baca
        yield return StartCoroutine(FadeText(halaman1Text, 0f)); // Fade out

        // 3. Jeda sejenak
        yield return new WaitForSeconds(1f);

        // 4. Tampilkan Halaman 2: The Hero
        yield return StartCoroutine(FadeText(halaman2Text, 1f)); // Fade in
        yield return new WaitForSeconds(textReadTime);           // Waktu baca
        yield return StartCoroutine(FadeText(halaman2Text, 0f)); // Fade out

        // 5. Fade Layar ke Hitam
        yield return StartCoroutine(FadeToBlack());

        // 6. Pindah ke Scene W2
        SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator MorphEnvironment()
    {
        float elapsedTime = 0f;
        Color scaryColor = scaryBackground.color;

        // Pastikan warmBackground terlihat penuh di belakang
        Color warmColor = warmBackground.color;
        warmColor.a = 1f;
        warmBackground.color = warmColor;

        while (elapsedTime < lightTransitionDuration)
        {
            elapsedTime += Time.deltaTime;
            // Kurangi alpha background seram menuju 0
            scaryColor.a = Mathf.Lerp(1f, 0f, elapsedTime / lightTransitionDuration);
            scaryBackground.color = scaryColor;
            yield return null;
        }
    }

    private IEnumerator MorphLighting()
    {
        float elapsedTime = 0f;
        Color initialLightColor = globalLight.color;
        float initialIntensity = globalLight.intensity;

        while (elapsedTime < lightTransitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / lightTransitionDuration;
            
            // Ubah warna dan intensitas cahaya
            globalLight.color = Color.Lerp(initialLightColor, warmLightColor, t);
            globalLight.intensity = Mathf.Lerp(initialIntensity, 1.2f, t); 
            yield return null;
        }
    }

    private IEnumerator FadeText(string textToDisplay, float targetAlpha)
    {
        loreTextUI.text = textToDisplay;
        float elapsedTime = 0f;
        Color textColor = loreTextUI.color;
        float startAlpha = textColor.a;

        while (elapsedTime < textFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            textColor.a = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / textFadeDuration);
            loreTextUI.color = textColor;
            yield return null;
        }
    }

    private IEnumerator FadeToBlack()
    {
        float elapsedTime = 0f;
        Color fadeColor = fadeBlackScreen.color;

        while (elapsedTime < 2f)
        {
            elapsedTime += Time.deltaTime;
            fadeColor.a = Mathf.Lerp(0f, 1f, elapsedTime / 2f);
            fadeBlackScreen.color = fadeColor;
            yield return null;
        }
    }
}