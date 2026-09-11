using System;
using UnityEngine;

namespace UnityGameSystems.Settings
{
    [Serializable]
    public sealed class GameSettingsData
    {
        public float masterVolume = 1f;
        public float musicVolume = 1f;
        public float sfxVolume = 1f;
        public int qualityLevel = -1;
        public bool fullscreen = true;
        public int targetFrameRate = -1;
    }

    public sealed class GameSettings : MonoBehaviour
    {
        private const string PrefsKey = "unity_game_systems.settings";

        [SerializeField] private GameSettingsData defaults = new GameSettingsData();
        [SerializeField] private bool loadOnAwake = true;

        public GameSettingsData Current { get; private set; }
        public event Action<GameSettingsData> Changed;

        private void Awake()
        {
            if (loadOnAwake)
                Load();
            else
                Apply(Clone(defaults));
        }

        public void Load()
        {
            var data = Clone(defaults);

            if (PlayerPrefs.HasKey(PrefsKey))
            {
                try
                {
                    var loaded = JsonUtility.FromJson<GameSettingsData>(PlayerPrefs.GetString(PrefsKey));
                    if (loaded != null)
                        data = loaded;
                }
                catch
                {
                    data = Clone(defaults);
                }
            }

            Apply(data);
        }

        public void Save()
        {
            if (Current == null) return;
            PlayerPrefs.SetString(PrefsKey, JsonUtility.ToJson(Current));
            PlayerPrefs.Save();
        }

        public void ResetToDefaults()
        {
            Apply(Clone(defaults));
            Save();
        }

        public void SetMasterVolume(float value)
        {
            EnsureCurrent();
            Current.masterVolume = Mathf.Clamp01(value);
            ApplyRuntimeValues();
            Changed?.Invoke(Current);
        }

        public void SetQualityLevel(int level)
        {
            EnsureCurrent();
            Current.qualityLevel = level;
            ApplyRuntimeValues();
            Changed?.Invoke(Current);
        }

        public void SetFullscreen(bool value)
        {
            EnsureCurrent();
            Current.fullscreen = value;
            ApplyRuntimeValues();
            Changed?.Invoke(Current);
        }

        public void SetTargetFrameRate(int value)
        {
            EnsureCurrent();
            Current.targetFrameRate = value;
            ApplyRuntimeValues();
            Changed?.Invoke(Current);
        }

        private void Apply(GameSettingsData data)
        {
            Current = data ?? Clone(defaults);
            Current.masterVolume = Mathf.Clamp01(Current.masterVolume);
            Current.musicVolume = Mathf.Clamp01(Current.musicVolume);
            Current.sfxVolume = Mathf.Clamp01(Current.sfxVolume);
            ApplyRuntimeValues();
            Changed?.Invoke(Current);
        }

        private void ApplyRuntimeValues()
        {
            AudioListener.volume = Current.masterVolume;
            Screen.fullScreen = Current.fullscreen;
            Application.targetFrameRate = Current.targetFrameRate;

            if (Current.qualityLevel >= 0 && Current.qualityLevel < QualitySettings.names.Length)
                QualitySettings.SetQualityLevel(Current.qualityLevel, true);
        }

        private void EnsureCurrent()
        {
            if (Current == null)
                Current = Clone(defaults);
        }

        private static GameSettingsData Clone(GameSettingsData source)
        {
            if (source == null) return new GameSettingsData();
            return JsonUtility.FromJson<GameSettingsData>(JsonUtility.ToJson(source));
        }
    }
}
