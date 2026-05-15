using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class HolographicMenu : MonoBehaviour
{
    [Header("Paneles del menú")]
    public GameObject mainPanel;          // Panel principal
    public GameObject levelPanel;         // Panel de selección de niveles

    [Header("Botones principales")]
    public Button btnStartExperience;     // Iniciar experiencia
    public Button btnLevels;              // Selección de niveles
    public Button btnSandbox;             // Modo libre
    public Button btnSettings;            // Configuración
    public Button btnBack;                // Volver al menú principal

    [Header("Botones de niveles")]
    public Button btnLevel1;
    public Button btnLevel2;
    public Button btnLevel3;

    [Header("Visual holográfico")]
    public float floatSpeed = 1f;         // Velocidad de flotación
    public float floatAmount = 0.05f;     // Cantidad de movimiento
    public float rotationSpeed = 15f;     // Rotación suave del menú

    [Header("Nombres de escenas")]
    public string level1Scene = "Lab_Level1";
    public string level2Scene = "Lab_Level2";
    public string level3Scene = "Lab_Level3";
    public string sandboxScene = "SandboxMode";

    private Vector3 startPosition;
    private bool isAnimating = false;

    void Start()
    {
        startPosition = transform.position;

        // Configurar botones principales
        if (btnStartExperience != null)
            btnStartExperience.onClick.AddListener(StartExperience);

        if (btnLevels != null)
            btnLevels.onClick.AddListener(ShowLevelPanel);

        if (btnSandbox != null)
            btnSandbox.onClick.AddListener(StartSandbox);

        if (btnBack != null)
            btnBack.onClick.AddListener(ShowMainPanel);

        // Configurar botones de niveles
        if (btnLevel1 != null)
            btnLevel1.onClick.AddListener(() => LoadLevel(level1Scene));

        if (btnLevel2 != null)
            btnLevel2.onClick.AddListener(() => LoadLevel(level2Scene));

        if (btnLevel3 != null)
            btnLevel3.onClick.AddListener(() => LoadLevel(level3Scene));

        // Mostrar panel principal al inicio
        ShowMainPanel();
    }

    void Update()
    {
        // Efecto de flotación suave
        FloatAnimation();

        // Hacer que el menú mire siempre al jugador
        FacePlayer();
    }

    void FloatAnimation()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmount;
        transform.position = new Vector3(
            transform.position.x,
            newY,
            transform.position.z
        );
    }

    void FacePlayer()
    {
        // Buscar la cámara principal
        Camera mainCam = Camera.main;
        if (mainCam == null) return;

        // Rotar suavemente hacia el jugador
        Vector3 direction = mainCam.transform.position - transform.position;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(-direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    public void ShowMainPanel()
    {
        if (mainPanel != null) mainPanel.SetActive(true);
        if (levelPanel != null) levelPanel.SetActive(false);
    }

    public void ShowLevelPanel()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (levelPanel != null) levelPanel.SetActive(true);
    }

    public void StartExperience()
    {
        // Inicia desde el nivel 1
        LoadLevel(level1Scene);
    }

    public void StartSandbox()
    {
        LoadLevel(sandboxScene);
    }

    void LoadLevel(string sceneName)
    {
        Debug.Log($"Cargando escena: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    // Mostrar/ocultar el menú
    public void ToggleMenu()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}