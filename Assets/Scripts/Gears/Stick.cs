using System.Collections.Generic;
using UnityEngine;

namespace Pospec
{
    public class Stick : MonoBehaviour
    {
        [HideInInspector] public float angularSpeed;
        [HideInInspector] public float distFromPrev;
        public List<Gear> gears;
        public float rotationNormalized() => transform.localEulerAngles.z / 360.0f;

        private List<Color> colors = new List<Color>()
        {
            Color.red, Color.green, Color.blue,
        };

        private void Start()
        {
            for (int i = 0; i < colors.Count; i++)
            {
                Color c = colors[i];
                c.a = 0.5f;
                colors[i] = c;
            }
        }

        public void UpdateStick(Stick prev)
        {
            transform.Rotate(Vector3.forward * angularSpeed * Time.deltaTime);
            if (prev != null)
                transform.localPosition = prev.transform.localPosition + Vector3.down * distFromPrev;
            for (int i = 0; i < gears.Count; i++)
                if (gears[i] != null)
                    gears[i].GetComponent<SpriteRenderer>().color = colors[i];
        }
    }
}
