using UnityEngine;

public class CuttingSystem : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float cutRange = 10f;
    [SerializeField] private LayerMask cutLayer;
    [SerializeField] private KeyCode cutKey = KeyCode.Mouse0;

    private Camera cam;
    private AlmaBirakmaSistemi almaSistemi;

    private void Awake()
    {
        cam = Camera.main;
        almaSistemi = FindFirstObjectByType<AlmaBirakmaSistemi>();
    }

    private void Update()
    {
        if (!Input.GetKeyDown(cutKey) || cam == null || almaSistemi == null)
            return;

        var held = almaSistemi.tutulanObje;
        if (held == null || !held.CompareTag("Bicak"))
            return;

        TryCut();
    }

    private void TryCut()
    {
        var ray = cam.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hit, cutRange, cutLayer, QueryTriggerInteraction.Ignore))
            return;

        var sliceable = hit.collider.GetComponentInParent<SliceableObject>();
        if (sliceable != null)
            sliceable.SliceOnce();
    }
}
