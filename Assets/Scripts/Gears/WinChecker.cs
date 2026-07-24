using UnityEngine;

namespace Pospec
{
    public class WinChecker : MonoBehaviour
    {
        private int columnsCount;
        [HideInInspector] public int winColumns;

        public static WinChecker instance;

        private void Awake()
        {
            instance = this;
        }

        private void OnDestroy()
        {
            instance = null;
        }

        private void Start()
        {
            columnsCount = 0;
            foreach (Column col in FindObjectsByType<Column>(FindObjectsSortMode.None))
                if (col.link != null)
                    columnsCount++;
        }

        private void LateUpdate()
        {
            if (winColumns == columnsCount)
            {
                Debug.Log("WIN");
            }
            winColumns = 0;
        }
    }
}
