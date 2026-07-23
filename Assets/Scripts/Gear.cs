using NUnit.Framework;
using UnityEngine;

namespace Pospec
{
    public class Gear : MonoBehaviour
    {
        public float radius;
        public float angularSpeed;

        public float surfaceSpeed() => radius * angularSpeed;

        public void LateUpdate()
        {
            transform.Rotate(Vector3.forward * angularSpeed * Time.deltaTime);
        }
    }
}
