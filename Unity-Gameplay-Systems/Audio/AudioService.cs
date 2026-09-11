using System.Collections.Generic;
using UnityEngine;

namespace UnityGameSystems.Audio
{
    public sealed class AudioService : MonoBehaviour
    {
        [SerializeField] private AudioSource sourcePrefab;
        [SerializeField, Min(1)] private int initialPoolSize = 6;

        private readonly Queue<AudioSource> available = new();
        private readonly List<AudioSource> playing = new();

        private void Awake()
        {
            for (var i = 0; i < initialPoolSize; i++)
                available.Enqueue(CreateSource());
        }

        private void Update()
        {
            for (var i = playing.Count - 1; i >= 0; i--)
            {
                if (playing[i].isPlaying) continue;
                available.Enqueue(playing[i]);
                playing.RemoveAt(i);
            }
        }

        public void Play(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
        {
            if (clip == null) return;

            var source = available.Count > 0 ? available.Dequeue() : CreateSource();
            source.transform.position = position;
            source.clip = clip;
            source.volume = Mathf.Clamp01(volume);
            source.pitch = Mathf.Clamp(pitch, -3f, 3f);
            source.Play();
            playing.Add(source);
        }

        private AudioSource CreateSource()
        {
            var source = Instantiate(sourcePrefab, transform);
            source.playOnAwake = false;
            return source;
        }
    }
}
