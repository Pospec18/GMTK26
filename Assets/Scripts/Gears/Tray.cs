using UnityEngine;

namespace Pospec
{
    /// <summary>
    /// A rectangular area cogs can be dropped into (the player's spare-cog shelf).
    /// Cogs simply rest wherever they're dropped inside it.
    /// </summary>
    public class Tray : MonoBehaviour
    {
        [Tooltip("Size of the drop area (in the tray's local space), centered on this object.")]
        public Vector2 size = new Vector2(4f, 2f);

        public bool Contains(Vector2 worldPoint)
        {
            // Compare in local space so rotation/scale are respected.
            Vector3 local = transform.InverseTransformPoint(worldPoint);
            Vector2 half = size * 0.5f;
            return Mathf.Abs(local.x) <= half.x && Mathf.Abs(local.y) <= half.y;
        }

        public void Place(Cog cog, Vector3 worldPoint)
        {
            cog.transform.SetParent(transform, true);
            cog.transform.position = new Vector3(worldPoint.x, worldPoint.y, transform.position.z);
        }

        private void OnDrawGizmos()
        {
            // Draw in the tray's local space so the box sits exactly on the object.
            Gizmos.matrix = transform.localToWorldMatrix;
            var box = new Vector3(size.x, size.y, 0.01f);

            Gizmos.color = new Color(0.3f, 0.6f, 1f, 0.15f);
            Gizmos.DrawCube(Vector3.zero, box);
            Gizmos.color = new Color(0.3f, 0.6f, 1f, 0.9f);
            Gizmos.DrawWireCube(Vector3.zero, box);
        }
    }
}
