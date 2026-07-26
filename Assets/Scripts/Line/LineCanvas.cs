using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

namespace Pospec
{
    public class LineCanvas : MonoBehaviour
    {
        public Slider slider;
        public LineButton ropePrefab;
        public Transform buttonsPanel;
        private List<LineButton> ropeButtons;

        List<float> lines;

        public void Start()
        {
            HideLine();
        }

        public void Setup(List<float> lines)
        {
            this.lines = lines;
            for (int i = 0; i < lines.Count; i++)
            {
                var button = Instantiate(ropePrefab, buttonsPanel);
                int idx = i;
                button.OnClick(() => StartLineDraw(idx));
                ropeButtons.Add(button);
            }
        }

        public void SetLineSize(float length)
        {
            slider.value = length;
        }

        public void HideLine()
        {
            buttonsPanel.gameObject.SetActive(true);
            slider.gameObject.SetActive(false);
        }

        private void StartLineDraw(int i)
        {
            buttonsPanel.gameObject.SetActive(false);
            slider.gameObject.SetActive(true);
            slider.maxValue = lines[i];
            Grid.Instance.lineDrawing = true;
        }
    }
}
