using System;
using UnityEditor;
using UnityEngine;

namespace Starblast.Tentacles
{
    
    
    public class TentacleRendering
    {
        private TentacleProperties _properties;
        private string[] sortingLayerNames;

        public TentacleRendering(SerializedObject serializedObject, TentacleProperties props)
        {
            _properties = props;
            InitializeSortingLayers();
        }

        public void DrawControls()
        {
            DrawColorField();
            DrawMaterialField();

            EditorGUILayout.PropertyField(_properties.textureType,
                new GUIContent("Draw Mode", "Choose the method to set the UVs."));
            EditorGUILayout.IntSlider(_properties.smoothness, 4, 128,
                new GUIContent("Smoothness", "The number of segments of a mesh."));
            EditorGUILayout.IntSlider(_properties.pivotCapSmoothness, 0, 64,
                new GUIContent("Pivot Cap Smoothness",
                    "Defines roundness of the mesh at the start of the tentacle. 0 = no cap."));
            EditorGUILayout.IntSlider(_properties.tipCapSmoothness, 0, 64,
                new GUIContent("Tip Cap Smoothness",
                    "Defines roundness of the mesh at the end of the tentacle. 0 = no cap."));


            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_properties.width,
                new GUIContent("Width", "The width multiplier of the tentacle's mesh."));
            if (EditorGUI.EndChangeCheck())
                for (int i = 0; i < _properties.tentacleData.Length; i++)
                    _properties.tentacleData[i].UpdateCircleColliders(_properties.width.floatValue,
                        _properties.shape.animationCurveValue);

            DrawLength();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_properties.shape,
                new GUIContent("Shape", "The shape of a mesh. To change width use Width field above."),
                GUILayout.Height(EditorGUIUtility.singleLineHeight * 2f));
            if (EditorGUI.EndChangeCheck())
                for (int i = 0; i < _properties.tentacleData.Length; i++)
                    _properties.tentacleData[i].UpdateCircleColliders(_properties.width.floatValue,
                        _properties.shape.animationCurveValue);


            DrawSortingLayers();
            DrawSortingOrder();
        }

        private void DrawColorField()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_properties.color, new GUIContent("Color", "Tint color of the mesh."));
            if (EditorGUI.EndChangeCheck())
                for (int i = 0; i < _properties.tentacleData.Length; i++)
                    _properties.tentacleData[i].SetColor(_properties.color.colorValue);
        }

        private void DrawMaterialField()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_properties.material,
                new GUIContent("Material", "Will render the mesh with the material applied."));
            if (EditorGUI.EndChangeCheck())
                for (int i = 0; i < _properties.tentacleData.Length; i++)
                    _properties.tentacleData[i].meshRenderer.material =
                        (Material)_properties.material.objectReferenceValue;
        }

        private void DrawSortingLayers()
        {
            var haveDifferentValues = false;
            var sortingLayerName = _properties.tentacleData[0].meshRenderer.sortingLayerName;
            for (int i = 1; i < _properties.tentacleData.Length; i++)
                if (sortingLayerName != _properties.tentacleData[i].meshRenderer.sortingLayerName)
                {
                    haveDifferentValues = true;
                    break;
                }

            if (haveDifferentValues) EditorGUI.showMixedValue = true;

            EditorGUI.BeginChangeCheck();

            int value;
            if (_properties.IsPropertyModified(typeof(MeshRenderer), "SortingLayer"))
            {
                TentacleEditorUtil.BeginBoldLabels();
                EditorStyles.popup.fontStyle = FontStyle.Bold;
                value = EditorGUILayout.Popup(new GUIContent("Sorting Layer", "Name of the renderer's sorting layer."),
                    haveDifferentValues ? 0 : Array.IndexOf(sortingLayerNames, sortingLayerName), sortingLayerNames);
                EditorStyles.popup.fontStyle = FontStyle.Normal;
                TentacleEditorUtil.EndBoldLabels();
            }
            else
            {
                value = EditorGUILayout.Popup(new GUIContent("Sorting Layer", "Name of the renderer's sorting layer."),
                    haveDifferentValues ? 0 : Array.IndexOf(sortingLayerNames, sortingLayerName), sortingLayerNames);
            }

            if (EditorGUI.EndChangeCheck())
                for (int i = 0; i < _properties.tentacleData.Length; i++)
                    _properties.tentacleData[i].SetSortingLayerName(sortingLayerNames[value]);

            if (haveDifferentValues) EditorGUI.showMixedValue = false;
        }

        private void DrawSortingOrder()
        {
            var haveDifferentValues = false;
            var sortingOrder = _properties.tentacleData[0].meshRenderer.sortingOrder;
            for (int i = 1; i < _properties.tentacleData.Length; i++)
                if (sortingOrder != _properties.tentacleData[i].meshRenderer.sortingOrder)
                {
                    haveDifferentValues = true;
                    break;
                }

            if (haveDifferentValues) EditorGUI.showMixedValue = true;

            EditorGUI.BeginChangeCheck();

            int value;
            if (_properties.IsPropertyModified(typeof(MeshRenderer), "SortingOrder"))
            {
                TentacleEditorUtil.BeginBoldLabels();
                value = EditorGUILayout.IntField(
                    new GUIContent("Order in Layer", "Renderer's order within the sorting layer."), sortingOrder);
                TentacleEditorUtil.EndBoldLabels();
            }
            else
                value = EditorGUILayout.IntField(
                    new GUIContent("Order in Layer", "Renderer's order within the sorting layer."), sortingOrder);

            if (EditorGUI.EndChangeCheck())
                for (int i = 0; i < _properties.tentacleData.Length; i++)
                    _properties.tentacleData[i].SetSortingOrder(value);

            if (haveDifferentValues) EditorGUI.showMixedValue = false;
        }

        private void DrawLength()
        {
            var haveDifferentValues = false;
            var length = _properties.tentacleData[0].Length;
            if (length.Item2)
                haveDifferentValues = true;
            else
                for (int i = 1; i < _properties.tentacleData.Length; i++)
                    if (length.Item1 != _properties.tentacleData[i].Length.Item1)
                    {
                        haveDifferentValues = true;
                        break;
                    }

            if (haveDifferentValues) EditorGUI.showMixedValue = true;
            EditorGUI.BeginChangeCheck();

            float value;
            if (_properties.IsPropertyModified(typeof(SpringJoint2D), "Distance"))
            {
                TentacleEditorUtil.BeginBoldLabels();
                value = EditorGUILayout.FloatField(new GUIContent("Length", "The length of the tentacle."),
                    length.Item1);
                TentacleEditorUtil.EndBoldLabels();
            }
            else
                value = EditorGUILayout.FloatField(new GUIContent("Length", "The length of the tentacle."),
                    length.Item1);

            if (EditorGUI.EndChangeCheck())
                for (int i = 0; i < _properties.tentacleData.Length; i++)
                    _properties.tentacleData[i].SetLength(value);
            if (haveDifferentValues) EditorGUI.showMixedValue = false;
        }

        private void InitializeSortingLayers()
        {
            sortingLayerNames = new string[SortingLayer.layers.Length];
            for (int i = 0; i < SortingLayer.layers.Length; i++)
                sortingLayerNames[i] = SortingLayer.layers[i].name;
        }
    }
}