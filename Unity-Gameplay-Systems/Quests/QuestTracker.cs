using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGameSystems.Quests
{
    public sealed class QuestTracker : MonoBehaviour
    {
        private readonly Dictionary<string, QuestProgress> active = new Dictionary<string, QuestProgress>();

        public IEnumerable<QuestProgress> ActiveQuests => active.Values;
        public event Action<QuestProgress> QuestStarted;
        public event Action<QuestProgress> QuestChanged;
        public event Action<QuestProgress> QuestCompleted;

        public bool StartQuest(QuestDefinition definition)
        {
            if (definition == null || string.IsNullOrEmpty(definition.Id) || active.ContainsKey(definition.Id))
                return false;

            var progress = new QuestProgress(definition);
            progress.Changed += () => QuestChanged?.Invoke(progress);
            progress.Completed += () => QuestCompleted?.Invoke(progress);
            active.Add(definition.Id, progress);
            QuestStarted?.Invoke(progress);
            return true;
        }

        public bool AddProgress(string questId, string objectiveId, int amount = 1)
        {
            if (string.IsNullOrEmpty(questId) || string.IsNullOrEmpty(objectiveId))
                return false;

            return active.TryGetValue(questId, out var quest) && quest.AddProgress(objectiveId, amount);
        }

        public bool TryGetQuest(string questId, out QuestProgress progress)
        {
            return active.TryGetValue(questId, out progress);
        }

        public bool RemoveQuest(string questId)
        {
            return !string.IsNullOrEmpty(questId) && active.Remove(questId);
        }

        public void Clear()
        {
            active.Clear();
        }
    }
}
