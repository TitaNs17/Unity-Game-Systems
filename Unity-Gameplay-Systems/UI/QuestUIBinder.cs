using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityGameSystems.Quests;

namespace UnityGameSystems.UI
{
    public sealed class QuestUIBinder : MonoBehaviour
    {
        [SerializeField] private QuestTracker tracker;
        [SerializeField] private Text titleText;
        [SerializeField] private Text objectiveText;
        [SerializeField] private GameObject root;

        private QuestProgress current;

        private void OnEnable()
        {
            if (tracker == null) return;

            tracker.QuestStarted += HandleQuestStarted;
            tracker.QuestChanged += HandleQuestChanged;
            tracker.QuestCompleted += HandleQuestCompleted;
        }

        private void OnDisable()
        {
            if (tracker == null) return;

            tracker.QuestStarted -= HandleQuestStarted;
            tracker.QuestChanged -= HandleQuestChanged;
            tracker.QuestCompleted -= HandleQuestCompleted;
        }

        public void ShowQuest(string questId)
        {
            if (tracker != null && tracker.TryGetQuest(questId, out var progress))
            {
                current = progress;
                Refresh();
            }
        }

        private void HandleQuestStarted(QuestProgress progress)
        {
            if (current == null)
                current = progress;
            Refresh();
        }

        private void HandleQuestChanged(QuestProgress progress)
        {
            if (ReferenceEquals(current, progress))
                Refresh();
        }

        private void HandleQuestCompleted(QuestProgress progress)
        {
            if (ReferenceEquals(current, progress))
                Refresh();
        }

        private void Refresh()
        {
            var visible = current != null;
            if (root != null)
                root.SetActive(visible);

            if (!visible) return;

            if (titleText != null)
                titleText.text = current.Definition.Title + (current.IsComplete ? " (Complete)" : string.Empty);

            if (objectiveText == null) return;

            var builder = new StringBuilder();
            var objectives = current.Definition.Objectives;
            for (int i = 0; i < objectives.Length; i++)
            {
                var objective = objectives[i];
                if (i > 0) builder.AppendLine();
                builder.Append(objective.description);
                builder.Append("  ");
                builder.Append(Mathf.Min(current.GetProgress(objective.id), objective.requiredAmount));
                builder.Append('/');
                builder.Append(objective.requiredAmount);
            }

            objectiveText.text = builder.ToString();
        }
    }
}
