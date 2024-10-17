using UnityEditor;
using UnityEngine;

namespace Starblast.Utils
{
    [CustomPropertyDrawer(typeof(FloatRange))]
    public class FloatRangeDrawer  : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Begin the property, drawing the label (the name of the range field)
            EditorGUI.BeginProperty(position, label, property);

            // Draw the label on the left side
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Calculate the rects for Min and Max fields
            var halfWidth = position.width / 2;
            var minRect = new Rect(position.x, position.y, halfWidth - 5, position.height);
            var maxRect = new Rect(position.x + halfWidth + 10, position.y, halfWidth - 5, position.height);

            // Get the Min and Max properties
            SerializedProperty minProperty = property.FindPropertyRelative("_min");
            SerializedProperty maxProperty = property.FindPropertyRelative("_max");

            // Draw the Min and Max fields
            EditorGUI.PropertyField(minRect, minProperty, GUIContent.none);  // No label for the fields themselves
            EditorGUI.PropertyField(maxRect, maxProperty, GUIContent.none);

            // End the property
            EditorGUI.EndProperty();
        }    
    }
}