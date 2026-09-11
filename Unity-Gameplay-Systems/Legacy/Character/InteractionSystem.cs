using TMPro;
using UnityEngine;

public class InteractionSystem : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float interactDistance = 3f;
    [SerializeField] private Camera cam;
    [SerializeField] private KeyCode key = KeyCode.E;
    [SerializeField] private TMP_Text interactionText;
    [SerializeField] private LayerMask interactionMask = ~0;

    private IInteractable current;

    private void Reset()
    {
        cam = GetComponentInChildren<Camera>();
    }

    private void Awake()
    {
        if (cam == null)
            cam = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        DetectInteractable();

        if (current != null && Input.GetKeyDown(key))
            current.Interact();
    }

    private void DetectInteractable()
    {
        current = null;
        SetPrompt(string.Empty);

        if (cam == null)
            return;

        var ray = new Ray(cam.transform.position, cam.transform.forward);
        if (!Physics.Raycast(ray, out var hit, interactDistance, interactionMask, QueryTriggerInteraction.Collide))
            return;

        current = FindInteractable(hit.collider);
        if (current != null)
            SetPrompt($"[{key}] {current.GetInteractionText()}");
    }

    private static IInteractable FindInteractable(Collider collider)
    {
        if (collider == null)
            return null;

        var behaviours = collider.GetComponentsInParent<MonoBehaviour>();
        foreach (var behaviour in behaviours)
        {
            if (behaviour is IInteractable interactable)
                return interactable;
        }

        return null;
    }

    private void SetPrompt(string value)
    {
        if (interactionText != null)
            interactionText.text = value;
    }
}
