using System.Collections.Generic;
using UnityEngine;

namespace Pospec
{
    /// <summary>
    /// Runs a cog rig: detects which cogs mesh, spins them from a driver, and
    /// checks whether the designated output pegs all "run the same".
    ///
    /// Model: each peg is a shaft with one angular speed shared by all its cogs.
    /// Two cogs on different pegs mesh when the distance between their pegs equals
    /// the sum of their radii; at a mesh the contact speeds match and direction
    /// flips, so  omega_other = -omega_cur * rCur / rOther.
    /// </summary>
    public class Machine : MonoBehaviour
    {
        [Header("Drive")]
        public Peg driver;
        public float driveSpeed = 90f; // degrees / second

        // Unity's +Z rotation is counter-clockwise, so speed > 0 = CCW, < 0 = CW.
        public enum Spin { Any, Clockwise, CounterClockwise }

        [System.Serializable]
        public class Output
        {
            public Peg peg;
            [Tooltip("Required spin of this output. Any = don't care about direction.")]
            public Spin requiredSpin = Spin.Any;
            [Tooltip("On: must reach Target Speed. Off: must match the other untargeted outputs.")]
            public bool useTarget;
            [Tooltip("Required speed in deg/s (sign = direction). Used only when Use Target is on.")]
            public float targetSpeed;
        }

        [Header("Win condition")]
        [Tooltip("Targeted outputs must hit their target; untargeted outputs must all run the same.")]
        public List<Output> outputs = new List<Output>();

        [Header("Test controls")]
        public KeyCode runKey = KeyCode.Space;
        public KeyCode resetKey = KeyCode.R;

        [Header("Tuning")]
        [Tooltip("Slop (world units) around the ideal touching distance. Small gaps or slight overlaps within this still mesh.")]
        public float meshTolerance = 0.03f;
        [Tooltip("How deep cogs may overlap and still mesh, as a fraction of the SMALLER cog's radius. " +
                 "0 = must just touch. ~0.6 lets diagonally-adjacent small cogs mesh.")]
        [Range(0f, 1f)]
        public float maxOverlap = 0.6f;

        [Header("Direction colors")]
        public Color counterClockwiseColor = new Color(0.3f, 0.6f, 1f);
        public Color clockwiseColor = Color.red;
        public Color idleColor = Color.white;

        public bool Running { get; private set; }
        public bool Jammed { get; private set; }
        public bool Solved { get; private set; }

        private struct Contact { public Peg a, b; public Cog ca, cb; }

        private readonly List<Peg> pegs = new List<Peg>();
        private readonly List<Contact> contacts = new List<Contact>();
        private readonly Dictionary<Peg, float> speeds = new Dictionary<Peg, float>();
        private readonly Dictionary<Peg, Quaternion> initialRot = new Dictionary<Peg, Quaternion>();

        private void Update()
        {
            if (Input.GetKeyDown(runKey)) Run();
            if (Input.GetKeyDown(resetKey)) ResetRig();

            if (Running)
            {
                foreach (var kv in speeds)
                    if (kv.Key != null)
                        kv.Key.transform.Rotate(0f, 0f, kv.Value * Time.deltaTime);
            }
        }

        /// <summary>Detect meshing, propagate rotation from the driver, evaluate the win.</summary>
        public void Run()
        {
            Collect();
            BuildContacts();
            Solve();
            Running = !Jammed;
            Debug.Log($"Run: {pegs.Count} pegs, {contacts.Count} meshes, jammed={Jammed}, solved={Solved}");
        }

        /// <summary>Stop spinning and restore every peg to its authored rotation.</summary>
        public void ResetRig()
        {
            Running = false;
            Solved = false;
            foreach (var kv in initialRot)
                if (kv.Key != null)
                    kv.Key.transform.localRotation = kv.Value;

            foreach (var peg in pegs)
                if (peg != null)
                    foreach (var cog in peg.GetCogs())
                        cog.SetColor(idleColor);
        }

        private void Collect()
        {
            pegs.Clear();
            pegs.AddRange(FindObjectsByType<Peg>(FindObjectsSortMode.None));
            foreach (var p in pegs)
            {
                if (!initialRot.ContainsKey(p))
                    initialRot[p] = p.transform.localRotation;
                foreach (var cog in p.GetCogs())
                    cog.SetColor(Color.white);
            }
        }

        private void BuildContacts()
        {
            contacts.Clear();
            for (int i = 0; i < pegs.Count; i++)
            for (int j = i + 1; j < pegs.Count; j++)
            {
                float dist = Vector2.Distance(pegs[i].transform.position, pegs[j].transform.position);
                foreach (var ca in pegs[i].GetCogs())
                foreach (var cb in pegs[j].GetCogs())
                {
                    float sum = ca.Radius + cb.Radius;
                    float overlap = sum - dist;          // >0 overlapping, <0 a gap
                    float allowed = maxOverlap * Mathf.Min(ca.Radius, cb.Radius);
                    // Mesh when they roughly touch: a small gap (meshTolerance) or an
                    // overlap up to `allowed` deep. Deeper than that = not meant to mesh.
                    bool mesh = overlap >= -meshTolerance && overlap <= allowed + meshTolerance;

                    if (mesh)
                        contacts.Add(new Contact { a = pegs[i], b = pegs[j], ca = ca, cb = cb });
                }
            }
        }

        private void Solve()
        {
            speeds.Clear();
            Jammed = false;
            Solved = false;
            if (driver == null)
            {
                Debug.LogWarning("Machine: no driver assigned.");
                return;
            }

            speeds[driver] = driveSpeed;
            var queue = new Queue<Peg>();
            queue.Enqueue(driver);

            while (queue.Count > 0)
            {
                Peg cur = queue.Dequeue();
                float w = speeds[cur];
                foreach (var c in contacts)
                {
                    Peg other;
                    float rCur, rOther;
                    if (c.a == cur) { other = c.b; rCur = c.ca.Radius; rOther = c.cb.Radius; }
                    else if (c.b == cur) { other = c.a; rCur = c.cb.Radius; rOther = c.ca.Radius; }
                    else continue;

                    float candidate = -w * rCur / rOther;
                    if (speeds.TryGetValue(other, out float existing))
                    {
                        if (!Approx(existing, candidate))
                            Jam(c);
                    }
                    else
                    {
                        speeds[other] = candidate;
                        queue.Enqueue(other);
                    }
                }
            }

            if (!Jammed)
            {
                ColorByDirection();
                EvaluateWin();
            }
        }

        private void ColorByDirection()
        {
            foreach (var peg in pegs)
            {
                if (peg == null)
                    continue;

                Color c = idleColor;
                if (speeds.TryGetValue(peg, out float s))
                {
                    if (s > 0f) c = counterClockwiseColor;
                    else if (s < 0f) c = clockwiseColor;
                }

                foreach (var cog in peg.GetCogs())
                    cog.SetColor(c);
            }
        }

        private void EvaluateWin()
        {
            if (outputs == null || outputs.Count == 0)
                return; // no win condition set — just spinning for a look

            bool solved = true;
            float matchSpeed = 0f;
            bool haveUntargeted = false;

            foreach (var o in outputs)
            {
                if (o == null || o.peg == null || !speeds.TryGetValue(o.peg, out float s))
                {
                    Debug.Log("Not solved: an output peg isn't being driven.");
                    solved = false;
                    continue;
                }

                if (o.requiredSpin != Spin.Any)
                {
                    bool ccw = s > 0f;
                    bool wantCcw = o.requiredSpin == Spin.CounterClockwise;
                    if (s == 0f || ccw != wantCcw)
                    {
                        Debug.Log($"Not solved: output spins {(s > 0f ? "CCW" : s < 0f ? "CW" : "not at all")}, needs {o.requiredSpin}.");
                        solved = false;
                    }
                }

                if (o.useTarget)
                {
                    if (!Approx(s, o.targetSpeed))
                    {
                        Debug.Log($"Not solved: output off target ({s:0.##} vs {o.targetSpeed:0.##} deg/s).");
                        solved = false;
                    }
                }
                else if (!haveUntargeted)
                {
                    matchSpeed = s;
                    haveUntargeted = true;
                }
                else if (!Approx(matchSpeed, s))
                {
                    Debug.Log($"Not solved: outputs run differently ({matchSpeed:0.##} vs {s:0.##} deg/s).");
                    solved = false;
                }
            }

            Solved = solved;
            if (Solved)
                Debug.Log("Scene complete!");
        }

        private void Jam(Contact c)
        {
            Jammed = true;
            c.ca.SetColor(Color.red);
            c.cb.SetColor(Color.red);
            Debug.Log("JAM: two cogs demand conflicting speeds on the same shaft.");
        }

        // Relative comparison so it scales with speed magnitude.
        private static bool Approx(float a, float b)
        {
            return Mathf.Abs(a - b) <= 0.01f * Mathf.Max(Mathf.Abs(a), Mathf.Abs(b), 1f);
        }
    }
}
