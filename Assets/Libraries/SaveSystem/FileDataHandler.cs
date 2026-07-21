using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Pospec.Saving
{
    internal class FileDataHandler : IDataHandler
    {
        private const string saveDirectory = "Pospec.Save";
        private string GetFilePath(string name, string group)
            => Path.Combine(Application.persistentDataPath, saveDirectory, group, name + ".json");

        public bool Load<T>(string name, string group, out T data) where T : SaveData
        {
            data = default;
            try
            {
                string path = GetFilePath(name, group);
                string json;

                if (!File.Exists(path))
                    return false;

                using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    using StreamReader reader = new StreamReader(stream);
                    json = reader.ReadToEnd();
                }

                data = JsonUtility.FromJson<T>(json);
                if (data == null)
                {
                    Debug.LogError("Can't deserialize object: " + json);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error while loading data: " + ex.Message);
                return false;
            }
            return true;
        }

        public bool Save<T>(T data, string name, string group) where T : SaveData
        {
            try
            {
                string json = JsonUtility.ToJson(data);
                if (string.IsNullOrEmpty(json))
                {
                    Debug.LogError("Empty object to serialize. Make sure, object has " + nameof(SerializableAttribute));
                    return false;
                }

                string path = GetFilePath(name, group);
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    using StreamWriter writer = new StreamWriter(stream);
                    writer.Write(json);
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError("Error while saving data: " + ex.Message);
                return false;
            }
        }

        public void ClearGroup(string group)
        {
            try
            {
                string dictionary = Path.GetDirectoryName(GetFilePath("", group));
                Directory.Delete(dictionary);
            }
            catch (Exception e)
            {
                Debug.LogError("Clear Group failed: " + e.Message);
            }
        }

        public IEnumerable<string> GetAllGroups(string pattern)
        {
            try
            {
                string directory = Path.Combine(Application.persistentDataPath, saveDirectory);
                return Directory.GetDirectories(directory, pattern, SearchOption.TopDirectoryOnly);
            }
            catch (Exception e)
            {
                Debug.LogError("GetAllGroups failed: " + e.Message);
                return new List<string>();
            }
        }
    }
}
