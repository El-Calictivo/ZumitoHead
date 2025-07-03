using UnityEditor;
using UnityEngine;

namespace Payosky.Architecture.Editor.Attributes
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false; // disable editing
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true; // re-enable after drawing
        }
    }
}