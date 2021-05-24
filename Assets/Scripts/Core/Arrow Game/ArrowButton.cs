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

        [Plugins.Properties.ReadOnly]
        public bool isNoteAbove;

        public bool canBeClick = true;

        private Tween m_ClickAnimation;
        private float buttonHeight;

#if UNITY_ANDROID || UNITY_IPHONE
        private const int TOUCH_MAX_DISTANCE = 2;
#endif
        public delegate void ButtonEvent(float buttonHeight);

        public event ButtonEvent onButtonPress;

        private void Awake()
        {
            if (!mainCamera) mainCamera = Camera.main;
            buttonHeight = transform.position.y;
        }

        private void Start() => m_ClickAnimation = transform.DOShakeScale(.5f, .5f,10,0).SetAutoKill(false);

        public void PressButton()
        {
            CheckButton();
            onButtonPress?.Invoke(buttonHeight);
        }


#if UNITY_ANDROID || UNITY_IPHONE
        //If slow and you need fps improvement you can reduce this code to be only on the CameraManager
        private void Update()
        {
            if (!canBeClick || Input.touchCount < 1) return;

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
        private void OnMouseDown()
        {
            if (!canBeClick) return;
            PressButton();
        }
#endif

        private void CheckButton()
        {
            if (!isNoteAbove) GameplayManager.Instance.MissArrowTap();
            //Arrow animation with tween
            m_ClickAnimation.Restart();
            m_ClickAnimation.Play();
        }
    }
}
