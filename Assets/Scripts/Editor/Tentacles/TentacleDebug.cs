using UnityEditor;
using UnityEngine;

namespace Starblast.Tentacles
{
    public class TentacleDebug
    {
        public SerializedProperty debugOutline, debugTriangles, debugUVs;
        
        private TentacleProperties _properties;
        
        public TentacleDebug(SerializedObject serializedObject, TentacleProperties properties)
        {
            debugOutline = serializedObject.FindProperty("debugOutline");
            debugTriangles = serializedObject.FindProperty("debugTriangles");
            debugUVs = serializedObject.FindProperty("debugUVs");
            
            _properties = properties;
        }

        public void DrawControls()
        {
            EditorGUILayout.PropertyField(debugOutline, new GUIContent("Mesh outline", "Will highlight the mesh."));
            EditorGUILayout.PropertyField(debugTriangles, new GUIContent("Final mesh", "Show triangles and outline used to build the mesh."));
            EditorGUILayout.PropertyField(debugUVs, new GUIContent("Draw UVs", "Show UVs created for the mesh (at the center of the scene)."));
            DrawHierarchyType();
        }

        private void DrawHierarchyType()
        {
            var haveDifferentValues = false;
            var hideFlags = _properties.tentacleData[0].HierarchyType;
            if (hideFlags.Item2)
                haveDifferentValues = true;
            else
                for (int i = 1; i < _properties.tentacleData.Length; i++)
                    if (hideFlags.Item1 != _properties.tentacleData[i].HierarchyType.Item1)
                    {
                        haveDifferentValues = true;
                        break;
                    }

            if (haveDifferentValues) EditorGUI.showMixedValue = true;
            EditorGUI.BeginChangeCheck();

            var value = EditorGUILayout.Toggle(new GUIContent("Show Segments", "Show childs of this tentacle."),
                hideFlags.Item1);

            if (EditorGUI.EndChangeCheck())
                for (int i = 0; i < _properties.tentacleData.Length; i++)
                    _properties.tentacleData[i].ChangeHiererachy(value);
            if (haveDifferentValues) EditorGUI.showMixedValue = false;
        }
    }
}