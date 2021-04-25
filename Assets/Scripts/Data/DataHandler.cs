using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace DataSaving
{
    #region Data Handler
    public static class DataHandler
    {
        private static readonly object WriteReadLock = new object();
        public static string persistentPath = Application.persistentDataPath;
        #region Naming Conventions
        private static string DirectoryPath => persistentPath + "/Saves/";
        private static string GetFilePath(Type type) => DirectoryPath + GetFileName(type) + ".txt";
        private static string GetFileName(Type type) => type.ToString().Replace("+", "_");
        private static string GetJson(object saveObj) => JsonUtility.ToJson(saveObj, true);
        private static bool FileExists(Type type) => File.Exists(GetFilePath(type));
        #endregion
        #region Cache
        private static readonly Dictionary<Type, DictionaryItem> dataDictionary = new Dictionary<Type, DictionaryItem>();
        private class DictionaryItem
        {
            public IDirtyData item;
            public DictionaryItem(IDirtyData item)
            {
                this.item = item;
            }
        }
        #endregion
        #region interface
        public static T GetData<T>() where T : class, IDirtyData, new()
        {
            if (dataDictionary.TryGetValue(typeof(T), out DictionaryItem instance))
                return (T)instance.item;

            if (TryLoad(out T item))
                return item;

            return null;
        }
        public static void SetData<T>(T value) where T : class, IDirtyData, new()
        {
            if (dataDictionary.ContainsKey(typeof(T)))
                dataDictionary[typeof(T)].item = value;
            else
                dataDictionary.Add(typeof(T), new DictionaryItem(value));
        }

        public static async Task<bool> SaveAllAsync(Action<bool> callback = null)
        {
            bool success = true;
            foreach (var key in dataDictionary.Keys)
                success &= await SaveAsync(key);
            callback?.Invoke(success);
            return success;
        }
        public static async Task<bool> SaveAsync(Type type, Action<bool> callback = null)
        {
            var task = new Task<bool>(() => TrySave(type));
            task.Start();
            var success = await task;
            callback?.Invoke(success);
            return success;
        }
        #endregion
        #region internal
        private static bool TrySave(Type type)
        {
            if (!type.IsSerializable)
                throw new InvalidOperationException("A serializable Type is required");

            if (!dataDictionary.TryGetValue(type, out DictionaryItem instance))
                return false;

            IDirtyData objectToSave = instance.item;

            if (!objectToSave.IsDirty)
                return true;

            var data = (GetJson(objectToSave));
            var filePath = GetFilePath(type);
            if (Directory.Exists(DirectoryPath))
                return Save();
            else
                if (CreateDirectory())
                return Save();
            return false;

            bool Save()
            {
                lock (WriteReadLock)
                {
                    try
                    {
                        File.WriteAllText(filePath, data);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message);
                        return false;
                    }
                    objectToSave.Saved();
                    Debug.Log("Saved");
                    return true;
                }
            }
            bool CreateDirectory()
            {
                lock (WriteReadLock)
                {
                    try
                    {
                        Directory.CreateDirectory(DirectoryPath);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message);
                        return false;
                    }
                    return Directory.Exists(DirectoryPath);
                }
            }
        }
        private static bool TryLoad<T>(out T objectToLoad) where T : class, IDirtyData, new()
        {
            objectToLoad = default;
            string filePath = GetFilePath(typeof(T));

            if (!FileExists(typeof(T)))
                return false;

            string json = "";
            lock (WriteReadLock)
                try
                {
                    json = File.ReadAllText(filePath);
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                }

            if (json == "")
                return false;

            if (JsonParser.TryParseJson(json, out objectToLoad))
                return true;

            if (objectToLoad == null)
                return false;

            return true;
        }
        #endregion
    }
    #endregion
    #region Data Interface
    /// <summary>
    /// A data interface which implements the IsDirt flag design pattern.
    /// <a href="https://gpgroup13.wordpress.com/a-dirty-flag-tutorial/#consider">(Refrence)</a>
    /// Make sure to run the ValueChanged function on every property change.
    /// </summary>
    public interface IDirtyData
    {
        bool IsDirty { get; }
        void Saved();
        void ValueChanged();
    }
    public abstract class DirtyData : IDirtyData
    {
        protected bool _isDirty;
        public virtual bool IsDirty => _isDirty;

        public virtual void Saved() => _isDirty = false;
        public virtual void ValueChanged() => _isDirty = true;
    }
    #endregion

    #region Json Parser
    public static class JsonParser
    {
        public static bool TryParseJson<T>(string json, out T jsonObject)
        {
            jsonObject = default;
            if (json.Length < 1 || json == "")
                return false;
            try
            {
                jsonObject = JsonUtility.FromJson<T>(json);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }
            if (jsonObject == null)
                return false;
            return true;
        }
    }
    #endregion
}