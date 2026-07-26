using System.Collections.Generic;
using UnityEngine;

namespace Pospec
{
    /// <summary>Lives on an empty object in the scene and keeps a direction arrow next to every
    /// gear: the clockwise one while the gear spins one way, the counter clockwise one while it
    /// spins the other, neither while it stands still.</summary>
    public class ArrowsGenerator : MonoBehaviour
    {
        public GameObject cwPrefab;
        public GameObject ccwPrefab;

        /// <summary>Where the arrow sits relative to a gear of referenceRadius, in world units.
        /// The Y part grows with the gear, so a big gear does not swallow its arrow.</summary>
        public Vector3 offset = new Vector3(0.0f, 0.6f, -0.5f);

        /// <summary>The gear radius the offset and the prefab scale were authored for.</summary>
        public float referenceRadius = 0.5f;

        /// <summary>How much of the gear's growth the arrow itself takes on: 0 keeps every arrow
        /// the authored size, 1 grows it in step with the gear. The offset always follows the
        /// gear in full, so a partly grown arrow still sits clear of the rim.</summary>
        [Range(0.0f, 1.0f)]
        public float sizeScaling = 0.4f;

        /// <summary>A gear this slow reads as standing still, so it gets no arrow.</summary>
        public float idleSpeed = 0.05f;

        /// <summary>How often the scene is searched for gears that appeared or went away.</summary>
        public float rescanInterval = 0.25f;

        private readonly Dictionary<LineGear, Arrows> arrows = new Dictionary<LineGear, Arrows>();
        private readonly List<LineGear> stale = new List<LineGear>();
        private float nextRescan;

        private struct Arrows
        {
            public GameObject cw;
            public GameObject ccw;
        }

        // after the gears moved and took their new speed for this frame
        private void LateUpdate()
        {
            if (Time.time >= nextRescan)
            {
                nextRescan = Time.time + rescanInterval;
                Rescan();
            }

            foreach (var pair in arrows)
            {
                LineGear gear = pair.Key;
                if (gear == null)
                    continue;

                float speed = gear.angularSpeed;
                bool idle = Mathf.Abs(speed) < idleSpeed;

                // the gear in our hand is not driven by anything, and the arrow would follow the
                // cursor around instead of pointing at a place on the grid
                if (gear.isDragging)
                    idle = true;

                pair.Value.cw.SetActive(!idle && speed < 0.0f);
                pair.Value.ccw.SetActive(!idle && speed > 0.0f);
            }
        }

        private void Rescan()
        {
            foreach (var gear in FindObjectsByType<LineGear>(FindObjectsSortMode.None))
            {
                if (!arrows.ContainsKey(gear))
                    arrows.Add(gear, Spawn(gear));
            }

            // gears destroyed with the level leave their arrows behind
            stale.Clear();
            foreach (var pair in arrows)
            {
                if (pair.Key == null)
                    stale.Add(pair.Key);
            }

            foreach (var gear in stale)
            {
                Destroy(arrows[gear].cw);
                Destroy(arrows[gear].ccw);
                arrows.Remove(gear);
            }
        }

        private Arrows Spawn(LineGear gear)
        {
            return new Arrows
            {
                cw = SpawnOne(cwPrefab, gear),
                ccw = SpawnOne(ccwPrefab, gear),
            };
        }

        // how much bigger this gear is than the one the prefab was authored against
        private float ScaleFor(LineGear gear)
        {
            return referenceRadius > 0.0f ? gear.radius / referenceRadius : 1.0f;
        }

        private GameObject SpawnOne(GameObject prefab, LineGear gear)
        {
            float scale = ScaleFor(gear);

            // only the vertical part of the offset scales: the arrow keeps its distance from the
            // rim as the gear grows, without drifting sideways or changing which layer it draws on
            Vector3 gearOffset = new Vector3(offset.x, offset.y * scale, offset.z);

            GameObject arrow = Instantiate(prefab, gear.transform.position + gearOffset, prefab.transform.rotation, transform);
            // the arrow grows slower than the gear does, so a huge gear does not come with a
            // banner next to it
            arrow.transform.localScale = prefab.transform.localScale * Mathf.Lerp(1.0f, scale, sizeScaling);

            // the arrow is not a child of the gear, so the gear's own spinning does not carry it
            // around - it only trails its position
            Follower follower = arrow.GetComponent<Follower>();
            if (follower)
            {
                follower.target = gear.transform;
                follower.offset = gearOffset;
            }

            arrow.SetActive(false);
            return arrow;
        }
    }
}
