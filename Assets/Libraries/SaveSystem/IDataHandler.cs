using System.Collections.Generic;

namespace Pospec.Saving
{
    internal interface IDataHandler
    {
        bool Save<T>(T data, string name, string group) where T : SaveData;
        bool Load<T>(string name, string group, out T data) where T : SaveData;
        void ClearGroup(string group);
        IEnumerable<string> GetAllGroups(string pattern = "");
    }
}
