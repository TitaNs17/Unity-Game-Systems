using UnityEngine;

namespace UnityGameSystems.Abilities
{
    [CreateAssetMenu(menuName = "Game/Ability", fileName = "Ability")]
    public sealed class AbilityDefinition : ScriptableObject
    {
        [SerializeField] private string abilityId;
        [SerializeField] private float cooldown = 1f;
        [SerializeField] private float resourceCost;

        public string Id => abilityId;
        public float Cooldown => Mathf.Max(0f, cooldown);
        public float ResourceCost => Mathf.Max(0f, resourceCost);
    }
}
