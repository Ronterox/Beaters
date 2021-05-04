using Plugins.Tools;
using UnityEngine;

namespace Managers
{
    public class CameraManager : MonoBehaviour
    {
        public Camera mainCamera;

        [Header("Rotations")]
        public float rotationSpeed;
        public Vector3 thirdDimensionRotation, secondDimensionRotation;

        [Header("Positions")]
        public float movementSpeed;
        public Vector3 thirdDimensionPosition, secondDimensionPosition;

        private bool m_In2D = true;

        private Coroutine m_RotatingCoroutine, m_MovementCoroutine;

        private void Awake()
        {
            if (!mainCamera) mainCamera = Camera.main;
        }

        public void InvertView()
        {
            if (m_In2D) See3DView();
            else See2DView();
        }

        public void See2DView()
        {
            mainCamera.orthographic = true;

            RotateCameraTowards(secondDimensionRotation);
            MoveCameraTowards(secondDimensionPosition);

            m_In2D = true;
        }

        public void See3DView()
        {
            mainCamera.orthographic = false;

            RotateCameraTowards(thirdDimensionRotation);
            MoveCameraTowards(thirdDimensionPosition);

            m_In2D = false;
        }

        private void MoveCameraTowards(Vector3 target)
        {
            if (m_MovementCoroutine != null) StopCoroutine(m_MovementCoroutine);

            Transform cameraTransform = mainCamera.transform;
            m_MovementCoroutine = StartCoroutine(UtilityMethods.FunctionCycleCoroutine
                                                 (
                                                     () => cameraTransform.position != target,
                                                     () => cameraTransform.position = Vector3.Lerp(cameraTransform.position, target, movementSpeed * Time.deltaTime)
                                                 ));
        }

        private void RotateCameraTowards(Vector3 target)
        {
            if (m_RotatingCoroutine != null) StopCoroutine(m_RotatingCoroutine);

            Transform cameraTransform = mainCamera.transform;
            Quaternion targetRotation = Quaternion.Euler(target);
            m_RotatingCoroutine = StartCoroutine(UtilityMethods.FunctionCycleCoroutine
                                                 (
                                                     () => cameraTransform.rotation != targetRotation,
                                                     () => cameraTransform.rotation =
                                                         Quaternion.RotateTowards(cameraTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime)
                                                 ));
        }
    }
}
