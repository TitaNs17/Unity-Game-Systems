using System;
using System.IO;
using UnityEngine;

namespace UnityGameSystems.Save
{
    [Serializable]
    public sealed class SaveGameData
    {
        public int version = 1;
        public string sceneName;
        public float playTimeSeconds;
        public Vector3 playerPosition;
    }

    public sealed class SaveGameService
    {
        private const int CurrentVersion = 1;
        private readonly string savePath;

        public SaveGameService(string fileName = "savegame.json")
        {
            savePath = Path.Combine(Application.persistentDataPath, fileName);
        }

        public bool Exists => File.Exists(savePath);

        public void Save(SaveGameData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            data.version = CurrentVersion;
            var json = JsonUtility.ToJson(data, true);
            var tempPath = savePath + ".tmp";

            File.WriteAllText(tempPath, json);

            if (File.Exists(savePath))
                File.Delete(savePath);

            File.Move(tempPath, savePath);
        }

        public bool TryLoad(out SaveGameData data)
        {
            data = null;
            if (!Exists) return false;

            try
            {
                var json = File.ReadAllText(savePath);
                data = JsonUtility.FromJson<SaveGameData>(json);
                return data != null && data.version <= CurrentVersion;
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Could not load save file: {exception.Message}");
                return false;
            }
        }

        public void Delete()
        {
            if (Exists)
                File.Delete(savePath);
        }
    }
}
