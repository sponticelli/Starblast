using System;
using UnityEditor;
using UnityEngine;

namespace Starblast.Tentacles
{
    [CustomEditor(typeof(Entities.Tentacles.Tentacle)), CanEditMultipleObjects]
    public class TentacleEditor : Editor
    {
        private TentacleProperties _properties;
        private TentacleDebug _debug;
        private TentacleRendering _rendering;
        private TentacleColliders _colliders;
        private TentacleBehavior _behavior;
        
        private GUIStyle header;
        private PropertyModification[] modifications;

        private void OnEnable()
        {
            _properties = new TentacleProperties(serializedObject, targets);
            _debug = new TentacleDebug(serializedObject, _properties);
            _rendering = new TentacleRendering(serializedObject, _properties);
            _colliders = new TentacleColliders(serializedObject, _properties);
            _behavior = new TentacleBehavior(serializedObject, _properties);


            InitializeStyles();

            EditorApplication.update += _behavior.RenewTarget;
            Undo.undoRedoPerformed += RenewEditor;
        }

        private void OnDisable()
        {
            EditorApplication.update -= _behavior.RenewTarget;
            Undo.undoRedoPerformed -= RenewEditor;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawHeader("Rendering", "This section includes all visual settings.");
            _rendering.DrawControls();

            DrawHeader("Colliders", "This section includes all settings for the colliders and their types.");
            _colliders.DrawControls();

            DrawHeader("Behaviour", "This section includes all settings for physics and AI.");
            _behavior.DrawControls();

            DrawHeader("Debug", "For debug purposes only, this will not be rendered.");
            _debug.DrawControls();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawHeader(string text, string description = "")
        {
            EditorGUILayout.Space();
            var rect = GUILayoutUtility.GetRect(1f, 20f);
            EditorGUI.LabelField(rect, new GUIContent(text, description), header);
        }
        
        private void RenewEditor()
        {
            _properties = new TentacleProperties(serializedObject, targets);
            Repaint();
        }
        private void InitializeStyles()
        {
            header = new GUIStyle();
            header.normal.textColor = Color.white;
            header.alignment = TextAnchor.MiddleLeft;
            header.fontStyle = FontStyle.Bold;
        }
    }
}