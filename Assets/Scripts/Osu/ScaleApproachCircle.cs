using DG.Tweening;
using UnityEngine;

namespace Osu
{
    public class ScaleApproachCircle : MonoBehaviour
    {
        public Vector3 scale;
        public float durationScale;

        private void Start() => transform.DOScale(scale, durationScale);
    }
}
