using UnityEngine;
using UnityEngine.UI;

namespace Pospec
{
    public class SliderTest : MonoBehaviour
    {
        public Link link;
        public Slider slider;

        private void Start()
        {
            slider.minValue = 0;
            slider.maxValue = 1;
        }

        private void LateUpdate()
        {
            slider.value = link.value;
        }
    }
}
