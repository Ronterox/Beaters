using UnityEngine;

namespace Utilities
{
    public enum Direction : byte { None, North, NorthEast, NorthWest, South, SouthEast, SouthWest, East, West }

    public class SwipeDetector : MonoBehaviour
    {
        public float swipeThreshold = 50f, timeThreshold = 0.3f;
        public LineRenderer lineRenderer;

        private Vector2 m_FingerDown, m_FingerUp;
        private float m_FingerDownTime, m_FingerUpTime;

        public delegate void SwipeEvent(Direction direction);

        public event SwipeEvent onSwipe;

        private void Update()
        {
#if !UNITY_ANDROID && !UNITY_IPHONE
            if (Input.GetMouseButtonDown(0))
            {
                m_FingerDown = m_FingerUp = Input.mousePosition;
                m_FingerDownTime = Time.time;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                m_FingerDown = Input.mousePosition;
                m_FingerUpTime = Time.time;
                CheckSwipe();
            }
#else
            foreach (Touch touch in Input.touches)
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        m_FingerDown = touch.position;
                        m_FingerUp = touch.position;
                        m_FingerDownTime = Time.time;
                        break;
                    case TouchPhase.Ended:
                        m_FingerDown = touch.position;
                        m_FingerUpTime = Time.time;
                        CheckSwipe();
                        break;
                }
            }
#endif
        }

        private void CheckSwipe()
        {
            float duration = m_FingerUpTime - m_FingerDownTime;
            if (duration > timeThreshold) return;

            Direction horizontalDirection = Direction.None, verticalDirection = Direction.None;

            float deltaX = m_FingerDown.x - m_FingerUp.x;
            if (Mathf.Abs(deltaX) > swipeThreshold)
            {
                if (deltaX > 0) horizontalDirection = Direction.East;
                else if (deltaX < 0) horizontalDirection = Direction.West;
            }

            float deltaY = m_FingerDown.y - m_FingerUp.y;
            if (Mathf.Abs(deltaY) > swipeThreshold)
            {
                if (deltaY > 0) verticalDirection = Direction.North;
                else if (deltaY < 0) verticalDirection = Direction.South;
            }
            
            Direction swipeDirection = MergeDirections(horizontalDirection, verticalDirection);
            if (swipeDirection != Direction.None)
            {
                ShowSwipeFeedback();
                onSwipe?.Invoke(swipeDirection);
            }

            m_FingerUp = m_FingerDown;
        }

        private void ShowSwipeFeedback()
        {
            lineRenderer.SetPosition(0, m_FingerDown);
            lineRenderer.SetPosition(1, m_FingerUp);
        }

        /// <summary>
        /// Merges 2 directions, Example: north and east, becomes NorthEast
        /// </summary>
        /// <param name="horizontal"></param>
        /// <param name="vertical"></param>
        /// <returns></returns>
        private static Direction MergeDirections(Direction horizontal, Direction vertical) =>
            vertical switch
            {
                Direction.North => horizontal switch
                {
                    Direction.East => Direction.NorthEast,
                    Direction.West => Direction.NorthWest,
                    _ => Direction.North
                },
                Direction.South => horizontal switch
                {
                    Direction.East => Direction.SouthEast,
                    Direction.West => Direction.SouthWest,
                    _ => Direction.South
                },
                _ => horizontal
            };
    }
}
