using System;
using UnityEngine;

namespace UnityGameSystems.Interaction
{
    public sealed class Interactor : MonoBehaviour
    {
        [SerializeField] private Camera viewCamera;
        [SerializeField, Min(0.1f)] private float range = 3f;
        [SerializeField] private LayerMask interactionMask = ~0;

        public event Action<IInteractable> FocusChanged;

        public IInteractable Focused { get; private set; }
        public Transform Owner => transform;

        private void Reset()
        {
            viewCamera = GetComponentInChildren<Camera>();
        }

        private void Update()
        {
            RefreshFocus();
        }

        public bool TryInteract()
        {
            if (Focused == null || !Focused.CanInteract(this))
                return false;

            Focused.Interact(this);
            return true;
        }

        private void RefreshFocus()
        {
            IInteractable next = null;

            if (viewCamera != null && Physics.Raycast(
                    viewCamera.transform.position,
                    viewCamera.transform.forward,
                    out var hit,
                    range,
                    interactionMask,
                    QueryTriggerInteraction.Collide))
            {
                next = FindInteractable(hit.collider);
            }

            if (ReferenceEquals(next, Focused)) return;

            Focused = next;
            FocusChanged?.Invoke(Focused);
        }

        private static IInteractable FindInteractable(Collider collider)
        {
            var behaviours = collider.GetComponentsInParent<MonoBehaviour>();
            foreach (var behaviour in behaviours)
            {
                if (behaviour is IInteractable interactable)
                    return interactable;
            }

            return null;
        }
    }
}
