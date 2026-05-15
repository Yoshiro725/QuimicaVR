using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class Level3_MoleculeFormation : MonoBehaviour
{
    [Header("Átomos disponibles")]
    public GameObject atomH_Prefab;
    public GameObject atomO_Prefab;
    public GameObject atomC_Prefab;

    [Header("Zona de combinación")]
    public Transform spawnArea;           // Área donde aparecen los átomos
    public Transform combinationZone;     // Centro donde se combinan

    [Header("UI")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI goalText;
    public TextMeshProUGUI infoText;
    public TextMeshProUGUI scoreText;
    public GameObject successPanel;
    public TextMeshProUGUI moleculeResultText;

    [Header("Efectos")]
    public ParticleSystem successParticles;
    public Light sceneLight;
    public AudioSource audioSource;
    public AudioClip successClip;

    // Estado del nivel
    private int score = 0;
    private int currentChallenge = 0;
    private List<GameObject> activeAtoms = new List<GameObject>();

    // Retos del nivel
    private string[] challengeNames = {
        "Forma H2 — Hidrogeno molecular",
        "Forma H2O — Agua",
        "Forma CO2 — Dioxido de Carbono",
        "Forma CH4 — Metano",
        "Modo Libre — Experimenta!"
    };

    private string[] challengeGoals = {
        "Combina: H + H",
        "Combina: H + H + O",
        "Combina: C + O + O",
        "Combina: C + H + H + H + H",
        "Combina los atomos que quieras"
    };

    private string[] challengeInfos = {
        "El hidrogeno molecular H2 es el gas\nmas ligero del universo.\nSe usa como combustible limpio.",
        "El agua H2O es esencial para la vida.\nDos hidrogenos y un oxigeno\nforman este compuesto vital.",
        "El CO2 es producido al respirar\ny al quemar combustibles.\nEs un gas de efecto invernadero.",
        "El metano CH4 es el principal\ncomponente del gas natural.\nUn carbono con 4 hidrogenos.",
        "Modo libre: experimenta con\ndiferentes combinaciones\nde atomos."
    };

    void Start()
    {
        if (successPanel != null)
            successPanel.SetActive(false);

        LoadChallenge(0);
    }

    public void LoadChallenge(int index)
    {
        currentChallenge = Mathf.Clamp(index, 0, challengeNames.Length - 1);

        // Limpiar átomos anteriores
        ClearAtoms();

        // Actualizar UI
        UpdateUI();

        // Generar átomos según el reto
        SpawnAtomsForChallenge(currentChallenge);
    }

    void UpdateUI()
    {
        if (titleText  != null) titleText.text  = challengeNames[currentChallenge];
        if (goalText   != null) goalText.text   = challengeGoals[currentChallenge];
        if (infoText   != null) infoText.text   = challengeInfos[currentChallenge];
        if (scoreText  != null) scoreText.text  = $"Puntos: {score}";
    }

    void SpawnAtomsForChallenge(int challenge)
    {
        if (spawnArea == null) return;

        Vector3 basePos = spawnArea.position;
        float spacing   = 0.4f;

        switch (challenge)
        {
            case 0: // H2
                SpawnAtom(atomH_Prefab, basePos + Vector3.left  * spacing);
                SpawnAtom(atomH_Prefab, basePos + Vector3.right * spacing);
                break;

            case 1: // H2O
                SpawnAtom(atomH_Prefab, basePos + Vector3.left  * spacing * 1.5f);
                SpawnAtom(atomH_Prefab, basePos + Vector3.right * spacing * 1.5f);
                SpawnAtom(atomO_Prefab, basePos + Vector3.up    * spacing);
                break;

            case 2: // CO2
                SpawnAtom(atomC_Prefab, basePos);
                SpawnAtom(atomO_Prefab, basePos + Vector3.left  * spacing * 1.5f);
                SpawnAtom(atomO_Prefab, basePos + Vector3.right * spacing * 1.5f);
                break;

            case 3: // CH4
                SpawnAtom(atomC_Prefab, basePos);
                SpawnAtom(atomH_Prefab, basePos + Vector3.forward * spacing);
                SpawnAtom(atomH_Prefab, basePos - Vector3.forward * spacing);
                SpawnAtom(atomH_Prefab, basePos + Vector3.left    * spacing);
                SpawnAtom(atomH_Prefab, basePos + Vector3.right   * spacing);
                break;

            case 4: // Modo libre — todos los átomos
                SpawnAtom(atomH_Prefab, basePos + new Vector3(-0.8f, 0, 0));
                SpawnAtom(atomH_Prefab, basePos + new Vector3(-0.4f, 0, 0));
                SpawnAtom(atomO_Prefab, basePos + new Vector3( 0f,   0, 0));
                SpawnAtom(atomC_Prefab, basePos + new Vector3( 0.4f, 0, 0));
                SpawnAtom(atomH_Prefab, basePos + new Vector3( 0.8f, 0, 0));
                break;
        }
    }

    void SpawnAtom(GameObject prefab, Vector3 position)
    {
        if (prefab == null) return;
        GameObject atom = Instantiate(prefab, position, Quaternion.identity);
        atom.transform.localScale = Vector3.one * 0.3f;
        activeAtoms.Add(atom);
    }

    void ClearAtoms()
    {
        foreach (var atom in activeAtoms)
            if (atom != null) Destroy(atom);
        activeAtoms.Clear();
    }

    // Llamado desde MoleculeManager cuando se forma una molécula
    public void OnMoleculeFormed(string moleculeName, string formula, Vector3 position)
    {
        score += 100;
        StartCoroutine(CelebrationRoutine(moleculeName, formula, position));
    }

    IEnumerator CelebrationRoutine(string name, string formula, Vector3 pos)
    {
        // Mostrar panel de éxito
        if (successPanel != null)
        {
            successPanel.SetActive(true);
            if (moleculeResultText != null)
                moleculeResultText.text = $"¡{name} formado!\n{formula}";
        }

        // Partículas
        if (successParticles != null)
        {
            successParticles.transform.position = pos;
            successParticles.Play();
        }

        // Flash de luz
        if (sceneLight != null)
            StartCoroutine(FlashLight());

        // Sonido
        if (audioSource != null && successClip != null)
            audioSource.PlayOneShot(successClip);

        // Actualizar score
        if (scoreText != null)
            scoreText.text = $"Puntos: {score}";

        yield return new WaitForSeconds(3f);

        // Ocultar panel y cargar siguiente reto
        if (successPanel != null)
            successPanel.SetActive(false);

        if (currentChallenge < challengeNames.Length - 1)
            LoadChallenge(currentChallenge + 1);
    }

    IEnumerator FlashLight()
    {
        if (sceneLight == null) yield break;

        Color original  = sceneLight.color;
        float origIntens = sceneLight.intensity;

        sceneLight.color     = new Color(0f, 1f, 0.8f);
        sceneLight.intensity = 4f;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 1f;
            sceneLight.intensity = Mathf.Lerp(4f, origIntens, t);
            yield return null;
        }

        sceneLight.color     = original;
        sceneLight.intensity = origIntens;
    }

    // Botones UI
    public void NextChallenge()
    {
        if (currentChallenge < challengeNames.Length - 1)
            LoadChallenge(currentChallenge + 1);
    }

    public void PreviousChallenge()
    {
        if (currentChallenge > 0)
            LoadChallenge(currentChallenge - 1);
    }

    public void RestartChallenge()
    {
        LoadChallenge(currentChallenge);
    }
}