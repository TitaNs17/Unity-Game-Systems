namespace UnityGameSystems.Interaction
{
    public interface IInteractable
    {
        string InteractionLabel { get; }
        bool CanInteract(Interactor interactor);
        void Interact(Interactor interactor);
    }
}
