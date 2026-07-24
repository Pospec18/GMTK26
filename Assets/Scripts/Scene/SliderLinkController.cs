using UnityEngine;
using UnityEngine.UI;

namespace Pospec
{
    public class SliderLinkController : MonoBehaviour
    {
        public Slider slider;
        public Link link;

        private void Start()
        {
            slider.minValue = 0;
            slider.maxValue = 1;
        }

        public void Update()
        {
            link.value = slider.value;
        }
    }
}
