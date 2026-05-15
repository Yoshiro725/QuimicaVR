using UnityEngine;
using TMPro;
using System.Collections;

public class Level1_AtomExplorer : MonoBehaviour
{
    [Header("Átomo a explorar")]
    public Transform atomTarget;           // El átomo que el usuario examina
    public float rotationSpeed = 45f;      // Velocidad de rotación automática
    public float zoomSpeed = 0.5f;         // Velocidad de zoom

    [Header("UI Información")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI infoText;
    public TextMeshProUGUI instructionText;
    public GameObject infoPanel;

    [Header("Componentes del átomo")]
    public GameObject nucleusHighlight;    // Resaltado del núcleo
    public GameObject electronHighlight;   // Resaltado de electrones
    public Light atomLight;               // Luz que ilumina el átomo

    [Header("Pasos de la lección")]
    private int currentStep = 0;
    private string[] stepTitles = {
        "Bienvenido al Átomo",
        "El Núcleo",
        "Los Electrones",
        "Capas Electrónicas",
        "Electrones de Valencia",
        "¡Lección Completada!"
    };

    private string[] stepInfos = {
        "Un átomo es la unidad básica de la materia.\nTodo lo que nos rodea está formado por átomos.\n\nObserva el átomo girando frente a ti.",
        "El NÚCLEO está en el centro del átomo.\nContiene:\n• Protones (carga positiva +)\n• Neutrones (sin carga)\n\nEl número de protones define el elemento.",
        "Los ELECTRONES son partículas de carga\nnegativa que orbitan el núcleo.\nSon mucho más pequeños que el núcleo\ny se mueven muy rápido.",
        "Los electrones se organizan en\nCAPAS o niveles de energía.\n• Capa 1: máximo 2 electrones\n• Capa 2: máximo 8 electrones\n• Capa 3: máximo 18 electrones",
        "Los ELECTRONES DE VALENCIA son\nlos que están en la capa más externa.\nDeterminan cómo el átomo\nse une con otros átomos.",
        "¡Excelente! Ya conoces la estructura\nbásica del átomo.\n\nAhora puedes avanzar al\nNivel 2: Electrones de Valencia"
    };

    private string[] stepInstructions = {
        "Mira alrededor del átomo para explorarlo",
        "Observa el centro brillante del átomo",
        "Observa las órbitas alrededor del núcleo",
        "Cada anillo representa una capa electrónica",
        "Los electrones externos son los de valencia",
        "Presiona SIGUIENTE para continuar al Nivel 2"
    };

    private bool isRotating = true;
    private Vector3 originalAtomPos;

    void Start()
    {
        if (atomTarget != null)
            originalAtomPos = atomTarget.position;

        ShowStep(0);
        StartCoroutine(IntroAnimation());
    }

    void Update()
    {
        if (atomTarget != null && isRotating)
        {
            atomTarget.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }

    IEnumerator IntroAnimation()
    {
        if (atomTarget == null) yield break;

        // El átomo aparece creciendo desde cero
        atomTarget.localScale = Vector3.zero;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 1.2f;
            float s = t * t * (3f - 2f * t);
            atomTarget.localScale = Vector3.one * 0.3f * s;
            yield return null;
        }
        atomTarget.localScale = Vector3.one * 0.3f;
    }

    public void ShowStep(int step)
    {
        currentStep = Mathf.Clamp(step, 0, stepTitles.Length - 1);

        if (titleText       != null) titleText.text       = stepTitles[currentStep];
        if (infoText        != null) infoText.text        = stepInfos[currentStep];
        if (instructionText != null) instructionText.text = stepInstructions[currentStep];

        // Efectos visuales según el paso
        UpdateVisualForStep(currentStep);
    }

    void UpdateVisualForStep(int step)
    {
        // Resetear highlights
        if (nucleusHighlight  != null) nucleusHighlight.SetActive(false);
        if (electronHighlight != null) electronHighlight.SetActive(false);

        switch (step)
        {
            case 1: // Resaltar núcleo
                if (nucleusHighlight != null) nucleusHighlight.SetActive(true);
                if (atomLight != null)
                {
                    atomLight.color     = new Color(1f, 0.8f, 0.2f);
                    atomLight.intensity = 2f;
                }
                rotationSpeed = 20f;
                break;

            case 2: // Resaltar electrones
            case 3:
            case 4:
                if (electronHighlight != null) electronHighlight.SetActive(true);
                if (atomLight != null)
                {
                    atomLight.color     = new Color(0f, 0.8f, 1f);
                    atomLight.intensity = 1.5f;
                }
                rotationSpeed = 60f;
                break;

            case 5: // Completado
                rotationSpeed = 90f;
                if (atomLight != null)
                {
                    atomLight.color     = Color.white;
                    atomLight.intensity = 3f;
                }
                StartCoroutine(CelebrationEffect());
                break;

            default:
                if (atomLight != null)
                {
                    atomLight.color     = new Color(0f, 1f, 1f);
                    atomLight.intensity = 1f;
                }
                rotationSpeed = 45f;
                break;
        }
    }

    IEnumerator CelebrationEffect()
    {
        if (atomTarget == null) yield break;

        float elapsed = 0f;
        Vector3 baseScale = atomTarget.localScale;

        while (elapsed < 2f)
        {
            elapsed += Time.deltaTime;
            float pulse = 1f + Mathf.Sin(elapsed * 8f) * 0.1f;
            atomTarget.localScale = baseScale * pulse;
            yield return null;
        }
        atomTarget.localScale = baseScale;
    }

    // Botones de navegación
    public void NextStep()
    {
        if (currentStep < stepTitles.Length - 1)
            ShowStep(currentStep + 1);
    }

    public void PreviousStep()
    {
        if (currentStep > 0)
            ShowStep(currentStep - 1);
    }

    public void ToggleRotation()
    {
        isRotating = !isRotating;
    }
}