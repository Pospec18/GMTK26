using DG.Tweening;
using System;
using UnityEngine;

namespace Pospec
{
    public class DiscreteTime : MonoBehaviour
    {
        public static DiscreteTime instance;
        public float timeSpeed = 1;
        public float timeStep = 1;
        private float time;
        public Ease ease;
        public float DeltaTime { get; private set; }

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
            if (time > 0.8)
                DeltaTime = DOVirtual.EasedValue(0.0f, 1.0f, (time - 0.8f) * 5, ease) * 10 * Time.deltaTime * timeStep;

            if (time > 1.0f)
                time -= 1.0f;
        }

        public void Stop()
        {
            timeSpeed = 0;
        }
    }
}
