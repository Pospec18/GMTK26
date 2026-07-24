using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Pospec
{
    public class WinChecker : MonoBehaviour
    {
        public List<Column> columns;

        public Image image;

        private const float winOffset = 0.05f;

        private void Start()
        {
            image.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        }

        private void Update()
        {
            int numColumns = columns.Count;
            int columnWins = 0;

            foreach (var column in columns)
            {
                if (column == null || column.link == null)
                {
                    continue;
                }

                float value = column.link.value;

                if (value > 1.0f - winOffset || value < winOffset)
                {
                    columnWins++;
                }
            }

            if (columnWins == numColumns)
            {
                image.color = Color.green;
            }
        }
    }
}
