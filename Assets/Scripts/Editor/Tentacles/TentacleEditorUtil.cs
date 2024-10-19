using UnityEditor;
using UnityEngine;

namespace Starblast.Tentacles
{
    public static class TentacleEditorUtil
    {
        public static void BeginBoldLabels()
        {
            EditorStyles.label.fontStyle = FontStyle.Bold;
            EditorStyles.textField.fontStyle = FontStyle.Bold;
            EditorStyles.popup.fontStyle = FontStyle.Bold;
            EditorStyles.objectField.fontStyle = FontStyle.Bold;
        }

        public static void EndBoldLabels()
        {
            EditorStyles.label.fontStyle = FontStyle.Normal;
            EditorStyles.textField.fontStyle = FontStyle.Normal;
            EditorStyles.popup.fontStyle = FontStyle.Normal;
            EditorStyles.objectField.fontStyle = FontStyle.Normal;
            DrawPrefabModifiedStyle();
        }
        
        public static void DrawPrefabModifiedStyle()
        {
            var rect = GUILayoutUtility.GetLastRect();
            rect.width = 3f;
            rect.height += 2f;
            rect.position = new Vector2(rect.position.x - 15f, rect.position.y - 1f);
            EditorGUI.DrawRect(rect, new Color(52f / 255f, 166f / 255f, 228f / 255f));
        }
    }
}