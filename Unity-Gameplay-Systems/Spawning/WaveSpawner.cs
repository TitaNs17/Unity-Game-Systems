using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameSystems.Combat;

namespace UnityGameSystems.Spawning
{
    [Serializable]
    public sealed class WaveDefinition
    {
        public GameObject enemyPrefab;
        [Min(1)] public int count = 5;
        [Min(0f)] public float spawnInterval = 0.75f;
    }

    public sealed class WaveSpawner : MonoBehaviour
    {
        [SerializeField] private WaveDefinition[] waves = Array.Empty<WaveDefinition>();
        [SerializeField] private Transform[] spawnPoints = Array.Empty<Transform>();
        [SerializeField, Min(0f)] private float delayBetweenWaves = 3f;
        [SerializeField] private bool playOnStart;

        private readonly HashSet<Health> alive = new HashSet<Health>();
        private Coroutine routine;

        public int CurrentWaveIndex { get; private set; } = -1;
        public int AliveCount => alive.Count;
        public bool IsRunning => routine != null;

        public event Action<int> WaveStarted;
        public event Action<int> WaveCompleted;
        public event Action AllWavesCompleted;

        private void Start()
        {
            if (playOnStart)
                StartWaves();
        }

        private void OnDisable()
        {
            if (routine != null)
            {
                StopCoroutine(routine);
                routine = null;
            }

            UnsubscribeAll();
        }

        public bool StartWaves()
        {
            if (routine != null || waves == null || waves.Length == 0 || spawnPoints == null || spawnPoints.Length == 0)
                return false;

            routine = StartCoroutine(RunWaves());
            return true;
        }

        public void StopWaves()
        {
            if (routine != null)
            {
                StopCoroutine(routine);
                routine = null;
            }
        }

        private IEnumerator RunWaves()
        {
            for (int waveIndex = 0; waveIndex < waves.Length; waveIndex++)
            {
                var wave = waves[waveIndex];
                if (wave == null || wave.enemyPrefab == null || wave.count <= 0)
                    continue;

                CurrentWaveIndex = waveIndex;
                WaveStarted?.Invoke(waveIndex);

                for (int i = 0; i < wave.count; i++)
                {
                    Spawn(wave.enemyPrefab, spawnPoints[i % spawnPoints.Length]);

                    if (wave.spawnInterval > 0f && i < wave.count - 1)
                        yield return new WaitForSeconds(wave.spawnInterval);
                }

                while (alive.Count > 0)
                    yield return null;

                WaveCompleted?.Invoke(waveIndex);

                if (waveIndex < waves.Length - 1 && delayBetweenWaves > 0f)
                    yield return new WaitForSeconds(delayBetweenWaves);
            }

            CurrentWaveIndex = -1;
            routine = null;
            AllWavesCompleted?.Invoke();
        }

        private void Spawn(GameObject prefab, Transform point)
        {
            if (prefab == null || point == null) return;

            var instance = Instantiate(prefab, point.position, point.rotation);
            var health = instance.GetComponentInChildren<Health>();
            if (health == null) return;

            alive.Add(health);
            health.Died += () => HandleEnemyDeath(health);
        }

        private void HandleEnemyDeath(Health health)
        {
            if (health == null) return;
            alive.Remove(health);
        }

        private void UnsubscribeAll()
        {
            alive.Clear();
        }
    }
}
