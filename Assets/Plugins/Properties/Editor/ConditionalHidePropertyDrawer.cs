using UnityEditor;
using UnityEngine;

namespace Plugins.Properties.Editor
{
    [CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
    public class ConditionalHidePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var condHAtt = (ConditionalHideAttribute)attribute;
            bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

            bool wasEnabled =UnityEngine.GUI.enabled;
           UnityEngine.GUI.enabled = enabled;
            if (!condHAtt.HideInInspector || enabled)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }

           UnityEngine.GUI.enabled = wasEnabled;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var condHAtt = (ConditionalHideAttribute)attribute;
            bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

            if (!condHAtt.HideInInspector || enabled)
            {
                return EditorGUI.GetPropertyHeight(property, label);
            }
            return -EditorGUIUtility.standardVerticalSpacing;
        }

        private bool GetConditionalHideAttributeResult(ConditionalHideAttribute condHAtt, SerializedProperty property)
        {
            var enabled = true;
            string propertyPath = property.propertyPath; //returns the property path of the property we want to apply the attribute to
            string conditionPath = propertyPath.Replace(property.name, condHAtt.ConditionalSourceField); //changes the path to the conditionalsource property path
            SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

            if (sourcePropertyValue != null)
            {
                enabled = sourcePropertyValue.boolValue;
            }
            else
            {
                Debug.LogWarning("Attempting to use a ConditionalHideAttribute but no matching SourcePropertyValue found in object: " + condHAtt.ConditionalSourceField);
            }

            return enabled;
        }
    }
}
