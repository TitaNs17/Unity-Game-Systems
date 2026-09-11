using UnityEngine;

public class BroomSystem : MonoBehaviour
{
    [Header("--- AYARLAR ---")]
    public float interactionDistance = 3f;
    public KeyCode interactKey = KeyCode.E;
    public float throwForce = 2.5f;

    [Header("--- TEMİZLİK HIZI ---")]
    public float cleaningRadius = 1.5f;
    public float scrubSpeed = 1.5f; 

    [Header("--- BAĞLANTILAR ---")]
    public Transform handPoint;
    public DirtManager dirtManager;

    private GameObject heldObject = null;

    void Update()
    {
     
        if (Input.GetKeyDown(interactKey))
        {
            if (heldObject == null) TryGrab();
            else DropObject();
        }

       
        if (Input.GetMouseButton(0) && heldObject != null)
        {
            if (heldObject.CompareTag("Supurge"))
            {
                ScrubArea();
            }
        }
    }

    void ScrubArea()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            Collider[] hitColliders = Physics.OverlapSphere(hit.point, cleaningRadius);

            foreach (var col in hitColliders)
            {
                if (col.CompareTag("Kir"))
                {
                    Renderer rend = col.GetComponent<Renderer>();
                    if (rend != null)
                    {
                        
                        Color currentColor = rend.material.color;

                        
                        currentColor.a -= Time.deltaTime * scrubSpeed;
                        rend.material.color = currentColor;

                        
                        if (currentColor.a <= 0.01f)
                        {
                            dirtManager.RemoveDirt(col.gameObject);
                        }
                    }
                }
            }
        }
    }

    void TryGrab()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.CompareTag("Supurge"))
            {
                PickUpObject(hit.collider.gameObject);
            }
        }
    }

    void PickUpObject(GameObject obj)
    {
        heldObject = obj;
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;
        Collider[] cols = heldObject.GetComponents<Collider>();
        foreach (Collider c in cols) c.enabled = false;

        heldObject.transform.SetParent(handPoint);
        heldObject.transform.localPosition = Vector3.zero;
        heldObject.transform.localRotation = Quaternion.identity;
    }

    void DropObject()
    {
        heldObject.transform.SetParent(null);
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = false;
            rb.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse);
        }
        Collider[] cols = heldObject.GetComponents<Collider>();
        foreach (Collider c in cols) c.enabled = true;
        heldObject = null;
    }
}