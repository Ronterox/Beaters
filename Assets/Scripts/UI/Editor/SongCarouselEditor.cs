using Plugins.UI.Editor;
using UnityEditor;

namespace UI.Editor
{
    [CustomEditor(typeof(SongCarousel)), CanEditMultipleObjects]
    public class SongCarrouselEditor : UICarouselEditor
    {
        private SerializedProperty m_SongScriptables, m_DefaultSongs, m_RecordScreen, m_LockedScreen;

        protected new void OnEnable()
        {
            base.OnEnable();
            m_SongScriptables = serializedObject.FindProperty("songs");
            m_RecordScreen = serializedObject.FindProperty("songRecordScreen");
            m_LockedScreen = serializedObject.FindProperty("lockedSongScreen");
            m_DefaultSongs = serializedObject.FindProperty("defaultSongs");
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_SongScriptables);
            EditorGUILayout.PropertyField(m_DefaultSongs);
            EditorGUILayout.PropertyField(m_RecordScreen);
            EditorGUILayout.PropertyField(m_LockedScreen);
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
            m_SongImage = serializedObject.FindProperty("lockedSongImage");
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
