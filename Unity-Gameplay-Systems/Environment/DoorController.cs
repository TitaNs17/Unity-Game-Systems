using UnityEngine;

public class DoorController : MonoBehaviour, IInteractable
{
    [Header("Hinges")]
    [SerializeField] private Transform leftHinge;
    [SerializeField] private Transform rightHinge;

    [Header("Door")]
    [SerializeField] private float openAngle = 90f;
    [SerializeField, Min(0.1f)] private float openSpeed = 4f;
    [SerializeField] private float direction = -1f;

    [Header("Automatic Close")]
    [SerializeField] private bool autoClose = true;
    [SerializeField, Min(0f)] private float closeDelay = 3f;
    [SerializeField, Min(0.1f)] private float npcDetectionRadius = 2.5f;
    [SerializeField, Min(0.05f)] private float npcScanInterval = 0.25f;

    private float closeTimer;
    private float nextNpcScan;
    private bool isOpen;
    private bool npcNearby;
    private Quaternion leftClosed;
    private Quaternion leftOpen;
    private Quaternion rightClosed;
    private Quaternion rightOpen;

    private void Start()
    {
        if (leftHinge != null)
        {
            leftClosed = leftHinge.localRotation;
            leftOpen = leftClosed * Quaternion.Euler(0f, openAngle * direction, 0f);
        }

        if (rightHinge != null)
        {
            rightClosed = rightHinge.localRotation;
            rightOpen = rightClosed * Quaternion.Euler(0f, -openAngle * direction, 0f);
        }
    }

    private void Update()
    {
        if (Time.time >= nextNpcScan)
        {
            nextNpcScan = Time.time + npcScanInterval;
            npcNearby = HasNearbyNpc();
            if (npcNearby && !isOpen)
                OpenDoor();
        }

        if (autoClose && isOpen && !npcNearby)
        {
            closeTimer += Time.deltaTime;
            if (closeTimer >= closeDelay)
                CloseDoor();
        }
        else if (npcNearby)
        {
            closeTimer = 0f;
        }

        UpdateHinges();
    }

    public void OpenDoor()
    {
        isOpen = true;
        closeTimer = 0f;
    }

    public void CloseDoor()
    {
        isOpen = false;
        closeTimer = 0f;
    }

    public void Interact()
    {
        if (isOpen)
            CloseDoor();
        else
            OpenDoor();
    }

    public string GetInteractionText() => isOpen ? "Kapat" : "Aç";

    private bool HasNearbyNpc()
    {
        GameObject[] npcs;
        try
        {
            npcs = GameObject.FindGameObjectsWithTag("NPC");
        }
        catch (UnityException)
        {
            return false;
        }

        var maxDistanceSqr = npcDetectionRadius * npcDetectionRadius;
        foreach (var npc in npcs)
        {
            if (npc != null && (npc.transform.position - transform.position).sqrMagnitude <= maxDistanceSqr)
                return true;
        }

        return false;
    }

    private void UpdateHinges()
    {
        var t = Time.deltaTime * openSpeed;

        if (leftHinge != null)
        {
            var target = isOpen ? leftOpen : leftClosed;
            leftHinge.localRotation = Quaternion.Slerp(leftHinge.localRotation, target, t);
        }

        if (rightHinge != null)
        {
            var target = isOpen ? rightOpen : rightClosed;
            rightHinge.localRotation = Quaternion.Slerp(rightHinge.localRotation, target, t);
        }
    }
}
