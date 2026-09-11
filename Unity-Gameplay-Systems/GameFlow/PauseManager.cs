using System;
using UnityEngine;

namespace UnityGameSystems.GameFlow
{
    [DisallowMultipleComponent]
    public sealed class PauseManager : MonoBehaviour
    {
        [SerializeField] private bool pauseOnStart;
        [SerializeField] private bool controlCursor = true;

        public bool IsPaused { get; private set; }
        public event Action<bool> PauseChanged;

        private void Start()
        {
            SetPaused(pauseOnStart);
        }

        private void OnDestroy()
        {
            if (IsPaused)
                Time.timeScale = 1f;
        }

        public void Toggle()
        {
            SetPaused(!IsPaused);
        }

        public void Pause()
        {
            SetPaused(true);
        }

        public void Resume()
        {
            SetPaused(false);
        }

        public void SetPaused(bool paused)
        {
            if (IsPaused == paused && Time.timeScale == (paused ? 0f : 1f)) return;

            IsPaused = paused;
            Time.timeScale = paused ? 0f : 1f;

            if (controlCursor)
            {
                Cursor.visible = paused;
                Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
            }

            PauseChanged?.Invoke(paused);
        }
    }
}
