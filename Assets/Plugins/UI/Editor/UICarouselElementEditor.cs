using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace Plugins.UI.Editor
{
    /// <summary>
    /// Create your own custom editor, base it on this one
    /// </summary>
    [CustomEditor(typeof(UICarouselElement)), CanEditMultipleObjects]
    public class UICarouselElementEditor : ButtonEditor
    {
        private SerializedProperty m_SelectAnim;
        private SerializedProperty m_DeselectAnim;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_SelectAnim = serializedObject.FindProperty("selectAnim");
            m_DeselectAnim = serializedObject.FindProperty("deselectAnim");
        }

        public override void OnInspectorGUI()
        {
            var header = new GUIStyle(EditorStyles.boldLabel) { fontSize = 14 };

            GUILayout.Space(5);
            GUILayout.Label("Button Behaviour", header);
            base.OnInspectorGUI();

            GUILayout.Label("Animations", header);
            EditorGUILayout.PropertyField(m_SelectAnim, new GUIContent("Select Animation"));
            EditorGUILayout.PropertyField(m_DeselectAnim, new GUIContent("Deselect Animation"));

            serializedObject.ApplyModifiedProperties();
        }
    } 
}
