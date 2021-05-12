using UnityEngine;

namespace Utilities
{
    [ExecuteInEditMode]
    public class Rotation : MonoBehaviour
    {
        public Vector3 rotateAxis = new Vector3(1, 5, 10);
        public float speed;
#if UNITY_EDITOR
        public bool alwaysRotate;
#endif

        private void Update()
        {
#if UNITY_EDITOR
            if(!Application.isPlaying && !alwaysRotate) return;
#endif
            transform.Rotate(rotateAxis * (speed * Time.deltaTime));
        }
    }
}
