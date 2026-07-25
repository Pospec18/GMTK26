using UnityEngine;

namespace Pospec
{
    public class WinSegment : MonoBehaviour
    {
        public float winRange = 0.1f;

        public Follower follower;
        public SpriteRenderer sr;

        [HideInInspector] public Link link;

        private void Start()
        {
            Material m = sr.material;
            m = new Material(m);
            m.SetFloat("_FillAmount", winRange);
            m.SetFloat("_Clockwise", 0);
        }

        public void Update()
        {
            sr.color = Color.magenta;
            if (link.value < winRange)
            {
                WinChecker.instance.winColumns++;
                sr.color = Color.green;
            }
        }
    }
}
