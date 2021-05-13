using DG.Tweening;
using Managers;
#if UNITY_ANDROID || UNITY_IPHONE
using Plugins.Tools;
#endif
using UnityEngine;

namespace Core.Arrow_Game
{
    public class ArrowButton : MonoBehaviour
    {
        public Camera mainCamera;

        [Header("Animations")]
        public float animationDuration;
        public Vector3 targetScale;
        private Vector3 m_DefaultScale;

        [Plugins.Properties.ReadOnly]
        public bool isNoteAbove;

#if UNITY_ANDROID || UNITY_IPHONE
        private const int TOUCH_MAX_DISTANCE = 2;
#endif
        public delegate void ButtonEvent();

        public event ButtonEvent onButtonPress;

        private void Awake()
        {
            if (!mainCamera) mainCamera = Camera.main;
        }

        private void Start() => m_DefaultScale = transform.localScale;

        public void PressButton()
        {
            CheckButton();
            onButtonPress?.Invoke();
        }


#if UNITY_ANDROID || UNITY_IPHONE
        //If slow and you need fps improvement you can reduce this code to be only on the CameraManager
        private void Update()
        {
            if (Input.touchCount < 1) return;

            foreach (Touch touch in Input.touches)
            {
                Vector3 touchPos = touch.position;

                //Set the position relative to the camera vision
                touchPos.z = Mathf.Abs(mainCamera.transform.position.z);

                touchPos = mainCamera.ScreenToWorldPoint(touchPos);

                touchPos.x = Mathf.RoundToInt(touchPos.x);
                touchPos.y = Mathf.RoundToInt(touchPos.y);

                if (touchPos.Approximates(transform.position, TOUCH_MAX_DISTANCE))
                {
                    if (touch.phase == TouchPhase.Began) PressButton();
                }
            }
        }
#else
        private void OnMouseDown() => PressButton();
#endif

        private void CheckButton()
        {
            if (!isNoteAbove) GameplayManager.MissArrowTap();
            //Arrow animation with tween
            transform.DOScale(targetScale, animationDuration).OnComplete(() => transform.DOScale(m_DefaultScale, animationDuration));
        }
    }
}
