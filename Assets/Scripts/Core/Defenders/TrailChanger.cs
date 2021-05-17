using Managers;
using UnityEngine;

namespace Core.Defenders
{
    public class TrailChanger : MonoBehaviour
    {
        [System.Serializable]
        public struct Trail
        {
            public Collider2D cubeCollider;
            public GameObject trail;

            public void SetActive(bool setActive = true)
            {
                trail.SetActive(setActive);
                cubeCollider.enabled = setActive;
            }
        }
        public Trail upTrail, downTrail, leftTrail, rightTrail;
        public Direction trailDirection;

        private PlayerData m_Data;

        private void Start()
        {
            m_Data = DataManager.Instance.playerData;
            ChangeTrail(trailDirection);
        }

        public void ChangeDirection(bool isRight)
        {
            m_Data.tapsDone++;
            switch (trailDirection)
            {
                case Direction.Up:
                    ChangeTrail(isRight ? Direction.Right : Direction.Left);
                    break;
                case Direction.Down:
                    ChangeTrail(isRight ? Direction.Left : Direction.Right);
                    break;
                case Direction.Left:
                    ChangeTrail(isRight ? Direction.Up : Direction.Down);
                    break;
                case Direction.Right:
                    ChangeTrail(isRight ? Direction.Down : Direction.Up);
                    break;
            }
        }

        public void ChangeTrail(Direction direction)
        {
            upTrail.SetActive(direction == Direction.Up);
            downTrail.SetActive(direction == Direction.Down);
            leftTrail.SetActive(direction == Direction.Left);
            rightTrail.SetActive(direction == Direction.Right);

            trailDirection = direction;
        }
    }

}
