using UnityEngine;

public class AtomController : MonoBehaviour
{
    [Header("Datos del Átomo")]
    public string atomName;          // Ej: "Hidrogeno", "Oxigeno"
    public string symbol;            // Ej: "H", "O", "C"
    public int atomicNumber;         // Número atómico
    public int valenceElectrons;     // Electrones de valencia
    public Color atomColor;          // Color del átomo

    [Header("Visual")]
    public float rotationSpeed = 30f;        // Velocidad de rotación de órbitas
    public float pulseSpeed = 1.5f;          // Velocidad de pulso de brillo
    public float pulseIntensity = 0.3f;      // Intensidad del pulso
    public Renderer atomRenderer;            // Renderer del núcleo

    [Header("Estado")]
    public bool isGrabbed = false;           // ¿El usuario lo está agarrando?
    public bool isHighlighted = false;       // ¿Está resaltado?

    private Material atomMaterial;
    private float baseEmission;
    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;

        // Obtener el material del renderer
        if (atomRenderer != null)
        {
            // Crear instancia del material para no afectar otros átomos
            atomMaterial = atomRenderer.material;
            atomMaterial.color = atomColor;

            // Activar emisión
            atomMaterial.EnableKeyword("_EMISSION");
            atomMaterial.SetColor("_EmissionColor", atomColor * 1.5f);
            baseEmission = 1.5f;
        }
    }

    void Update()
    {
        // Rotar las órbitas constantemente
        if (!isGrabbed)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }

        // Efecto de pulso en el brillo
        PulseEffect();
    }

    void PulseEffect()
    {
        if (atomMaterial == null) return;

        float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseIntensity;
        float currentEmission = baseEmission + pulse;

        atomMaterial.SetColor("_EmissionColor", atomColor * currentEmission);
    }

    // Llamado cuando el usuario agarra el átomo
    public void OnGrab()
    {
        isGrabbed = true;
        // Escalar ligeramente al agarrar para feedback visual
        transform.localScale = originalScale * 1.1f;
    }

    // Llamado cuando el usuario suelta el átomo
    public void OnRelease()
    {
        isGrabbed = false;
        transform.localScale = originalScale;
    }

    // Resaltar átomo (usado en Nivel 2 para electrones de valencia)
    public void Highlight(bool active)
    {
        isHighlighted = active;

        if (atomMaterial == null) return;

        if (active)
        {
            atomMaterial.SetColor("_EmissionColor", Color.white * 3f);
        }
        else
        {
            atomMaterial.SetColor("_EmissionColor", atomColor * baseEmission);
        }
    }

    // Devuelve info del átomo como texto (para UI)
    public string GetAtomInfo()
    {
        return $"{atomName} ({symbol})\n" +
               $"Número atómico: {atomicNumber}\n" +
               $"Electrones de valencia: {valenceElectrons}";
    }
}