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
                GameplayManager.MissArrow();
                gameObject.SetActive(false);
            }
            else if (other.CompareTag("Player"))
            {
                GameplayManager.HitArrow(HitType.Good, transform.position, Color.white);
                gameObject.SetActive(false);
            }
        }
    }
}
