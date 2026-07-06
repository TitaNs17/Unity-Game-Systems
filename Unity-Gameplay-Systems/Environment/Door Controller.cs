using UnityEngine;

public class DoorController : MonoBehaviour, IInteractable
{
    [Header("Menteşe Ayarları")]
    public Transform leftHinge;
    public Transform rightHinge;

    [Header("Kapı Ayarları")]
    public float openAngle = 90f;
    public float openSpeed = 4f;
    public float yonSecimi = -1f;

    [Header("Otomatik Kapanma")]
    public bool otomatikKapansin = true;
    public float kapanmaSuresi = 3.0f; 
    private float kapanmaZamanlayici;

    private bool isOpen = false;
    private Quaternion leftClosed, leftOpen;
    private Quaternion rightClosed, rightOpen;

    void Start()
    {
        if (leftHinge) leftClosed = leftHinge.localRotation;
        if (rightHinge) rightClosed = rightHinge.localRotation;

        if (leftHinge) leftOpen = leftClosed * Quaternion.Euler(0, openAngle * yonSecimi, 0);
        if (rightHinge) rightOpen = rightClosed * Quaternion.Euler(0, -openAngle * yonSecimi, 0);
    }

    void Update()
    {
        
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");
        bool yakinlardaNPCVar = false;

        foreach (GameObject npc in npcs)
        {
            if (Vector3.Distance(npc.transform.position, transform.position) < 2.5f)
            {
                yakinlardaNPCVar = true;
                if (!isOpen) OpenDoor();
            }
        }

        if (otomatikKapansin && isOpen && !yakinlardaNPCVar)
        {
            kapanmaZamanlayici += Time.deltaTime;
            if (kapanmaZamanlayici >= kapanmaSuresi)
            {
                CloseDoor();
            }
        }

       
        if (leftHinge)
        {
            Quaternion target = isOpen ? leftOpen : leftClosed;
            leftHinge.localRotation = Quaternion.Slerp(leftHinge.localRotation, target, Time.deltaTime * openSpeed);
        }

        if (rightHinge)
        {
            Quaternion target = isOpen ? rightOpen : rightClosed;
            rightHinge.localRotation = Quaternion.Slerp(rightHinge.localRotation, target, Time.deltaTime * openSpeed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC")) OpenDoor();
    }

    public void OpenDoor()
    {
        isOpen = true;
        kapanmaZamanlayici = 0; 
    }

    public void CloseDoor()
    {
        isOpen = false;
    }

    public void Interact()
    {
        if (isOpen) CloseDoor();
        else OpenDoor();
    }

    public string GetInteractionText() => isOpen ? "Kapat" : "Aç";
}