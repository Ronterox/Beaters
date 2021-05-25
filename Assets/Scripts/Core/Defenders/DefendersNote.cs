using UnityEngine;
using Managers;

namespace Core.Defenders
{
    public enum Direction { Up, Down, Right, Left }

    public class DefendersNote : MonoBehaviour
    {
        public ushort id;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("NoteMisser"))
            {
                GameplayManager.Instance.MissArrow();
                gameObject.SetActive(false);
            }
            else if (other.CompareTag("Player"))
            {
                GameplayManager.Instance.HitArrow(HitType.Good, transform.position, Color.white);
                gameObject.SetActive(false);
            }
        }
    }
}
