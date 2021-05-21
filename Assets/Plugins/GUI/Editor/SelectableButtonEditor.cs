using UnityEditor;
using UnityEditor.UI;

namespace Plugins.GUI.Editor
{
    [CustomEditor(typeof(SelectableButton)), CanEditMultipleObjects]
    public class SelectableButtonEditor : ButtonEditor
    {
        private SerializedProperty m_OnSelect, m_OnDeselect;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_OnSelect = serializedObject.FindProperty("onSelect");
            m_OnDeselect = serializedObject.FindProperty("onDeselect");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_OnSelect);
            EditorGUILayout.PropertyField(m_OnDeselect);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
