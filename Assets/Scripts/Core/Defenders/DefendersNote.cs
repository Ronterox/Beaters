using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;

namespace Core.Defenders{
    public enum Direction{
            up, down, right, left
        }    
    public class DefendersNote : MonoBehaviour
    {
        
        public Direction direction;
        public ushort id;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            GameManager.Instance.HitArrow();
            gameObject.SetActive(false);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            GameManager.Instance.MissArrow();
            gameObject.SetActive(false);
        }
    }
}
