using System.IO;
using UnityEngine;

namespace MyFps
{
    [System.Serializable]
    public class SaveData
    {
        public int sceneBuildIndex;
    }

    public static class SaveSystem
    {
        private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

        public static void Save(int sceneBuildIndex)
        {
            SaveData data = new SaveData { sceneBuildIndex = sceneBuildIndex };
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath, json);
            Debug.Log($"Game Saved: Scene Index {sceneBuildIndex} to {SavePath}");
        }

        public static SaveData Load()
        {
            if (!HasSaveFile())
            {
                return null;
            }

            try
            {
                string json = File.ReadAllText(SavePath);
                return JsonUtility.FromJson<SaveData>(json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load save data: {e.Message}");
                return null;
            }
        }

        public static bool HasSaveFile()
        {
            return File.Exists(SavePath);
        }

        public static void DeleteSaveFile()
        {
            if (HasSaveFile())
            {
                File.Delete(SavePath);
            }
        }
    }
}
