using System;
using Plugins.Tools;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Managers
{
    public struct CameraLimits
    {
        public float minimum, maximum;
        public CameraLimits(float minimum, float maximum)
        {
            this.minimum = minimum;
            this.maximum = maximum;
        }

        public void SetMinimum(float minim) => minimum = minim;

        public void SetMaximum(float maxim) => maximum = maxim;
    }
    public class CameraManager : Singleton<CameraManager>
    {
        public Camera mainCamera;

        [Header("Rotations")]
        public float animationRotationSpeed;
        public Vector3 thirdDimensionRotation, secondDimensionRotation;

        [Header("Positions")]
        public float animationMoveSpeed;
        public Vector3 thirdDimensionPosition, secondDimensionPosition;

        public bool CanDoPanning { get; set; }

        public static bool In2D { get; private set; } = true;

        private Coroutine m_RotatingCoroutine, m_MovementCoroutine;

        private Vector3 lastPanPosition;
        private bool m_IsPanning;

#if UNITY_ANDROID || UNITY_IPHONE
        private int panFingerId; // Touch mode only

        private bool wasZoomingLastFrame;    // Touch mode only
        private Vector2[] lastZoomPositions; // Touch mode only

        private const float ZOOM_SPEED_TOUCH = 0.1f;
#else
        private const float ZOOM_SPEED_MOUSE = 10f;
#endif
        private const float PAN_SPEED = 20f;

        //first value is minim second is maxim
        //For each second 4, cells
        public static CameraLimits boundsY2d = new CameraLimits { minimum = -5f, maximum = 18f },
                                   boundsY3d = new CameraLimits { minimum = -10f, maximum = 10f };

        private static readonly CameraLimits BOUNDS_X = new CameraLimits { minimum = -5f, maximum = 5f };

        private static readonly CameraLimits ZOOM_BOUNDS_2D = new CameraLimits { minimum = 2.5f, maximum = 10f },
                                             ZOOM_BOUNDS_3D = new CameraLimits { minimum = 10f, maximum = 85f };

        protected override void Awake()
        {
            base.Awake();
            if (!mainCamera) mainCamera = Camera.main;
            CanDoPanning = true;
        }

        private void Update()
        {
            if (!CanDoPanning) return;

#if UNITY_ANDROID || UNITY_IPHONE
            HandleTouch();
#else
            HandleMouse();
#endif
        }

        public void InvertView()
        {
            if (In2D) See3DView();
            else See2DView();
        }

        public void See2DView()
        {
            RotateCameraTowards(secondDimensionRotation, () =>
            {
                mainCamera.orthographic = true;
                mainCamera.orthographicSize = 5f;
            });
            MoveCameraTowards(secondDimensionPosition);

            In2D = true;
        }

        public void See3DView()
        {
            mainCamera.orthographic = false;

            RotateCameraTowards(thirdDimensionRotation);
            MoveCameraTowards(thirdDimensionPosition);

            In2D = false;
        }

        private void MoveCameraTowards(Vector3 target)
        {
            if (m_MovementCoroutine != null) StopCoroutine(m_MovementCoroutine);

            Transform cameraTransform = mainCamera.transform;
            m_MovementCoroutine = StartCoroutine(UtilityMethods.FunctionCycleCoroutine
                                                 (
                                                     () => !cameraTransform.position.Approximates(target, 0.1f),
                                                     () => cameraTransform.position = Vector3.Lerp(cameraTransform.position, target, animationMoveSpeed * Time.deltaTime)
                                                 ));
        }

        private void RotateCameraTowards(Vector3 target, Action onEnd = null)
        {
            if (m_RotatingCoroutine != null) StopCoroutine(m_RotatingCoroutine);

            Transform cameraTransform = mainCamera.transform;
            Quaternion targetRotation = Quaternion.Euler(target);
            m_RotatingCoroutine = StartCoroutine(UtilityMethods.FunctionCycleCoroutine
                                                 (
                                                     () => cameraTransform.rotation != targetRotation,
                                                     () => cameraTransform.rotation =
                                                         Quaternion.RotateTowards(cameraTransform.rotation, targetRotation, animationRotationSpeed * Time.deltaTime),
                                                     null,
                                                     null,
                                                     onEnd
                                                 ));
        }

#if UNITY_ANDROID || UNITY_IPHONE
        private void HandleTouch()
        {
            switch (Input.touchCount)
            {
                case 1: // Panning
                    wasZoomingLastFrame = false;

                    // If the touch began, capture its position and its finger ID.
                    // Otherwise, if the finger ID of the touch doesn't match, skip it.
                    Touch touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Began)
                    {
                        if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) return;
                        lastPanPosition = touch.position;
                        panFingerId = touch.fingerId;
                        m_IsPanning = true;
                    }
                    else if (m_IsPanning && touch.fingerId == panFingerId && touch.phase == TouchPhase.Moved)
                    {
                        PanCamera(touch.position);
                    }
                    break;

                case 2: // Zooming
                    Vector2[] newPositions = { Input.GetTouch(0).position, Input.GetTouch(1).position };
                    if (!wasZoomingLastFrame)
                    {
                        lastZoomPositions = newPositions;
                        wasZoomingLastFrame = true;
                    }
                    else
                    {
                        // Zoom based on the distance between the new positions compared to the 
                        // distance between the previous positions.
                        float newDistance = Vector2.Distance(newPositions[0], newPositions[1]);
                        float oldDistance = Vector2.Distance(lastZoomPositions[0], lastZoomPositions[1]);
                        float offset = newDistance - oldDistance;

                        ZoomCamera(offset, ZOOM_SPEED_TOUCH);

                        lastZoomPositions = newPositions;
                    }
                    break;

                default:
                    wasZoomingLastFrame = false;
                    m_IsPanning = false;
                    break;
            }
        }
#else
        private void HandleMouse()
        {
            // On mouse down, capture it's position.
            // Otherwise, if the mouse is still down, pan the camera.
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject()) return;
                lastPanPosition = Input.mousePosition;
                m_IsPanning = true;
            }
            else if (Input.GetMouseButton(0) && m_IsPanning) PanCamera(Input.mousePosition);
            else if (Input.GetMouseButtonUp(0)) m_IsPanning = false;

            // Check for scrolling to zoom the camera
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            ZoomCamera(scroll, ZOOM_SPEED_MOUSE);
        }
#endif

        private void PanCamera(Vector3 newPanPosition)
        {
            // Determine how much to move the camera
            Vector3 offset = mainCamera.ScreenToViewportPoint(lastPanPosition - newPanPosition);
            var move = new Vector3(offset.x * PAN_SPEED, offset.y * PAN_SPEED, 0f);

            // Perform the movement
            Transform movedTransform;
            (movedTransform = transform).Translate(move, Space.World);

            // Ensure the camera remains within bounds.
            Vector3 position = movedTransform.position;

            position.x = Mathf.Clamp(position.x, BOUNDS_X.minimum, BOUNDS_X.maximum);
            position.y = In2D ?
                Mathf.Clamp(position.y, boundsY2d.minimum, boundsY2d.maximum) :
                Mathf.Clamp(position.y, boundsY3d.minimum, boundsY3d.maximum);

            transform.position = position;

            // Cache the position
            lastPanPosition = newPanPosition;
        }

        private void ZoomCamera(float offset, float speed)
        {
            if (offset == 0) return;

            if (In2D) mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize - offset * speed, ZOOM_BOUNDS_2D.minimum, ZOOM_BOUNDS_2D.maximum);
            else mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView - offset * speed, ZOOM_BOUNDS_3D.minimum, ZOOM_BOUNDS_3D.maximum);
        }
    }
}
