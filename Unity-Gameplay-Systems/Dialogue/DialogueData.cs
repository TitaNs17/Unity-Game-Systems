using System;
using UnityEngine;

namespace UnityGameSystems.Dialogue
{
    [Serializable]
    public struct DialogueLine
    {
        public string speaker;
        [TextArea(2, 5)] public string text;
        public AudioClip voiceClip;
    }

    [CreateAssetMenu(menuName = "Game/Dialogue", fileName = "Dialogue")]
    public sealed class DialogueData : ScriptableObject
    {
        [SerializeField] private DialogueLine[] lines = Array.Empty<DialogueLine>();

        public int Count => lines != null ? lines.Length : 0;

        public DialogueLine GetLine(int index)
        {
            if (lines == null || index < 0 || index >= lines.Length)
                throw new IndexOutOfRangeException();

            return lines[index];
        }
    }
}
