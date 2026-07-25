using UnityEngine;

namespace Pospec
{
    public class RotationViz : MonoBehaviour
    {
        public float angularSpeed;

        public void Update()
        {
            transform.Rotate(Vector3.forward * angularSpeed * DiscreteTime.instance.DeltaTime);
        }
    }
}
