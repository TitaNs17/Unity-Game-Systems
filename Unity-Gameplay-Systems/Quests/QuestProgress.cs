using System;
using System.Collections.Generic;

namespace UnityGameSystems.Quests
{
    public sealed class QuestProgress
    {
        private readonly QuestDefinition definition;
        private readonly Dictionary<string, int> progress = new();

        public event Action Changed;
        public event Action Completed;

        public QuestDefinition Definition => definition;
        public bool IsComplete { get; private set; }

        public QuestProgress(QuestDefinition definition)
        {
            this.definition = definition ?? throw new ArgumentNullException(nameof(definition));

            foreach (var objective in definition.Objectives)
                progress[objective.id] = 0;
        }

        public int GetProgress(string objectiveId)
        {
            return progress.TryGetValue(objectiveId, out var amount) ? amount : 0;
        }

        public bool AddProgress(string objectiveId, int amount = 1)
        {
            if (IsComplete || amount <= 0 || !progress.ContainsKey(objectiveId)) return false;

            progress[objectiveId] += amount;
            Changed?.Invoke();
            CheckCompletion();
            return true;
        }

        private void CheckCompletion()
        {
            foreach (var objective in definition.Objectives)
            {
                if (GetProgress(objective.id) < objective.requiredAmount)
                    return;
            }

            IsComplete = true;
            Completed?.Invoke();
        }
    }
}
