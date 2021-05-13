using UnityEngine;

namespace Osu
{
    public class CurvedLinePoint : MonoBehaviour
    {
#if UNITY_EDITOR
        [HideInInspector] public bool showGizmo = true;
        [HideInInspector] public float gizmoSize = 0.1f;
        [HideInInspector] public Color gizmoColor = new Color(1, 0, 0, 0.5f);

        private void OnDrawGizmos()
        {
            if (!showGizmo) return;

            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(transform.position, gizmoSize);
        }
#endif
    }
}
