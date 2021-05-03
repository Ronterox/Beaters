﻿using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

#endif

namespace Plugins.Properties
{
    public class InformationAttribute : PropertyAttribute
    {
        public enum InformationType { Error, Info, None, Warning }

#if UNITY_EDITOR
        public string message;
        public MessageType type;
        public bool messageAfterProperty;

        public InformationAttribute(string message, InformationType type, bool messageAfterProperty)
        {
            this.message = message;
            this.type = type switch { InformationType.Error => UnityEditor.MessageType.Error, InformationType.Info => UnityEditor.MessageType.Info, InformationType.Warning => UnityEditor.MessageType.Warning, InformationType.None => UnityEditor.MessageType.None, _ => this.type };
            this.messageAfterProperty = messageAfterProperty;
        }
#else
		public InformationAttribute(string message, InformationType type, bool messageAfterProperty)
		{

		}
#endif
    }
}
