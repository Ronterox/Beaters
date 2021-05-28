using DG.Tweening;
using Plugins.Tools;
using UnityEngine;

namespace UI
{
    [System.Serializable]
    public class LinearSpawner
    {
        public Vector2 direction = new Vector2(0, 1);

        [Range(0f, 1f)]
        public float buttonSpacing = .5f;
        public float buttonNumOffset;
    }

    public class ButtonBrancher : MonoBehaviour
    {
        public RectTransform[] buttonRefs;

        public float moveDuration = 1f;
        public bool revealOnStart;

        private bool m_Spawned;

        public LinearSpawner linSpawner = new LinearSpawner();

        private void Start()
        {
            if (revealOnStart) ShowHideButtons();
        }

        public void ShowHideButtons()
        {
            m_Spawned = !m_Spawned;

            if (m_Spawned) ShowButton();
            else HideButtons();
        }

        private void ShowButton()
        {
            Transform tform = transform;
            buttonRefs.ForEach(buttonRef => buttonRef.transform.position = tform.position);

            SetButtonRefsEnable(true);

            RevealLinearlyNormal();
        }

        private void HideButtons() => SetButtonRefsEnable(false);

        private void RevealLinearlyNormal()
        {
            Vector3 position = transform.position;
            Vector3 spawnerDirection = linSpawner.direction.normalized;

            for (var i = 0; i < buttonRefs.Length; i++)
            {
                RectTransform buttonRect = buttonRefs[i];
                Vector2 sizeDelta = buttonRect.sizeDelta;

                var targetPos = new Vector3
                {
                    x = spawnerDirection.x * ((i + linSpawner.buttonNumOffset) * (sizeDelta.x * linSpawner.buttonSpacing)) + position.x,
                    y = spawnerDirection.y * ((i + linSpawner.buttonNumOffset) * (sizeDelta.y * linSpawner.buttonSpacing)) + position.y,
                    z = 0
                };

                buttonRect.DOMove(targetPos, moveDuration);
            }
        }

        private void SetButtonRefsEnable(bool enable) => buttonRefs.ForEach(buttonRef => buttonRef.gameObject.SetActive(enable));
    }
}
