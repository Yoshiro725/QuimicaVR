using UnityEngine;
using System.Collections;

public class FeedbackSystem : MonoBehaviour
{
    public static FeedbackSystem Instance;

    [Header("Partículas éxito")]
    public GameObject successParticlePrefab;
    public GameObject failParticlePrefab;

    [Header("Luz de feedback")]
    public Light feedbackLight;
    public Color successColor = new Color(0f, 1f, 0.8f);
    public Color failColor    = new Color(1f, 0.3f, 0.1f);
    public float lightDuration = 1.2f;
    public float lightIntensity = 3f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip successClip;
    public AudioClip failClip;
    public AudioClip bondClip;

    [Header("HUD")]
    public UnityEngine.UI.Image hudPanel;
    public TMPro.TextMeshProUGUI moleculeNameText;
    public TMPro.TextMeshProUGUI moleculeFormulaText;
    public TMPro.TextMeshProUGUI moleculeDescText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (hudPanel != null)
            hudPanel.gameObject.SetActive(false);
    }

    // ── Llamado desde MoleculeManager al formar molécula ──
    public void PlaySuccess(Vector3 position, string name, string formula, string desc)
    {
        StartCoroutine(SuccessRoutine(position, name, formula, desc));
    }

    public void PlayFail(Vector3 position)
    {
        StartCoroutine(FailRoutine(position));
    }

    // ── Éxito ──
    IEnumerator SuccessRoutine(Vector3 pos, string name, string formula, string desc)
    {
        // 1. Sonido
        if (audioSource != null && successClip != null)
            audioSource.PlayOneShot(successClip);

        // 2. Partículas
        if (successParticlePrefab != null)
        {
            GameObject p = Instantiate(successParticlePrefab, pos, Quaternion.identity);
            Destroy(p, 3f);
        }

        // 3. Flash de luz
        if (feedbackLight != null)
            StartCoroutine(FlashLight(successColor));

        // 4. HUD con info de la molécula
        ShowHUD(name, formula, desc);

        yield return new WaitForSeconds(4f);

        HideHUD();
    }

    // ── Fallo ──
    IEnumerator FailRoutine(Vector3 pos)
    {
        if (audioSource != null && failClip != null)
            audioSource.PlayOneShot(failClip);

        if (failParticlePrefab != null)
        {
            GameObject p = Instantiate(failParticlePrefab, pos, Quaternion.identity);
            Destroy(p, 2f);
        }

        if (feedbackLight != null)
            StartCoroutine(FlashLight(failColor));

        yield return null;
    }

    // ── Flash de luz ──
    IEnumerator FlashLight(Color color)
    {
        feedbackLight.color     = color;
        feedbackLight.intensity = lightIntensity;
        feedbackLight.enabled   = true;

        float elapsed = 0f;
        while (elapsed < lightDuration)
        {
            elapsed += Time.deltaTime;
            feedbackLight.intensity = Mathf.Lerp(lightIntensity, 0f, elapsed / lightDuration);
            yield return null;
        }

        feedbackLight.enabled = false;
    }

    // ── HUD ──
    void ShowHUD(string name, string formula, string desc)
    {
        if (hudPanel == null) return;
        hudPanel.gameObject.SetActive(true);

        if (moleculeNameText    != null) moleculeNameText.text    = name;
        if (moleculeFormulaText != null) moleculeFormulaText.text = formula;
        if (moleculeDescText    != null) moleculeDescText.text    = desc;

        StartCoroutine(AnimateHUD());
    }

    void HideHUD()
    {
        if (hudPanel != null)
            hudPanel.gameObject.SetActive(false);
    }

    IEnumerator AnimateHUD()
    {
        if (hudPanel == null) yield break;

        CanvasGroup cg = hudPanel.GetComponent<CanvasGroup>();
        if (cg == null) cg = hudPanel.gameObject.AddComponent<CanvasGroup>();

        cg.alpha = 0f;
        float t = 0f;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Clamp01(t / 0.5f);
            yield return null;
        }
        cg.alpha = 1f;
    }
}