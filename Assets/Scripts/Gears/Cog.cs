using UnityEngine;

namespace Pospec
{
    public enum CogSize { Small = 0, Medium = 1, Large = 2, Huge = 3 }

    /// <summary>
    /// A single cog placed on a <see cref="Peg"/>. Has one of 4 discrete sizes.
    /// The radii are chosen so every pairwise sum lands on the 0.25 grid, which
    /// means two axis-aligned pegs can always be bridged by some pair of cogs.
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class Cog : MonoBehaviour
    {
        // Small+Small = 0.5, ... Huge+Huge = 2.0 — all multiples of the 0.25 grid.
        public static readonly float[] Radii = { 0.25f, 0.5f, 0.75f, 1.0f };

        public CogSize size = CogSize.Small;

        public float Radius => Radii[(int)size];

        private SpriteRenderer sr;

        private void Awake() => Apply();
        private void OnValidate() => Apply();

        /// <summary>Tints the cog sprite (used to flag jams / reset to white).</summary>
        public void SetColor(Color color)
        {
            if (sr == null)
                sr = GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.color = color;
        }

        /// <summary>Scales the sprite so its rendered radius matches <see cref="Radius"/>.</summary>
        private void Apply()
        {
            if (sr == null)
                sr = GetComponent<SpriteRenderer>();
            if (sr == null || sr.sprite == null)
                return;

            float spriteRadius = Mathf.Max(sr.sprite.bounds.extents.x, 0.0001f);
            float scale = Radius / spriteRadius;
            transform.localScale = new Vector3(scale, scale, 1f);

            // Local radius * transform scale == Radius in world, so the clickable
            // area always matches the drawn cog.
            var col = GetComponent<CircleCollider2D>();
            if (col != null)
                col.radius = spriteRadius;
        }
    }
}
