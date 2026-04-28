using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class XRAtomGrab : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private AtomController atomController;
    private Rigidbody rb;

    [Header("Configuración de agarre")]
    public float throwForceMultiplier = 1.5f;
    public bool freezeRotationOnGrab = true;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        atomController = GetComponent<AtomController>();
        rb = GetComponent<Rigidbody>();

        // Configurar el XRGrabInteractable
        grabInteractable.movementType = XRBaseInteractable.MovementType.VelocityTracking;
        grabInteractable.throwOnDetach = true;

        // Suscribir eventos de agarre
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    void OnGrabbed(SelectEnterEventArgs args)
    {
        if (atomController != null)
            atomController.OnGrab();

        if (freezeRotationOnGrab && rb != null)
            rb.freezeRotation = true;

        Debug.Log($"Átomo agarrado: {atomController?.symbol}");
    }

    void OnReleased(SelectExitEventArgs args)
    {
        if (atomController != null)
            atomController.OnRelease();

        if (rb != null)
            rb.freezeRotation = false;

        Debug.Log($"Átomo soltado: {atomController?.symbol}");
    }
}