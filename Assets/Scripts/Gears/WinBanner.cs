using UnityEngine;

namespace Pospec
{
    /// <summary>
    /// Shows a "you win" GameObject (any UI/sprite) while the machine is solved,
    /// and hides it otherwise. Drop this anywhere and wire the two fields.
    /// </summary>
    public class WinBanner : MonoBehaviour
    {
        [Tooltip("The machine to watch. Auto-found if left empty.")]
        public Machine machine;

        [Tooltip("The object to show when solved (e.g. a 'YOU WIN' panel). Starts hidden.")]
        public GameObject banner;

        private void Awake()
        {
            if (machine == null)
                machine = FindFirstObjectByType<Machine>();
            if (banner != null)
                banner.SetActive(false);
        }

        private void Update()
        {
            if (banner != null && machine != null)
                banner.SetActive(machine.Solved);
        }
    }
}
