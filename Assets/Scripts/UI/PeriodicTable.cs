using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PeriodicTable : MonoBehaviour
{
    [Header("Referencias")]
    public Transform spawnPoint;          // Donde aparece el átomo al seleccionar
    public GameObject atomPrefab;         // Prefab base del átomo

    [Header("Elemento seleccionado")]
    public TextMeshProUGUI elementNameText;
    public TextMeshProUGUI elementSymbolText;
    public TextMeshProUGUI elementInfoText;
    public Image elementColorSwatch;

    // Datos de elementos
    private List<ElementData> elements = new List<ElementData>();
    private ElementData selectedElement;
    private GameObject currentAtom;

    void Start()
    {
        RegisterElements();
        BuildTable();
    }

    void RegisterElements()
    {
        // Nombre, Símbolo, NumAtomico, Electrones Valencia, Color, Descripción
        elements.Add(new ElementData("Hidrogeno",  "H",  1, 1,
            new Color(0f, 1f, 1f),
            "El elemento mas abundante del universo. Forma parte del agua."));

        elements.Add(new ElementData("Helio",      "He", 2, 2,
            new Color(0.9f, 0.9f, 0.2f),
            "Gas noble inerte. Se usa en globos y cohetes."));

        elements.Add(new ElementData("Litio",      "Li", 3, 1,
            new Color(0.8f, 0.4f, 0.9f),
            "Metal ligero. Usado en baterias recargables."));

        elements.Add(new ElementData("Carbono",    "C",  6, 4,
            new Color(0.4f, 0.4f, 0.4f),
            "Base de la vida organica. Forma diamantes y grafito."));

        elements.Add(new ElementData("Nitrogeno",  "N",  7, 5,
            new Color(0.3f, 0.5f, 1f),
            "Compone el 78% del aire. Esencial para proteinas."));

        elements.Add(new ElementData("Oxigeno",    "O",  8, 6,
            new Color(1f, 0.2f, 0.2f),
            "Necesario para la respiracion. Forma parte del agua."));

        elements.Add(new ElementData("Fluor",      "F",  9, 7,
            new Color(0.2f, 0.9f, 0.4f),
            "El elemento mas electronegativo. Muy reactivo."));

        elements.Add(new ElementData("Neon",       "Ne", 10, 8,
            new Color(1f, 0.4f, 0.1f),
            "Gas noble. Usado en letreros luminosos."));

        elements.Add(new ElementData("Sodio",      "Na", 11, 1,
            new Color(1f, 0.8f, 0.1f),
            "Metal muy reactivo con el agua. Componente de la sal."));

        elements.Add(new ElementData("Cloro",      "Cl", 17, 7,
            new Color(0.6f, 1f, 0.2f),
            "Gas amarillo-verde. Se combina con sodio para formar sal."));

        elements.Add(new ElementData("Calcio",     "Ca", 20, 2,
            new Color(0.9f, 0.9f, 0.9f),
            "Esencial para huesos y dientes. Metal alcalinoterreo."));

        elements.Add(new ElementData("Hierro",     "Fe", 26, 2,
            new Color(0.7f, 0.3f, 0.1f),
            "Metal mas usado en la industria. Componente del acero."));
    }

    void BuildTable()
    {
        // Los botones se crean dinámicamente en la UI
        // Busca todos los botones hijos con tag "ElementButton"
        Button[] buttons = GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length && i < elements.Count; i++)
        {
            int index = i;
            ElementData el = elements[i];

            // Color del botón según el elemento
            Image img = buttons[i].GetComponent<Image>();
            if (img != null)
                img.color = new Color(el.color.r * 0.3f, el.color.g * 0.3f, el.color.b * 0.3f, 0.8f);

            // Texto del botón
            TextMeshProUGUI tmp = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.text = el.symbol;
                tmp.color = el.color;
                tmp.fontStyle = FontStyles.Bold;
            }

            // Click → seleccionar elemento
            buttons[i].onClick.AddListener(() => SelectElement(index));
        }
    }

    public void SelectElement(int index)
    {
        if (index < 0 || index >= elements.Count) return;

        selectedElement = elements[index];
        UpdateInfoPanel();
        SpawnAtom();
    }

    void UpdateInfoPanel()
    {
        if (elementNameText   != null) elementNameText.text   = selectedElement.name;
        if (elementSymbolText != null) elementSymbolText.text = selectedElement.symbol;
        if (elementColorSwatch != null) elementColorSwatch.color = selectedElement.color;
        if (elementInfoText   != null)
        {
            elementInfoText.text =
                $"Numero atomico: {selectedElement.atomicNumber}\n" +
                $"Electrones de valencia: {selectedElement.valenceElectrons}\n\n" +
                selectedElement.description;
        }
    }

    void SpawnAtom()
    {
        // Destruir átomo anterior
        if (currentAtom != null)
            Destroy(currentAtom);

        if (atomPrefab == null || spawnPoint == null) return;

        // Instanciar nuevo átomo
        currentAtom = Instantiate(atomPrefab, spawnPoint.position, Quaternion.identity);

        // Aplicar color del elemento
        AtomController ac = currentAtom.GetComponent<AtomController>();
        if (ac != null)
        {
            ac.atomName        = selectedElement.name;
            ac.symbol          = selectedElement.symbol;
            ac.atomicNumber    = selectedElement.atomicNumber;
            ac.valenceElectrons = selectedElement.valenceElectrons;
            ac.atomColor       = selectedElement.color;
        }

        // Aplicar color al material
        Renderer[] renderers = currentAtom.GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            Material mat = r.material;
            mat.color = selectedElement.color;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", selectedElement.color * 1.8f);
        }

        Debug.Log($"Atomo generado: {selectedElement.name} ({selectedElement.symbol})");
    }
}

// Datos de cada elemento
[System.Serializable]
public class ElementData
{
    public string name;
    public string symbol;
    public int atomicNumber;
    public int valenceElectrons;
    public Color color;
    public string description;

    public ElementData(string name, string symbol, int atomicNumber,
        int valenceElectrons, Color color, string description)
    {
        this.name             = name;
        this.symbol           = symbol;
        this.atomicNumber     = atomicNumber;
        this.valenceElectrons = valenceElectrons;
        this.color            = color;
        this.description      = description;
    }
}