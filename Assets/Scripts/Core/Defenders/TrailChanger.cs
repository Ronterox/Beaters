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

        private void Start() => ChangeTrail(trailDirection);

        public void ChangeDirection(bool isRight)
        {
            DataManager.playerData.tapsDone++;
            switch (trailDirection)
            {
                case Direction.up:
                    ChangeTrail(isRight ? Direction.right : Direction.left);
                    break;
                case Direction.down:
                    ChangeTrail(isRight ? Direction.left : Direction.right);
                    break;
                case Direction.left:
                    ChangeTrail(isRight ? Direction.up : Direction.down);
                    break;
                case Direction.right:
                    ChangeTrail(isRight ? Direction.down : Direction.up);
                    break;
            }
        }

        public void ChangeTrail(Direction direction)
        {
            upTrail.SetActive(direction == Direction.up);
            downTrail.SetActive(direction == Direction.down);
            leftTrail.SetActive(direction == Direction.left);
            rightTrail.SetActive(direction == Direction.right);

            trailDirection = direction;
        }
    }

}
