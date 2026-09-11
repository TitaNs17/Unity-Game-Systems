using UnityEngine;

namespace UnityGameSystems.AI.Customers
{
    [CreateAssetMenu(menuName = "AI/Customer Personality", fileName = "CustomerPersonality")]
    public sealed class CustomerPersonality : ScriptableObject
    {
        [SerializeField, Range(0f, 1f)] private float patience = 0.5f;
        [SerializeField, Range(0f, 1f)] private float generosity = 0.5f;
        [SerializeField, Range(0f, 1f)] private float cleanlinessTolerance = 0.5f;
        [SerializeField, Range(0f, 1f)] private float socialNeed = 0.5f;

        public float Patience => patience;
        public float Generosity => generosity;
        public float CleanlinessTolerance => cleanlinessTolerance;
        public float SocialNeed => socialNeed;
    }
}
