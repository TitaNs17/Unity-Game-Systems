using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityGameSystems.Scenes
{
    public sealed class SceneTransitionService : MonoBehaviour
    {
        public bool IsLoading { get; private set; }
        public float Progress { get; private set; }

        public event Action<string> LoadStarted;
        public event Action<string> LoadCompleted;

        public bool Load(string sceneName)
        {
            if (IsLoading || string.IsNullOrWhiteSpace(sceneName)) return false;
            StartCoroutine(LoadRoutine(sceneName));
            return true;
        }

        private IEnumerator LoadRoutine(string sceneName)
        {
            IsLoading = true;
            Progress = 0f;
            LoadStarted?.Invoke(sceneName);

            var operation = SceneManager.LoadSceneAsync(sceneName);
            if (operation == null)
            {
                IsLoading = false;
                yield break;
            }

            operation.allowSceneActivation = false;

            while (operation.progress < 0.9f)
            {
                Progress = Mathf.Clamp01(operation.progress / 0.9f);
                yield return null;
            }

            Progress = 1f;
            operation.allowSceneActivation = true;

            while (!operation.isDone)
                yield return null;

            IsLoading = false;
            LoadCompleted?.Invoke(sceneName);
        }
    }
}
