using System.Collections.Generic;
using UnityEngine;

namespace Pospec
{
    public class LineGear : MonoBehaviour
    {
        public float angularSpeed;
        public float radius;
        public List<LineGear> children;
        public ConnectionType connectionToParent;

        public void UpdateAngularSpeed(LineGear parent)
        {
            if (parent == null)
                return;

            switch (connectionToParent)
            {
                case ConnectionType.Stick:
                    angularSpeed = parent.angularSpeed;
                    break;
                case ConnectionType.Line:
                    angularSpeed = parent.radius * parent.angularSpeed / radius;
                    break;
                case ConnectionType.Teeth:
                    angularSpeed = -parent.radius * parent.angularSpeed / radius;
                    break;
                default:
                    break;
            }

            foreach (var child in children)
                child.UpdateAngularSpeed(this);
        }

        private void LateUpdate()
        {
            transform.Rotate(Vector3.forward * angularSpeed * DiscreteTime.instance.DeltaTime);
        }
    }

    public enum ConnectionType { Stick, Line, Teeth };
}
