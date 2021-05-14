using DG.Tweening;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace Plugins.UI.Editor
{
    /// <summary>
    /// Create your own custom editor, base it on this one
    /// </summary>
    [CustomEditor(typeof(UICarousel)), CanEditMultipleObjects]
    public class UICarouselEditor : ScrollRectEditor
    {
        private SerializedProperty m_ElementPrefab, m_ScrollMask;
        private bool m_SaveRuntime;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_ElementPrefab = serializedObject.FindProperty("elementPrefab");
            m_ScrollMask = serializedObject.FindProperty("scrollMask");
        }

        public override void OnInspectorGUI()
        {
            var header = new GUIStyle(EditorStyles.boldLabel) { fontSize = 14 };

            var carousel = target as UICarousel;

            GUILayout.Space(5);
            GUILayout.Label("Scroll Rect Behaviour", header);
            base.OnInspectorGUI();

            GUILayout.Label("Carousel", header);
            EditorGUILayout.PropertyField(m_ElementPrefab, new GUIContent("Element Prefab"));
            EditorGUILayout.PropertyField(m_ScrollMask, new GUIContent("Scroll Mask"));
            
            GUILayout.Space(5);
            m_SaveRuntime = EditorGUILayout.Toggle("Save changes on runtime", m_SaveRuntime);
            
            GUILayout.Space(5);
            GUILayout.Label("Animation", EditorStyles.boldLabel);
            if (carousel is { })
            {
                carousel.scrollSpeed = EditorGUILayout.FloatField("Scroll Speed", carousel.scrollSpeed);
                carousel.scrollEase = (Ease)EditorGUILayout.EnumPopup("Scroll Ease", carousel.scrollEase);

                carousel.scrollSpeed = Mathf.Max(carousel.scrollSpeed, 0.01f);
            }

            serializedObject.ApplyModifiedProperties();
        }

        protected override void OnDisable()
        {
            if (m_SaveRuntime)
            {
                var carousel = target as UICarousel;
                Debug.Log("CHANGED SAVED! of Carousel");
                Undo.RecordObject(carousel, "UICarousel changed");
            }

            base.OnDisable();
        }
    } 
}
