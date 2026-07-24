using UnityEngine;

namespace Pospec
{
    /// <summary>
    /// Lets the player pick up cogs with the mouse and drop them onto pegs (or back
    /// where they came from). Re-parenting is all that's needed — the <see cref="Machine"/>
    /// reads whatever cogs are on the pegs when it runs.
    /// </summary>
    public class CogDragger : MonoBehaviour
    {
        [Tooltip("How close (world units) a drop must be to a peg to snap onto it.")]
        public float snapDistance = 0.5f;

        [Tooltip("Z the held cog sits at while dragging, so it renders above the board.")]
        public float dragZ = -1f;

        private Camera cam;
        private Machine machine;

        private Cog held;
        private Transform returnParent;
        private Vector3 returnLocalPos;

        private void Awake()
        {
            cam = Camera.main;
            machine = FindFirstObjectByType<Machine>();
        }

        private void Update()
        {
            if (cam == null)
                cam = Camera.main;

            if (held == null)
            {
                if (Input.GetMouseButtonDown(0))
                    TryPickup();
                return;
            }

            // Dragging.
            Vector3 p = MouseWorld();
            held.transform.position = new Vector3(p.x, p.y, dragZ);

            if (Input.GetMouseButtonUp(0))
                Drop();
        }

        private void TryPickup()
        {
            Vector2 p = MouseWorld();
            Cog top = null;
            foreach (var col in Physics2D.OverlapPointAll(p))
            {
                var cog = col.GetComponent<Cog>();
                if (cog == null)
                    continue;
                // Prefer the cog rendered on top (nearest the camera = smallest z).
                if (top == null || cog.transform.position.z < top.transform.position.z)
                    top = cog;
            }
            if (top == null)
                return;

            // Editing stops the run and restores authored rotations.
            if (machine != null && machine.Running)
                machine.ResetRig();

            held = top;
            returnParent = held.transform.parent;
            returnLocalPos = held.transform.localPosition;

            held.transform.SetParent(null, true);

            // Re-stack the peg we just took it from.
            var fromPeg = returnParent != null ? returnParent.GetComponent<Peg>() : null;
            if (fromPeg != null)
                fromPeg.ArrangeCogs();
        }

        private void Drop()
        {
            Vector2 dropPoint = held.transform.position;

            Peg nearest = null;
            float best = snapDistance;
            foreach (var peg in FindObjectsByType<Peg>(FindObjectsSortMode.None))
            {
                float d = Vector2.Distance(dropPoint, peg.transform.position);
                if (d <= best)
                {
                    best = d;
                    nearest = peg;
                }
            }

            if (nearest != null)
            {
                nearest.AddCog(held);
            }
            else if (TrayAt(dropPoint) is Tray tray)
            {
                tray.Place(held, dropPoint);
            }
            else
            {
                // Not over a peg or a tray — return it to where it started.
                held.transform.SetParent(returnParent, true);
                held.transform.localPosition = returnLocalPos;
                var homePeg = returnParent != null ? returnParent.GetComponent<Peg>() : null;
                if (homePeg != null)
                    homePeg.ArrangeCogs();
            }

            held = null;
        }

        private Tray TrayAt(Vector2 point)
        {
            foreach (var tray in FindObjectsByType<Tray>(FindObjectsSortMode.None))
                if (tray.Contains(point))
                    return tray;
            return null;
        }

        private Vector3 MouseWorld()
        {
            Vector3 screen = Input.mousePosition;
            screen.z = -cam.transform.position.z; // distance to the z=0 board plane
            return cam.ScreenToWorldPoint(screen);
        }
    }
}
