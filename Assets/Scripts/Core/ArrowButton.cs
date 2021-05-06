using DG.Tweening;
using Managers;
using Plugins.Tools;
using UnityEngine;

namespace Core
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

        public static int arrowTouches;
        private const int TOUCHES_LIMIT = 2;

        public delegate void ButtonEvent();

        public event ButtonEvent onButtonPress;

        private void Awake()
        {
            if (!mainCamera) mainCamera = Camera.main;
        }

        private void Start() => m_DefaultScale = transform.localScale;

        private void OnEnable() => onButtonPress += CheckButton;

        private void OnDisable() => onButtonPress -= CheckButton;

        public void PressButton() => onButtonPress?.Invoke();


#if UNITY_ANDROID || UNITY_IPHONE
        private void Update()
        {
            if (Input.touchCount < 1 || arrowTouches == TOUCHES_LIMIT) return;

            foreach (Touch touch in Input.touches)
            {
                if (arrowTouches == TOUCHES_LIMIT) return;
                Vector3 touchPos = touch.position;

                //Set the position relative to the camera vision
                touchPos.z = Mathf.Abs(mainCamera.transform.position.z);

                touchPos = mainCamera.ScreenToWorldPoint(touchPos);

                touchPos.x = Mathf.RoundToInt(touchPos.x);
                touchPos.y = Mathf.RoundToInt(touchPos.y);

                if (touchPos.Approximates(transform.position, 1f))
                {
                    onButtonPress?.Invoke();
                    arrowTouches++;
                }
            }
        }

        private void LateUpdate()
        {
            if (Input.touchCount < 1) arrowTouches = 0;
        }
#else
        private void OnMouseDown() => onButtonPress?.Invoke();
#endif

        private void CheckButton()
        {
            if (!isNoteAbove) GameManager.Instance.MissArrow();
            //Arrow animation with tween
            transform.DOScale(targetScale, animationDuration).OnComplete(() => transform.DOScale(m_DefaultScale, animationDuration));
        }
    }
}
