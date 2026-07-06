using UnityEngine;
using TMPro;

public class InteractionSystem : MonoBehaviour
{
    public float interactDistance = 3f;
    public Camera cam;
    public KeyCode key = KeyCode.E;

    public TMP_Text interactionText; 

    private IInteractable current;

    void Update()
    {
        DetectInteractable();
        HandleInput();
    }

    void DetectInteractable()
    {
        current = null;
        interactionText.text = "";

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            current = hit.collider.GetComponent<IInteractable>();

            if (current != null)
                interactionText.text = "[E] " + current.GetInteractionText();
        }
    }

    void HandleInput()
    {
        if (current != null && Input.GetKeyDown(key))
        {
            current.Interact();
        }
    }
}
