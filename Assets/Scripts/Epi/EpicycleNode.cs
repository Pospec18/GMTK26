using UnityEngine;

namespace Pospec
{
    public class EpicycleNode : MonoBehaviour
    {
        [Header("Rotation Settings")]
        [Tooltip("Rychlost rotace tohoto kloubu ve stupních za sekundu")]
        public float rotationSpeed = 90f;

        void Update()
        {
            // Fyzicky otáčíme tento konkrétní objekt (a s ním se otočí i všechna jeho vnořená 'dětí')
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
    }
}