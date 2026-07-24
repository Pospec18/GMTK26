using UnityEngine;

namespace Pospec
{
    public class DiscreteTime : MonoBehaviour
    {
        public static DiscreteTime instance;
        public float timeSpeed = 1;
        public float timeStep = 1;
        private float time;
        public float DeltaTime;

        private void Awake()
        {
            instance = this;
        }

        private void OnDestroy()
        {
            instance = null;
        }

        public void Update()
        {
            time += Time.deltaTime * timeSpeed;
            DeltaTime = 0;
            if (time > 1)
            {
                DeltaTime = timeStep;
                time -= 1.0f;
            }
        }
    }
}
