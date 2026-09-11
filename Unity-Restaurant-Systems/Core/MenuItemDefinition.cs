using UnityEngine;

namespace UnityGameSystems.Restaurant
{
    [CreateAssetMenu(menuName = "Restaurant/Menu Item", fileName = "MenuItem")]
    public sealed class MenuItemDefinition : ScriptableObject
    {
        [SerializeField] private string itemId;
        [SerializeField] private string displayName;
        [SerializeField, Min(0f)] private float price = 10f;
        [SerializeField, Min(0.1f)] private float preparationTime = 5f;
        [SerializeField] private Sprite icon;

        public string Id => itemId;
        public string DisplayName => displayName;
        public float Price => price;
        public float PreparationTime => preparationTime;
        public Sprite Icon => icon;
    }
}
