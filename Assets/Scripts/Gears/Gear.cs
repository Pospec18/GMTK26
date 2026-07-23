using UnityEngine;

namespace Pospec
{
    public class Gear : MonoBehaviour
    {
        public float radius;
        public float angularSpeed;

        public float surfaceSpeed() => radius * angularSpeed;
        public float rotationNormalized() => transform.localEulerAngles.z / 360.0f;

        public void LateUpdate()
        {
            transform.Rotate(Vector3.forward * angularSpeed * Time.deltaTime);
        }
    }
}
