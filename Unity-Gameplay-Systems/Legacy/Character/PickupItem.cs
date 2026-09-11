using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    public string itemName = "Obje";

    [Header("Hold Offset")]
    public Vector3 holdPositionOffset = Vector3.zero;
    public Vector3 holdRotationOffset = Vector3.zero;

    public string GetInteractionText()
    {
        return "Al: " + itemName;
    }

    public void Interact()
    {
        var almaSistemi = FindFirstObjectByType<AlmaBirakmaSistemi>();
        if (almaSistemi != null && almaSistemi.tutulanObje == null)
            almaSistemi.Al(gameObject);
    }
}
