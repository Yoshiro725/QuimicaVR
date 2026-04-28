using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoleculeManager : MonoBehaviour
{
    public static MoleculeManager Instance;

    [Header("Configuración de detección")]
    public float detectionRadius = 0.5f;      // Radio para detectar átomos cercanos
    public float combinationDelay = 0.5f;     // Tiempo antes de combinar

    [Header("Efectos")]
    public GameObject successParticles;        // Partículas al formar molécula
    public GameObject failParticles;           // Partículas al fallar
    public AudioSource audioSource;
    public AudioClip successSound;
    public AudioClip failSound;

    [Header("Moléculas conocidas")]
    // Cada receta define qué símbolos se necesitan y qué se forma
    private List<MoleculeRecipe> recipes = new List<MoleculeRecipe>();

    // Átomos actualmente en la zona de combinación
    private List<AtomController> atomsInZone = new List<AtomController>();

    void Awake()
    {
        // Singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Registrar recetas de moléculas
        RegisterRecipes();
    }

    void RegisterRecipes()
    {
        // Agua: H + H + O = H2O
        recipes.Add(new MoleculeRecipe(
            name: "Agua",
            formula: "H₂O",
            atoms: new Dictionary<string, int> { { "H", 2 }, { "O", 1 } },
            description: "El agua es esencial para la vida."
        ));

        // Dióxido de carbono: C + O + O = CO2
        recipes.Add(new MoleculeRecipe(
            name: "Dióxido de Carbono",
            formula: "CO₂",
            atoms: new Dictionary<string, int> { { "C", 1 }, { "O", 2 } },
            description: "Gas presente en la atmósfera terrestre."
        ));

        // Metano: C + H + H + H + H = CH4
        recipes.Add(new MoleculeRecipe(
            name: "Metano",
            formula: "CH₄",
            atoms: new Dictionary<string, int> { { "C", 1 }, { "H", 4 } },
            description: "Gas natural usado como combustible."
        ));

        // Hidrógeno molecular: H + H = H2
        recipes.Add(new MoleculeRecipe(
            name: "Hidrógeno molecular",
            formula: "H₂",
            atoms: new Dictionary<string, int> { { "H", 2 } },
            description: "El elemento más abundante del universo."
        ));
    }

    // Llamado cuando un átomo entra a la zona de combinación
    public void AtomEntered(AtomController atom)
    {
        if (!atomsInZone.Contains(atom))
        {
            atomsInZone.Add(atom);
            Debug.Log($"Átomo en zona: {atom.symbol} — Total: {atomsInZone.Count}");
            CheckCombination();
        }
    }

    // Llamado cuando un átomo sale de la zona
    public void AtomExited(AtomController atom)
    {
        atomsInZone.Remove(atom);
    }

    void CheckCombination()
    {
        // Contar cuántos de cada símbolo hay en la zona
        Dictionary<string, int> currentAtoms = new Dictionary<string, int>();
        foreach (var atom in atomsInZone)
        {
            if (!currentAtoms.ContainsKey(atom.symbol))
                currentAtoms[atom.symbol] = 0;
            currentAtoms[atom.symbol]++;
        }

        // Comparar con las recetas conocidas
        foreach (var recipe in recipes)
        {
            if (RecipeMatches(recipe, currentAtoms))
            {
                StartCoroutine(FormMolecule(recipe));
                return;
            }
        }
    }

    bool RecipeMatches(MoleculeRecipe recipe, Dictionary<string, int> current)
    {
        // Verificar que coincida exactamente con la receta
        if (recipe.requiredAtoms.Count != current.Count) return false;

        foreach (var kvp in recipe.requiredAtoms)
        {
            if (!current.ContainsKey(kvp.Key)) return false;
            if (current[kvp.Key] != kvp.Value) return false;
        }
        return true;
    }

    IEnumerator FormMolecule(MoleculeRecipe recipe)
    {
        Debug.Log($"¡Molécula formada: {recipe.formula}!");

        // Obtener posición central entre los átomos
        Vector3 centerPos = GetAtomsCenter();

        yield return new WaitForSeconds(combinationDelay);

        // Efecto de éxito
        PlaySuccessEffect(centerPos);

        // Mostrar información de la molécula
        ShowMoleculeInfo(recipe);

        // Desactivar los átomos usados
        foreach (var atom in atomsInZone)
        {
            atom.gameObject.SetActive(false);
        }
        atomsInZone.Clear();
    }

    Vector3 GetAtomsCenter()
    {
        if (atomsInZone.Count == 0) return Vector3.zero;

        Vector3 sum = Vector3.zero;
        foreach (var atom in atomsInZone)
            sum += atom.transform.position;
        return sum / atomsInZone.Count;
    }

    void PlaySuccessEffect(Vector3 position)
    {
        // Partículas de éxito
        if (successParticles != null)
            Instantiate(successParticles, position, Quaternion.identity);

        // Sonido de éxito
        if (audioSource != null && successSound != null)
            audioSource.PlayOneShot(successSound);
    }

    void PlayFailEffect(Vector3 position)
    {
        if (failParticles != null)
            Instantiate(failParticles, position, Quaternion.identity);

        if (audioSource != null && failSound != null)
            audioSource.PlayOneShot(failSound);
    }

    void ShowMoleculeInfo(MoleculeRecipe recipe)
    {
        // Por ahora lo mostramos en consola
        // Luego lo conectaremos al HUD
        Debug.Log($"✅ {recipe.name} ({recipe.formula})\n{recipe.description}");
    }
}

// Clase que define una receta de molécula
[System.Serializable]
public class MoleculeRecipe
{
    public string name;
    public string formula;
    public string description;
    public Dictionary<string, int> requiredAtoms;

    public MoleculeRecipe(string name, string formula,
        Dictionary<string, int> atoms, string description)
    {
        this.name = name;
        this.formula = formula;
        this.requiredAtoms = atoms;
        this.description = description;
    }
}