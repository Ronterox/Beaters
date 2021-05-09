using Plugins.UI.Editor;
using UnityEditor;

namespace UI.Editor
{
    [CustomEditor(typeof(SongCarousel)), CanEditMultipleObjects]
    public class SongCarrouselEditor : UICarouselEditor
    {
        private SerializedProperty m_SongScriptables;

        protected new void OnEnable()
        {
            base.OnEnable();
            m_SongScriptables = serializedObject.FindProperty("songs");
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_SongScriptables);
            serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomEditor(typeof(SongCarouselElement)), CanEditMultipleObjects]
    public class SongCarouselElementEditor : UICarouselElementEditor
    {
        private SerializedProperty m_SongImage, m_SongText;
        
        protected new void OnEnable()
        {
            base.OnEnable();
            m_SongImage = serializedObject.FindProperty("songImage");
            m_SongText = serializedObject.FindProperty("songName");
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_SongImage);
            EditorGUILayout.PropertyField(m_SongText);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
