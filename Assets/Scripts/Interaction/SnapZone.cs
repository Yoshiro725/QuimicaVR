using UnityEngine;
using System.Collections.Generic;

public class SnapZone : MonoBehaviour
{
    [Header("Configuración")]
    public float snapRadius = 0.6f;           // Radio de detección
    public Color zoneColorNormal = new Color(0f, 1f, 1f, 0.15f);
    public Color zoneColorActive = new Color(0f, 1f, 0.5f, 0.35f);

    [Header("Visual")]
    public Renderer zoneRenderer;              // Renderer de la zona visual

    private List<AtomController> atomsInZone = new List<AtomController>();
    private Material zoneMaterial;

    void Start()
    {
        // Configurar el trigger
        SphereCollider col = gameObject.GetComponent<SphereCollider>();
        if (col == null) col = gameObject.AddComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = snapRadius;

        // Configurar visual de la zona
        if (zoneRenderer != null)
        {
            zoneMaterial = zoneRenderer.material;
            zoneMaterial.color = zoneColorNormal;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        AtomController atom = other.GetComponentInParent<AtomController>();
        if (atom != null && !atomsInZone.Contains(atom))
        {
            atomsInZone.Add(atom);
            atom.Highlight(true);

            // Notificar al MoleculeManager
            if (MoleculeManager.Instance != null)
                MoleculeManager.Instance.AtomEntered(atom);

            UpdateZoneVisual();
            Debug.Log($"Átomo entró a la zona: {atom.symbol} — Total: {atomsInZone.Count}");
        }
    }

    void OnTriggerExit(Collider other)
    {
        AtomController atom = other.GetComponentInParent<AtomController>();
        if (atom != null && atomsInZone.Contains(atom))
        {
            atomsInZone.Remove(atom);
            atom.Highlight(false);

            // Notificar al MoleculeManager
            if (MoleculeManager.Instance != null)
                MoleculeManager.Instance.AtomExited(atom);

            UpdateZoneVisual();
            Debug.Log($"Átomo salió de la zona: {atom.symbol}");
        }
    }

    void UpdateZoneVisual()
    {
        if (zoneMaterial == null) return;

        // Cambiar color según si hay átomos en la zona
        zoneMaterial.color = atomsInZone.Count > 0 ? zoneColorActive : zoneColorNormal;

        // Activar emisión cuando hay átomos
        if (atomsInZone.Count > 0)
        {
            zoneMaterial.EnableKeyword("_EMISSION");
            zoneMaterial.SetColor("_EmissionColor", Color.green * 0.5f);
        }
        else
        {
            zoneMaterial.DisableKeyword("_EMISSION");
        }
    }

    // Mostrar la zona en el editor
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
        Gizmos.DrawSphere(transform.position, snapRadius);
        Gizmos.color = new Color(0f, 1f, 1f, 0.8f);
        Gizmos.DrawWireSphere(transform.position, snapRadius);
    }
}