using UnityEngine;
using Managers;

namespace Core.Defenders
{
    public enum Direction { up, down, right, left }

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
                GameplayManager.HitArrow();
                gameObject.SetActive(false);
            }
        }
    }
}
