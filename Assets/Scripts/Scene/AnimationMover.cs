using UnityEngine;

namespace Pospec
{
    public class AnimationMover : MonoBehaviour
    {
        public AnimationClip clip;
        public Link link;

        public void Update()
        {
            float timeInSeconds = Mathf.Clamp01(link.value) * clip.length;
            clip.SampleAnimation(gameObject, timeInSeconds);
        }
    }
}
