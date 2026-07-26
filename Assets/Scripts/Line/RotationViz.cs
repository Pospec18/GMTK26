using UnityEngine;

namespace Pospec
{
    public class RotationViz : MonoBehaviour
    {
        public float angularSpeed;

        public void Update()
        {
            // spin smoothly instead of in DiscreteTime's per-cycle pulses
            var dt = DiscreteTime.instance;
            transform.Rotate(Vector3.forward * angularSpeed * Time.deltaTime * dt.timeSpeed * dt.timeStep);
        }
    }
}
