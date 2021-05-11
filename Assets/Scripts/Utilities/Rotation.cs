using UnityEngine;

namespace Utilities
{
    [ExecuteInEditMode]
    public class Rotation : MonoBehaviour
    {
        public Vector3 rotateAxis = new Vector3(1, 5, 10);
        public float speed;

        private void Update() => transform.Rotate(rotateAxis * (speed * Time.deltaTime));
    }
}
