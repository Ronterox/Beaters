using DG.Tweening;
using UnityEngine;

namespace Osu
{
    public class GreatBadMove : MonoBehaviour
    {
        public Vector3 endPosition;
        public float duration;

        private void Start()
        {
            transform.DOMove(endPosition, duration);
            Destroy(gameObject, duration - 0.5f);
        }
    }
}
