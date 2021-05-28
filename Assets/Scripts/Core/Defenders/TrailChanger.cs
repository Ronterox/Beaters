using Managers;
using UnityEngine;
using Utilities;

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
                case Direction.North:
                    ChangeTrail(isRight ? Direction.East : Direction.West);
                    break;
                case Direction.South:
                    ChangeTrail(isRight ? Direction.West : Direction.East);
                    break;
                case Direction.West:
                    ChangeTrail(isRight ? Direction.North : Direction.South);
                    break;
                case Direction.East:
                    ChangeTrail(isRight ? Direction.South : Direction.North);
                    break;
            }
        }

        public void ChangeTrail(Direction direction)
        {
            upTrail.SetActive(direction == Direction.North);
            downTrail.SetActive(direction == Direction.South);
            leftTrail.SetActive(direction == Direction.West);
            rightTrail.SetActive(direction == Direction.East);

            trailDirection = direction;
        }
    }

}
