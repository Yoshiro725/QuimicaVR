using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class Level2_ValenceElectrons : MonoBehaviour
{
    [Header("Átomo")]
    public Transform atomTarget;
    public Renderer[] electronRenderers;   // Renderers de los electrones
    public Renderer nucleusRenderer;       // Renderer del núcleo

    [Header("Colores")]
    public Color normalElectronColor  = new Color(0.3f, 0.3f, 1f);
    public Color valenceElectronColor = new Color(1f, 1f, 0f);    // Amarillo = valencia
    public Color inactiveColor        = new Color(0.15f, 0.15f, 0.15f);
    public Color nucleusColor         = new Color(1f, 0.4f, 0f);

    [Header("UI")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI infoText;
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI valenceCountText;  // Muestra cuántos electrones de valencia
    public GameObject progressBar;

    [Header("Elementos para practicar")]
    public int currentElement = 0;

    // Datos simplificados de elementos para el nivel
    private string[]  elementNames    = { "Hidrogeno", "Carbono", "Oxigeno",  "Sodio",  "Cloro"  };
    private string[]  elementSymbols  = { "H",         "C",       "O",        "Na",     "Cl"     };
    private int[]     valenceCount    = { 1,            4,         6,          1,        7        };
    private int[]     totalElectrons  = { 1,            6,         8,          11,       17       };
    private Color[]   elementColors   = {
        new Color(0f,   1f,   1f),
        new Color(0.4f, 0.4f, 0.4f),
        new Color(1f,   0.2f, 0.2f),
        new Color(1f,   0.8f, 0.1f),
        new Color(0.4f, 1f,   0.2f)
    };

    private int currentStep = 0;
    private bool showingValence = false;

    void Start()
    {
        LoadElement(0);
    }

    public void LoadElement(int index)
    {
        currentElement = Mathf.Clamp(index, 0, elementNames.Length - 1);
        currentStep    = 0;
        showingValence = false;

        UpdateUI();
        ResetElectronColors();
        StartCoroutine(ElementTransition());
    }

    IEnumerator ElementTransition()
    {
        // Pequeña animación al cambiar de elemento
        if (atomTarget == null) yield break;

        float t = 0f;
        Vector3 baseScale = Vector3.one * 0.3f;

        while (t < 0.3f)
        {
            t += Time.deltaTime;
            atomTarget.localScale = baseScale * (1f - t / 0.3f);
            yield return null;
        }

        // Cambiar color del átomo
        if (nucleusRenderer != null)
        {
            nucleusRenderer.material.color = elementColors[currentElement];
            nucleusRenderer.material.SetColor("_EmissionColor",
                elementColors[currentElement] * 1.5f);
        }

        t = 0f;
        while (t < 0.4f)
        {
            t += Time.deltaTime;
            atomTarget.localScale = baseScale * (t / 0.4f);
            yield return null;
        }
        atomTarget.localScale = baseScale;
    }

    void UpdateUI()
    {
        string name    = elementNames[currentElement];
        string symbol  = elementSymbols[currentElement];
        int valence    = valenceCount[currentElement];
        int total      = totalElectrons[currentElement];

        if (titleText != null)
            titleText.text = $"{name} ({symbol})";

        if (valenceCountText != null)
            valenceCountText.text = $"Electrones de valencia: {valence}";

        if (infoText != null)
        {
            if (!showingValence)
            {
                infoText.text =
                    $"El {name} tiene {total} electrones en total.\n\n" +
                    $"Estos electrones se distribuyen\n" +
                    $"en diferentes capas alrededor del núcleo.\n\n" +
                    $"Presiona RESALTAR para ver\ncuáles son los electrones de valencia.";
            }
            else
            {
                infoText.text =
                    $"Los electrones AMARILLOS son\nlos electrones de VALENCIA del {name}.\n\n" +
                    $"Tiene {valence} electrones de valencia.\n\n" +
                    (valence <= 4
                        ? $"Con {valence} electrones puede formar\nhasta {valence} enlaces químicos."
                        : $"Con {valence} electrones le faltan {8 - valence}\npara completar el octeto.");
            }
        }

        if (instructionText != null)
        {
            instructionText.text = showingValence
                ? "Los electrones amarillos son los de valencia"
                : "Presiona RESALTAR para ver los electrones de valencia";
        }
    }

    void ResetElectronColors()
    {
        if (electronRenderers == null) return;
        foreach (var r in electronRenderers)
        {
            if (r == null) continue;
            r.material.color = normalElectronColor;
            r.material.EnableKeyword("_EMISSION");
            r.material.SetColor("_EmissionColor", normalElectronColor * 1.2f);
        }
    }

    // Botón RESALTAR — muestra los electrones de valencia en amarillo
    public void HighlightValenceElectrons()
    {
        showingValence = !showingValence;

        if (showingValence)
            StartCoroutine(HighlightAnimation());
        else
            ResetElectronColors();

        UpdateUI();
    }

    IEnumerator HighlightAnimation()
    {
        if (electronRenderers == null) yield break;

        int valence = valenceCount[currentElement];
        int total   = electronRenderers.Length;

        // Primero atenuar todos
        foreach (var r in electronRenderers)
        {
            if (r == null) continue;
            r.material.color = inactiveColor;
            r.material.SetColor("_EmissionColor", inactiveColor);
        }

        yield return new WaitForSeconds(0.3f);

        // Resaltar solo los de valencia (los últimos N)
        int startIndex = Mathf.Max(0, total - valence);
        for (int i = startIndex; i < total && i < electronRenderers.Length; i++)
        {
            if (electronRenderers[i] == null) continue;

            electronRenderers[i].material.color = valenceElectronColor;
            electronRenderers[i].material.SetColor("_EmissionColor",
                valenceElectronColor * 2f);

            yield return new WaitForSeconds(0.15f); // Aparecen uno a uno
        }
    }

    // Navegación entre elementos
    public void NextElement()
    {
        if (currentElement < elementNames.Length - 1)
            LoadElement(currentElement + 1);
    }

    public void PreviousElement()
    {
        if (currentElement > 0)
            LoadElement(currentElement - 1);
    }

    public void NextStep()  => NextElement();
    public void PrevStep()  => PreviousElement();
}