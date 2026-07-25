using UnityEngine;
using Unity.Cinemachine; // Pokud máš starší Cinemachine, použij jen: using Cinemachine;

namespace Pospec
{
    [ExecuteInEditMode]
    public class CameraFitBounds : MonoBehaviour
    {
        [Header("Target Area Settings")]
        [Tooltip("Šířka tvojí herní plochy ve světových jednotkách (units)")]
        public float targetWidth = 16f;

        [Tooltip("Výška tvojí herní plochy ve světových jednotkách (units)")]
        public float targetHeight = 9f;

        private CinemachineCamera m_Vcam; // Ve starším Cinemachine: CinemachineVirtualCamera

        void Awake()
        {
            m_Vcam = GetComponent<CinemachineCamera>();
        }

        void Update()
        {
            if (m_Vcam == null) return;

            // Vypočítáme požadovanou velikost na základě aktuálního poměru stran monitoru
            float screenAspect = (float)Screen.width / (float)Screen.height;
            float targetAspect = targetWidth / targetHeight;

            if (screenAspect >= targetAspect)
            {
                // Obrazovka je širší než hra -> držíme výšku
                m_Vcam.Lens.OrthographicSize = targetHeight / 2f;
            }
            else
            {
                // Obrazovka je užší než hra -> přizpůsobíme podle šířky, aby se strany neořízly
                float differenceInSize = targetAspect / screenAspect;
                m_Vcam.Lens.OrthographicSize = (targetHeight / 2f) * differenceInSize;
            }
        }
    }
}