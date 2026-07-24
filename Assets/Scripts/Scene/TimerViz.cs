using UnityEngine;

namespace Pospec
{
    public class TimerViz : MonoBehaviour
    {
        public Material timerMat;
        public SpriteRenderer sr;
        public Link link;
        public Follower follower;

        private void Start()
        {
            timerMat = new Material(timerMat);
            sr.material = timerMat;
            timerMat.SetFloat("_Clockwise", 0.0f);
        }

        private void Update()
        {
            SetValue(link.value);
        }

        public void SetValue(float value)
        {
            timerMat.SetFloat("_FillAmount", value);
        }
    }
}
