using System.Collections.Generic;
using UnityEngine;

namespace Pospec.Saving
{
    /// <summary>
    /// Class marking ScriptableObject that can be saved.
    /// 
    /// All SOs need to be placed in the Resources folder and at the start of the game loaded using Resources.LoadAll.
    /// SO must be in OnEnable added to it's LoadedSSOMap and in OnDisable removed from LoadedSSOMap.
    /// 
    /// When saving SerializableScriptableObject, create string atribute marked with [SerializeField].
    /// Class containing SerializableScriptableObject must implement ISerializationCallbackReceiver.
    /// OnAfterDeserialize call `string = LoadedSSOMap.Serialize(item)`.
    /// OnAfterDeserialize call `item = LoadedSSOMap.Deserialize(string)`.
    /// </summary>
    public class SerializableScriptableObject : ScriptableObject
    {
        public string SerializationKey => name + "_" + GetType();
    }

    public static class LoadedSSOMap<T> where T : SerializableScriptableObject
    {
        public static Dictionary<string, T> loaded = new Dictionary<string, T>();

        public static void Add(T sso)
        {
            if (sso == null)
                return;

#if DEBUG
            if (loaded.ContainsKey(sso.SerializationKey))
                Debug.LogWarning($"Key '{sso.SerializationKey}' is already present in {nameof(LoadedSSOMap<T>)}", sso);
#endif
            loaded[sso.SerializationKey] = sso;
        }

        public static void Remove(T sso)
        {
            if (sso == null)
                return;

#if DEBUG
            if (!loaded.ContainsKey(sso.SerializationKey))
                Debug.LogWarning($"Key '{sso.SerializationKey}' is NOT present in {nameof(LoadedSSOMap<T>)}", sso);
#endif
            loaded.Remove(sso.SerializationKey);
        }

        public static string Serialize(T sso)
        {
            if (sso == null)
                return "";
            return sso.SerializationKey;
        }

        public static T Deserialize(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;
            return loaded[key];
        }
    }
}
