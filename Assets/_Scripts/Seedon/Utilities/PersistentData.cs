using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using EasyButtons;

namespace Seedon
{
    public class PersistentData : MonoBehaviour
    {
        private Dictionary<string, DataEntry> _data = new Dictionary<string, DataEntry>();
        private string _savePath;

        [SerializeField] private float _saveInterval = 5f;
        private bool _isAutoSaving = false;

        private void OnEnable()
        {
            _savePath = Path.Combine(Application.persistentDataPath, "savefile.json");
            LoadData();
            StartAutoSave();
        }

        private void OnDisable()
        {
            _isAutoSaving = false;
            SaveData();
        }

        private void StartAutoSave()
        {
            if (_isAutoSaving) return;
            _isAutoSaving = true;
            AutoSaveLoop().Forget();
        }

        private async UniTaskVoid AutoSaveLoop()
        {
            while (_isAutoSaving)
            {
                await UniTask.Delay((int)(_saveInterval * 1000), cancellationToken: destroyCancellationToken);
                if (_isAutoSaving)
                {
                    SaveData();
                }
            }
        }

        public bool SaveFileExists() => File.Exists(_savePath);

        [Button]
        public void DeleteSaveFile()
        {
            if (SaveFileExists())
            {
                _data = new Dictionary<string, DataEntry>();
                File.Delete(_savePath);
            }
        }

        public void Set(string key, string value)
        {
            _data[key] = new DataEntry { stringValue = value, dataType = DataEntry.DataType.String };
        }

        public void Set(string key, int value)
        {
            _data[key] = new DataEntry { intValue = value, dataType = DataEntry.DataType.Int };
        }

        public void Set(string key, float value)
        {
            _data[key] = new DataEntry { floatValue = value, dataType = DataEntry.DataType.Float };
        }

        public T Get<T>(string key, T defaultValue = default(T))
        {
            if (!_data.ContainsKey(key))
                return defaultValue;

            DataEntry entry = _data[key];

            if (typeof(T) == typeof(string))
                return (T)(object)entry.stringValue;

            if (typeof(T) == typeof(int))
                return (T)(object)(entry.dataType == DataEntry.DataType.Float
                    ? (int)entry.floatValue
                    : entry.intValue);

            if (typeof(T) == typeof(float))
                return (T)(object)(entry.dataType == DataEntry.DataType.Int
                    ? (float)entry.intValue
                    : entry.floatValue);

            return defaultValue;
        }

        public bool HasKey(string key) => _data.ContainsKey(key);

        public void SaveData()
        {
            string jsonData = JsonUtility.ToJson(new SerializableDictionary<DataEntry>(_data), true);
            File.WriteAllText(_savePath, jsonData);
        }

        public void LoadData()
        {
            if (File.Exists(_savePath))
            {
                string jsonData = File.ReadAllText(_savePath);
                _data = JsonUtility.FromJson<SerializableDictionary<DataEntry>>(jsonData).ToDictionary();
            }
            else
            {
                _data = new Dictionary<string, DataEntry>();
            }
        }

        [Button]
        public void ShowAllData()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("All stored data:\n");

            foreach (var entry in _data)
            {
                string key = entry.Key;
                DataEntry value = entry.Value;

                string info = value.dataType switch
                {
                    DataEntry.DataType.String => value.stringValue,
                    DataEntry.DataType.Int => value.intValue.ToString(),
                    DataEntry.DataType.Float => value.floatValue.ToString(),
                    _ => "Unknown"
                };

                sb.Append($"Key: {key}, Value: {info}, Type: {value.dataType}\n");
            }

            Debug.Log(sb.ToString());
        }
    }

    [System.Serializable]
    public class SerializableDictionary<T>
    {
        public List<string> keys = new List<string>();
        public List<T> values = new List<T>();

        public SerializableDictionary(Dictionary<string, T> dictionary)
        {
            foreach (var kvp in dictionary)
            {
                keys.Add(kvp.Key);
                values.Add(kvp.Value);
            }
        }

        public Dictionary<string, T> ToDictionary()
        {
            var result = new Dictionary<string, T>();
            for (int i = 0; i < keys.Count; i++)
            {
                result.Add(keys[i], values[i]);
            }
            return result;
        }
    }

    [System.Serializable]
    public class DataEntry
    {
        public string stringValue;
        public int intValue;
        public float floatValue;
        public DataType dataType;

        public enum DataType
        {
            String,
            Int,
            Float
        }
    }
}