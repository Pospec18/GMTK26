using UnityEngine;

namespace Pospec
{
    public class DragManager : MonoBehaviour
    {
        public Stick selectedStick;
        public bool stickIsDeselected;

        public static DragManager instance;

        private void Awake()
        {
            instance = this;
        }

        private void OnDestroy()
        {
            instance = null;
        }

        public Stick SelectedStick() => selectedStick;
        public Column FromColumn() => selectedStick == null ? null : selectedStick.column;

        public void SelectStick(Stick stick)
        {
            selectedStick = stick;
        }

        public void DeselectStick()
        {
            stickIsDeselected = true;
        }

        private void LateUpdate()
        {
            if (stickIsDeselected)
            {
                stickIsDeselected = false;
                selectedStick = null;
            }
        }
    }
}
