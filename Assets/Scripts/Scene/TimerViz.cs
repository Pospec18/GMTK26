using UnityEngine;

namespace Pospec
{
    public class TimerViz : MonoBehaviour
    {
        public Material timerMat;
        public SpriteRenderer sr;
        public Link link;

        private void Start()
        {
            timerMat = new Material(timerMat);
            sr.material = timerMat;
            timerMat.SetFloat("_Clockwise", 1.0f);
        }

        private void Update()
        {
            timerMat.SetFloat("_FillAmount", link.value);
        }
    }
}
