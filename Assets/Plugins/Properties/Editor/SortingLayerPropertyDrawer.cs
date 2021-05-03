using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Plugins.Properties.Editor
{
    [CustomPropertyDrawer(typeof(SortingLayerAttribute))]
    public class SortingLayerPropertyDrawer : PropertyDrawer
    {
        /**
     * Is called to draw a property.
     *
     * @param position Rectangle on the screen to use for the property GUI.
     * @param property The SerializedProperty to make the custom GUI for.
     * @param label The label of the property.
     */
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Integer)
            {
                // Integer is expected. Everything else is ignored.
                return;
            }
            EditorGUI.LabelField(position, label);

            position.x += EditorGUIUtility.labelWidth;
            position.width -= EditorGUIUtility.labelWidth;

            string[] sortingLayerNames = GetSortingLayerNames();
            int[] sortingLayerIDs = GetSortingLayerIDs();

            int sortingLayerIndex = Mathf.Max(0, System.Array.IndexOf(sortingLayerIDs, property.intValue));
            sortingLayerIndex = EditorGUI.Popup(position, sortingLayerIndex, sortingLayerNames);
            property.intValue = sortingLayerIDs[sortingLayerIndex];
        }

        /**
         * Retrives list of sorting layer names.
         *
         * @return List of sorting layer names.
         */
        private string[] GetSortingLayerNames()
        {
            System.Type internalEditorUtilityType = typeof(InternalEditorUtility);
            PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
            return (string[])sortingLayersProperty.GetValue(null, new object[0]);
        }

        /**
         * Retrives list of sorting layer identifiers.
         *
         * @return List of sorting layer identifiers.
         */
        private int[] GetSortingLayerIDs()
        {
            System.Type internalEditorUtilityType = typeof(InternalEditorUtility);
            PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic);
            return (int[])sortingLayersProperty.GetValue(null, new object[0]);
        }
    }
}

