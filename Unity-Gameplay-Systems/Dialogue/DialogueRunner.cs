using System;
using UnityEngine;
using UnityEngine.Events;

namespace UnityGameSystems.Dialogue
{
    [Serializable]
    public sealed class DialogueLineEvent : UnityEvent<string, string> { }

    public sealed class DialogueRunner : MonoBehaviour
    {
        [SerializeField] private AudioSource voiceSource;
        [SerializeField] private DialogueLineEvent lineChanged = new DialogueLineEvent();
        [SerializeField] private UnityEvent dialogueStarted = new UnityEvent();
        [SerializeField] private UnityEvent dialogueFinished = new UnityEvent();

        private DialogueData current;
        private int index = -1;

        public bool IsRunning => current != null;
        public DialogueData CurrentDialogue => current;
        public int CurrentIndex => index;

        public event Action<DialogueLine> LineChanged;
        public event Action Started;
        public event Action Finished;

        public bool StartDialogue(DialogueData dialogue)
        {
            if (dialogue == null || dialogue.Count == 0) return false;

            current = dialogue;
            index = 0;
            Started?.Invoke();
            dialogueStarted.Invoke();
            ShowCurrentLine();
            return true;
        }

        public bool Next()
        {
            if (current == null) return false;

            index++;
            if (index >= current.Count)
            {
                Stop();
                return false;
            }

            ShowCurrentLine();
            return true;
        }

        public void Stop()
        {
            if (current == null) return;

            if (voiceSource != null)
                voiceSource.Stop();

            current = null;
            index = -1;
            Finished?.Invoke();
            dialogueFinished.Invoke();
        }

        private void ShowCurrentLine()
        {
            if (current == null || index < 0 || index >= current.Count) return;

            var line = current.GetLine(index);

            if (voiceSource != null)
            {
                voiceSource.Stop();
                voiceSource.clip = line.voiceClip;
                if (line.voiceClip != null)
                    voiceSource.Play();
            }

            LineChanged?.Invoke(line);
            lineChanged.Invoke(line.speaker, line.text);
        }
    }
}
