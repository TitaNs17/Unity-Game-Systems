using System;
using UnityEngine;

namespace UnityGameSystems.Quests
{
    [Serializable]
    public struct QuestObjective
    {
        public string id;
        public string description;
        [Min(1)] public int requiredAmount;
    }

    [CreateAssetMenu(menuName = "Game/Quest", fileName = "Quest")]
    public sealed class QuestDefinition : ScriptableObject
    {
        [SerializeField] private string questId;
        [SerializeField] private string title;
        [SerializeField] private QuestObjective[] objectives = Array.Empty<QuestObjective>();

        public string Id => questId;
        public string Title => title;
        public QuestObjective[] Objectives => objectives ?? Array.Empty<QuestObjective>();
    }
}
