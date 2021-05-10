using System.Collections;
using System.Collections.Generic;
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
            if(other.CompareTag("NoteMisser")){
                GameManager.Instance.MissArrow();
                gameObject.SetActive(false);
            }else if (other.CompareTag("Player")){
                GameManager.Instance.HitArrow();
                gameObject.SetActive(false);
            }
            
        }

    }
}
