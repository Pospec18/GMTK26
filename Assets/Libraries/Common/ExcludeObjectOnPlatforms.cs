using UnityEngine;

namespace Pospec.Common
{
    public class ExcludeObjectOnPlatforms : MonoBehaviour
    {
        [SerializeField] private BuildPlatform excludeOnPlatforms;
        [SerializeField] private Object objectToDelete;

        private void OnValidate()
        {
            if (objectToDelete == null)
                objectToDelete = gameObject;
        }

        private void Awake()
        {
            if (excludeOnPlatforms.ContainsCurrentPlatform())
                Destroy(objectToDelete);
            Destroy(this);
        }
    }
}
