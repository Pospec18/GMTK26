using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pospec.Saving
{
    public static class SaveManager
    {
        private static SlotsData _slots = null;
        private static SlotsData Slots
        {
            get
            {
                if (_slots == null && !dataHandler.Load(SlotsData.key, slotMetadataGroupName, out _slots))
                    _slots = new SlotsData();
                return _slots;
            }
        }
        private static readonly IDataHandler dataHandler = new FileDataHandler();

        private const string sharedGroupName = "Shared";
        private const string slotMetadataGroupName = "Slots";
        private const string slotPrefixGroupName = "Slot";
        private static string GetName<T>(string key) => key;
        private static string GetSlotGroupName(int slot) => slotPrefixGroupName + slot;
        private static int GetSlotId(string group)
        {
            int i = group.LastIndexOf(slotPrefixGroupName);
            if (i == -1)
                return i;
            return int.Parse(group.Substring(i + slotPrefixGroupName.Length));
        }

        public static event Action DoLoad;
        public static event Action DoSave;

        /// <summary>
        /// Change active slot. You must reload all loaded data from slots after this.
        /// </summary>
        /// <param name="slotNumber">number of slot</param>
        public static void ChangeSaveSlot(int slotNumber)
        {
            Slots.ActiveSlot = slotNumber;
            dataHandler.Save(Slots, SlotsData.key, slotMetadataGroupName);
        }

        /// <summary>
        /// Save data which is meant to persist across multiple save slots or repeated plays
        /// </summary>
        /// <typeparam name="T">type of data</typeparam>
        /// <param name="data">data to be saved</param>
        /// <param name="key">unique identifier of the data</param>
        /// <returns></returns>
        public static bool SaveShared<T>(T data, string key) where T : SaveData
        {
            try
            {
                string name = GetName<T>(key);
                return dataHandler.Save(data, name, sharedGroupName);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        /// <summary>
        /// Save data valid for only one playthrough.
        /// Do this even for games without multiple save slots, this way you can restart your playthrough by calling ClearSlot.
        /// </summary>
        /// <typeparam name="T">type of data</typeparam>
        /// <param name="data">data to be saved</param>
        /// <param name="key">unique identifier of the data</param>
        /// <returns></returns>
        public static bool SaveToActiveSlot<T>(T data, string key) where T : SaveData
        {
            string name = GetName<T>(key);
            return dataHandler.Save(data, name, GetSlotGroupName(Slots.ActiveSlot));
        }

        /// <summary>
        /// Clear all data saved to specific slot.
        /// Can be also used for resetting game without multiple slots - you just need all your data in one slot.
        /// </summary>
        /// <param name="slot">slot to be cleared</param>
        public static void ClearSlot(int slot)
        {
            dataHandler.ClearGroup(GetSlotGroupName(slot));
        }

        public static int GetActiveSlot()
        {
            return Slots.ActiveSlot;
        }

        /// <summary>
        /// Loads saved data
        /// </summary>
        /// <typeparam name="T">type of data</typeparam>
        /// <param name="key">identifier of data</param>
        /// <param name="data">data to be loaded</param>
        /// <returns>loaded correctly</returns>
        public static bool Load<T>(string key, out T data) where T : SaveData
        {
            try
            {
                string name = GetName<T>(key);
                if (dataHandler.Load(name, GetSlotGroupName(Slots.ActiveSlot), out data))
                    return true;

                if (dataHandler.Load(name, sharedGroupName, out data))
                    return true;
                return false;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                data = default;
                return false;
            }
        }

        /// <summary>
        /// Loads data with same key from all saved slots.
        /// Perfect for showing information in slot selection.
        /// </summary>
        /// <typeparam name="T">type of data</typeparam>
        /// <param name="key">identifier of data</param>
        /// <param name="results"></param>
        public static void LoadFromAllSlots<T>(string key, out IList<T> results) where T : SaveData
        {
            var groups = dataHandler.GetAllGroups(slotPrefixGroupName + "*");
            results = new List<T>();
            foreach (var group in groups)
            {
                int slot = GetSlotId(group);
                while (results.Count <= slot)
                    results.Add(default);
                if (dataHandler.Load(key, group, out T data))
                    results[slot] = data;
            }
        }
    }

    [Serializable]
    internal class SlotsData : SaveData
    {
        public const string key = "Medatada";
        public int ActiveSlot = 0;

        public SlotsData() : base(1)
        {
        }
    }
}
