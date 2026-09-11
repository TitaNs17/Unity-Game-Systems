using System;
using System.IO;
using UnityEngine;

namespace UnityGameSystems.Save
{
    public sealed class SaveProfileService
    {
        private const int CurrentVersion = 2;
        private readonly string savePath;
        private readonly string backupPath;

        public SaveProfileService(string fileName = "profile.json")
        {
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = "profile.json";

            savePath = Path.Combine(Application.persistentDataPath, fileName);
            backupPath = savePath + ".bak";
        }

        public bool Exists => File.Exists(savePath);
        public string PathOnDisk => savePath;

        public bool Save(SaveProfile profile)
        {
            if (profile == null) return false;

            try
            {
                profile.version = CurrentVersion;
                var directory = Path.GetDirectoryName(savePath);
                if (!string.IsNullOrEmpty(directory))
                    Directory.CreateDirectory(directory);

                var json = JsonUtility.ToJson(profile, true);
                var tempPath = savePath + ".tmp";
                File.WriteAllText(tempPath, json);

                if (File.Exists(savePath))
                    File.Copy(savePath, backupPath, true);

                if (File.Exists(savePath))
                    File.Delete(savePath);

                File.Move(tempPath, savePath);
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError($"Save failed: {exception.Message}");
                return false;
            }
        }

        public bool TryLoad(out SaveProfile profile)
        {
            if (TryLoadFile(savePath, out profile))
                return true;

            return TryLoadFile(backupPath, out profile);
        }

        public void Delete()
        {
            DeleteIfExists(savePath);
            DeleteIfExists(backupPath);
            DeleteIfExists(savePath + ".tmp");
        }

        private static bool TryLoadFile(string path, out SaveProfile profile)
        {
            profile = null;
            if (!File.Exists(path)) return false;

            try
            {
                var json = File.ReadAllText(path);
                var loaded = JsonUtility.FromJson<SaveProfile>(json);
                if (loaded == null || loaded.version > CurrentVersion)
                    return false;

                if (loaded.values == null)
                    loaded.values = new System.Collections.Generic.List<SaveValue>();

                profile = loaded;
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Could not load save file '{path}': {exception.Message}");
                return false;
            }
        }

        private static void DeleteIfExists(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}
