using DG.Tweening;
using UnityEngine;

namespace Pospec
{
    public class Mover : MonoBehaviour
    {
        public Vector3 from;
        public Vector3 to;
        public Ease ease;
        public Link link;

        public void Update()
        {
            transform.position = DOVirtual.EasedValue(from, to, link.value, ease);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(from, 0.1f);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(to, 0.1f);
        }
    }
}
