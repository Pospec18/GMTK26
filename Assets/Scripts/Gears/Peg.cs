using System.Collections.Generic;
using UnityEngine;

namespace Pospec
{
    /// <summary>
    /// A fixed "stick" in 2D space. Snaps to a 0.25 grid in the editor and holds
    /// cogs (its <see cref="Cog"/> children), stacking them concentrically.
    /// </summary>
    [ExecuteAlways]
    public class Peg : MonoBehaviour
    {
        public const float GridSize = 0.25f;

        [Tooltip("Z step between stacked cogs so they render in a stable order.")]
        public float cogStackZ = -0.01f;

        [Header("Optional peg visual")]
        [Tooltip("If set, drives the SpriteRenderer below. Leave empty for no peg art.")]
        public Sprite sprite;
        [Tooltip("SpriteRenderer to draw the peg. Auto-found on this object if left empty.")]
        public SpriteRenderer spriteRenderer;

        private void Awake() => ApplyVisual();
        private void OnValidate() => ApplyVisual();

        private void ApplyVisual()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && sprite != null)
                spriteRenderer.sprite = sprite;
        }

        /// <summary>Parents a cog to this peg (keeping its world scale) and re-stacks.</summary>
        public void AddCog(Cog cog)
        {
            cog.transform.SetParent(transform, true);
            ArrangeCogs();
        }

        /// <summary>All cogs stacked on this peg (its Cog children).</summary>
        public List<Cog> GetCogs()
        {
            var list = new List<Cog>();
            foreach (Transform child in transform)
            {
                var cog = child.GetComponent<Cog>();
                if (cog != null)
                    list.Add(cog);
            }
            return list;
        }

        private void Update()
        {
#if UNITY_EDITOR
            // Editor-only authoring: keep the peg on the grid and its cogs centered.
            if (!Application.isPlaying)
            {
                SnapToGrid();
                ArrangeCogs();
            }
#endif
        }

        private void SnapToGrid()
        {
            Vector3 p = transform.position;
            float x = Mathf.Round(p.x / GridSize) * GridSize;
            float y = Mathf.Round(p.y / GridSize) * GridSize;
            if (!Mathf.Approximately(x, p.x) || !Mathf.Approximately(y, p.y))
                transform.position = new Vector3(x, y, p.z);
        }

        public void ArrangeCogs()
        {
            int order = 0;
            foreach (Transform child in transform)
            {
                if (child.GetComponent<Cog>() == null)
                    continue;
                // order + 1 so even the first cog sits in front of the peg sprite (z = 0).
                child.localPosition = new Vector3(0f, 0f, (order + 1) * cogStackZ);
                order++;
            }
        }

        private void OnDrawGizmos()
        {
            // The peg itself.
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 0.05f);

            // Each cog's reach, so meshing can be eyeballed while placing.
            Gizmos.color = new Color(1f, 1f, 1f, 0.4f);
            foreach (Transform child in transform)
            {
                var cog = child.GetComponent<Cog>();
                if (cog != null)
                    DrawCircle(transform.position, cog.Radius);
            }
        }

        private static void DrawCircle(Vector3 center, float radius, int segments = 48)
        {
            Vector3 prev = center + new Vector3(radius, 0f, 0f);
            for (int i = 1; i <= segments; i++)
            {
                float a = i / (float)segments * Mathf.PI * 2f;
                Vector3 next = center + new Vector3(Mathf.Cos(a) * radius, Mathf.Sin(a) * radius, 0f);
                Gizmos.DrawLine(prev, next);
                prev = next;
            }
        }
    }
}
