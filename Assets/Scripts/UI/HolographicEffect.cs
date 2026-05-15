using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HolographicEffect : MonoBehaviour
{
    [Header("Scanline")]
    public RectTransform scanline;          // Línea que recorre el panel
    public float scanlineSpeed = 150f;      // Velocidad de la línea
    public float panelHeight = 400f;        // Alto del panel

    [Header("Parpadeo holográfico")]
    public CanvasGroup canvasGroup;         // Para efecto de parpadeo
    public float flickerSpeed = 8f;        // Velocidad de parpadeo
    public float flickerIntensity = 0.05f; // Intensidad (sutil)
    public float glitchInterval = 4f;      // Cada cuánto hace glitch

    [Header("Bordes brillantes")]
    public Image[] borderLines;            // Líneas de borde del panel
    public float borderPulseSpeed = 2f;    // Velocidad de pulso de bordes

    [Header("Partículas")]
    public ParticleSystem menuParticles;   // Partículas flotantes

    [Header("Texto título")]
    public TextMeshProUGUI titleText;      // Texto del título
    public float titleGlowSpeed = 1.5f;   // Velocidad de brillo del título

    private float scanlineY;
    private bool isGlitching = false;

    void Start()
    {
        scanlineY = -panelHeight / 2f;

        // Iniciar efectos
        StartCoroutine(GlitchRoutine());
        StartCoroutine(AnimateEntry());
    }

    void Update()
    {
        AnimateScanline();
        AnimateFlicker();
        AnimateBorders();
        AnimateTitleGlow();
    }

    // Línea que recorre el panel de abajo a arriba
    void AnimateScanline()
    {
        if (scanline == null) return;

        scanlineY += scanlineSpeed * Time.deltaTime;

        if (scanlineY > panelHeight / 2f)
            scanlineY = -panelHeight / 2f;

        scanline.anchoredPosition = new Vector2(0, scanlineY);
    }

    // Parpadeo sutil holográfico
    void AnimateFlicker()
    {
        if (canvasGroup == null || isGlitching) return;

        float flicker = 1f - (Mathf.PerlinNoise(Time.time * flickerSpeed, 0f) * flickerIntensity);
        canvasGroup.alpha = flicker;
    }

    // Pulso en los bordes del panel
    void AnimateBorders()
    {
        if (borderLines == null) return;

        float pulse = (Mathf.Sin(Time.time * borderPulseSpeed) + 1f) / 2f;
        Color borderColor = new Color(0f, 0.8f + pulse * 0.2f, 1f, 0.5f + pulse * 0.5f);

        foreach (var border in borderLines)
        {
            if (border != null)
                border.color = borderColor;
        }
    }

    // Brillo pulsante en el título
    void AnimateTitleGlow()
    {
        if (titleText == null) return;

        float glow = (Mathf.Sin(Time.time * titleGlowSpeed) + 1f) / 2f;
        titleText.color = new Color(
            0f,
            0.8f + glow * 0.2f,
            1f,
            1f
        );
    }

    // Efecto de glitch cada cierto tiempo
    IEnumerator GlitchRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(glitchInterval + Random.Range(-1f, 1f));
            yield return StartCoroutine(DoGlitch());
        }
    }

    IEnumerator DoGlitch()
    {
        isGlitching = true;

        // 3 parpadeos rápidos
        for (int i = 0; i < 3; i++)
        {
            if (canvasGroup != null)
                canvasGroup.alpha = Random.Range(0.6f, 0.95f);
            yield return new WaitForSeconds(0.05f);

            if (canvasGroup != null)
                canvasGroup.alpha = 1f;
            yield return new WaitForSeconds(0.03f);
        }

        isGlitching = false;
    }

    // Animación de entrada — el menú aparece con fade y escala
    IEnumerator AnimateEntry()
    {
        if (canvasGroup == null) yield break;

        canvasGroup.alpha = 0f;
        transform.localScale = Vector3.one * 0.8f;

        float elapsed = 0f;
        float duration = 0.8f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float smoothT = t * t * (3f - 2f * t); // Smooth step

            canvasGroup.alpha = smoothT;
            transform.localScale = Vector3.Lerp(Vector3.one * 0.8f, Vector3.one, smoothT);

            yield return null;
        }

        canvasGroup.alpha = 1f;
        transform.localScale = Vector3.one;
    }
}