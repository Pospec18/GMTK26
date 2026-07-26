using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pospec
{
    public class LineCanvas : MonoBehaviour
    {
        public Slider slider;
        public Image image;
        public LineButton ropePrefab;
        public Transform buttonsPanel;
        private List<LineButton> ropeButtons = new List<LineButton>();

        public Color normalLineCol = Color.white;
        public Color errorLineCol = Color.red;
        List<float> lines;

        public int currId;
        public float LineLength() => slider.maxValue;

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
            image.color = length < slider.maxValue ? normalLineCol : errorLineCol;
        }

        public void HideLine()
        {
            buttonsPanel.gameObject.SetActive(true);
            slider.gameObject.SetActive(false);
        }

        private void StartLineDraw(int i)
        {
            currId = i;
            buttonsPanel.gameObject.SetActive(false);
            slider.gameObject.SetActive(true);
            slider.maxValue = lines[i];
            Grid.Instance.lineDrawing = true;
        }

        public void UseRope()
        {
            ropeButtons[currId].IsInteractible = false;
        }

        public void RemoveLine(int lineID)
        {
            ropeButtons[currId].IsInteractible = true;
        }
    }
}
