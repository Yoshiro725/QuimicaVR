using UnityEngine;

public class FuturisticDoor : MonoBehaviour
{
    [Header("Partes que se mueven")]
    public Transform frame;   // arrastra Frame aquí
    public Transform top;     // arrastra Top aquí

    [Header("Configuración")]
    public float openHeight = 2.5f;
    public float speed = 2f;
    public float triggerDistance = 3f;
    public Transform player;

    private Vector3 frameClosed, frameOpen;
    private Vector3 topClosed, topOpen;

    void Start()
    {
        frameClosed = frame.position;
        frameOpen = frameClosed + Vector3.up * openHeight;

        topClosed = top.position;
        topOpen = topClosed + Vector3.up * openHeight;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        bool isOpen = distance < triggerDistance;

        frame.position = Vector3.Lerp(frame.position, isOpen ? frameOpen : frameClosed, Time.deltaTime * speed);
        top.position = Vector3.Lerp(top.position, isOpen ? topOpen : topClosed, Time.deltaTime * speed);
    }
}