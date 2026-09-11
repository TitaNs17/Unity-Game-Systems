using UnityEngine;

namespace UnityGameSystems.Inventory
{
    public enum ItemCategory
    {
        Resource,
        Consumable,
        Tool,
        Quest,
        Equipment
    }

    [CreateAssetMenu(menuName = "Game/Item", fileName = "Item")]
    public sealed class ItemDefinition : ScriptableObject
    {
        [SerializeField] private string itemId;
        [SerializeField] private string displayName;
        [SerializeField] private ItemCategory category;
        [SerializeField, Min(1)] private int maxStack = 1;
        [SerializeField] private Sprite icon;
        [SerializeField] private GameObject worldPrefab;

        public string Id => itemId;
        public string DisplayName => displayName;
        public ItemCategory Category => category;
        public int MaxStack => maxStack;
        public Sprite Icon => icon;
        public GameObject WorldPrefab => worldPrefab;
    }
}
