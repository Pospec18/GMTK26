using UnityEngine;
using Unity.Cinemachine; // V novějším Unity jen: using Cinemachine;

namespace Pospec
{
    // Automaticky přidá zdroj impulsu
    [RequireComponent(typeof(CinemachineImpulseSource))]
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance { get; private set; }

        public enum ShakeType { Mini, Mid, Max }

        private CinemachineImpulseSource m_ImpulseSource;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            m_ImpulseSource = GetComponent<CinemachineImpulseSource>();
        }

        public void Shake(ShakeType type)
        {
            // Výchozí síla a doba trvání
            float force = 1f;

            switch (type)
            {
                case ShakeType.Mini:
                    force = 0.2f;
                    break;
                case ShakeType.Mid:
                    force = 0.6f;
                    break;
                case ShakeType.Max:
                    force = 1.2f;
                    break;
            }

            // Vygeneruje otřes v náhodném směru o dané síle
            Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f).normalized;
            m_ImpulseSource.GenerateImpulse(randomDirection * force);
        }
    }
}