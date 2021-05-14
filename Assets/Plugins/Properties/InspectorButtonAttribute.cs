using System.Reflection;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace Plugins.Properties
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class InspectorButtonAttribute : PropertyAttribute
    {
        public readonly string MethodName;

        public InspectorButtonAttribute(string MethodName) => this.MethodName = MethodName;
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(InspectorButtonAttribute))]
    public class InspectorButtonPropertyDrawer : PropertyDrawer
    {
        private MethodInfo _eventMethodInfo;

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            var inspectorButtonAttribute = (InspectorButtonAttribute)attribute;

            float buttonLength = 50 + inspectorButtonAttribute.MethodName.Length * 6;
            var buttonRect = new Rect(position.x + (position.width - buttonLength) * 0.5f, position.y, buttonLength, position.height);

            if (!UnityEngine.GUI.Button(buttonRect, inspectorButtonAttribute.MethodName)) return;
            
            System.Type eventOwnerType = prop.serializedObject.targetObject.GetType();
            string eventName = inspectorButtonAttribute.MethodName;

            if (_eventMethodInfo == null)
            {
                _eventMethodInfo = eventOwnerType.GetMethod(eventName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            }

            if (_eventMethodInfo != null)
            {
                _eventMethodInfo.Invoke(prop.serializedObject.targetObject, null);
            }
            else
            {
                Debug.LogWarning($"InspectorButton: Unable to find method {eventName} in {eventOwnerType}");
            }
        }
    }
#endif
}