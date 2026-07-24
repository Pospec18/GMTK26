using System;
using UnityEngine;

namespace Pospec
{
    [Serializable]
    public class EpicycleLayer
    {
        public float radius = 1f;
        public float speed = 90f;
        public float initialAngle = 0f;

        [Header("Visual Representation")]
        public Transform jointVisual;
    }

    [RequireComponent(typeof(LineRenderer))]
    public class EpicycleMachine : MonoBehaviour
    {
        [Header("Machine Configuration")]
        public EpicycleLayer[] layers;

        [Header("Drawing Element")]
        public Transform drawingPen;

        private LineRenderer m_LineRenderer;
        private float m_CurrentTime = 0f;

        void Start()
        {
            m_LineRenderer = GetComponent<LineRenderer>();
            m_LineRenderer.positionCount = layers.Length + 1;
        }

        void Update()
        {
            m_CurrentTime += Time.deltaTime;
            CalculateAndDraw();
        }

        private void CalculateAndDraw()
        {
            Vector3 currentPos = transform.position;
            m_LineRenderer.SetPosition(0, currentPos);

            // This tracks the total rotation inherited from parent layers
            float accumulatedAngle = 0f;

            for (int i = 0; i < layers.Length; i++)
            {
                // Add this layer's rotation to the total mechanical rotation
                accumulatedAngle += layers[i].initialAngle + (layers[i].speed * m_CurrentTime);

                // Place the visual joint at the START of this arm and rotate it
                if (layers[i].jointVisual != null)
                {
                    layers[i].jointVisual.position = currentPos;
                    layers[i].jointVisual.rotation = Quaternion.Euler(0f, 0f, accumulatedAngle);
                }

                float rad = accumulatedAngle * Mathf.Deg2Rad;

                // Calculate the offset for the end of this arm
                float x = Mathf.Cos(rad) * layers[i].radius;
                float y = Mathf.Sin(rad) * layers[i].radius;

                // Move the current position to the end of the arm
                currentPos += new Vector3(x, y, 0f);
                m_LineRenderer.SetPosition(i + 1, currentPos);
            }

            // Finally, place the drawing pen at the very end of the machine
            if (drawingPen != null)
            {
                drawingPen.position = currentPos;
            }
        }
    }
}