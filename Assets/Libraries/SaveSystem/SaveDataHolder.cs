using System;

namespace Pospec.Saving
{
    internal interface ISaveDataHoldable
    {
        ref object GetData();
        void Replace<DT>(DT newData);
        void SlotChanged();
    }

    internal class SaveDataHolder<T> : ISaveDataHoldable
    {
        private object data;
        private readonly GroupType groupType;

        public SaveDataHolder(T data, GroupType groupType)
        {
            this.data = data;
            this.groupType = groupType;
        }

        public ref object GetData()
        {
            return ref data;
        }

        public void Replace<DT>(DT newData)
        {
            throw new ArgumentException();
        }

        public void Replace(T newData)
        {
            data = newData;
        }

        public void SlotChanged()
        {
            if (groupType == GroupType.Shared)
                return;

        }
    }

    internal enum GroupType { Slot, Shared };
}
