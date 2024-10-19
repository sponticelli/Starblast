using UnityEditor;
using UnityEngine;

namespace Starblast.Tentacles
{
    public class TentacleBehavior
    {
        private TentacleProperties _properties;
        private bool isDraggedHasRigidbody = false;
        private SerializedObject _serializedObject;

        public TentacleBehavior(SerializedObject serializedObject, TentacleProperties props)
        {
            _properties = props;
            _serializedObject = serializedObject;
        }

        public void DrawControls()
        {
            DrawBodyType();
            DrawTarget();
            DrawCatchReleaseButtons();
            DrawAnimations();
            EditorGUILayout.PropertyField(_properties.speed, new GUIContent("Speed", "Gonna reach target with this force. Set None to disable."));
            DrawMass();
            DrawDrag();
            DrawGravity();
            DrawStiffness();
        }

        public void RenewTarget()
        {
            var draggedGOs = DragAndDrop.objectReferences;
            if (draggedGOs != null && draggedGOs.Length > 0)
            {
                if (draggedGOs[0] is GameObject draggedGO)
                {
                    isDraggedHasRigidbody = draggedGO.GetComponent<Rigidbody2D>() != null;
                }
            }
        }

        private void DrawBodyType()
        {
            var haveDifferentValues = false;
            var currentType = _properties.tentacleData[0].TentacleBodyType;
            if (currentType == (TentacleData.BodyType)(-1))
                haveDifferentValues = true;
            else
                for (int i = 1; i < _properties.tentacleData.Length; i++)
                    if (currentType != _properties.tentacleData[i].TentacleBodyType)
                    {
                        haveDifferentValues = true;
                        break;
                    }

            if (haveDifferentValues) EditorGUI.showMixedValue = true;
            EditorGUI.BeginChangeCheck();

            TentacleData.BodyType value;
            if (_properties.IsPropertyModified(typeof(SpringJoint2D), "Frequency"))
            {
                TentacleEditorUtil.BeginBoldLabels();
                value = (TentacleData.BodyType)EditorGUILayout.EnumPopup(
                    new GUIContent("Attached To", "Whether tentacle attached or detached."), currentType);
                TentacleEditorUtil.EndBoldLabels();
            }
            else
                value = (TentacleData.BodyType)EditorGUILayout.EnumPopup(
                    new GUIContent("Attached To", "Whether tentacle attached or detached."), currentType);

            if (EditorGUI.EndChangeCheck())
                for (int i = 0; i < _properties.tentacleData.Length; i++)
                    _properties.tentacleData[i].SetBodyType(value);
            if (haveDifferentValues) EditorGUI.showMixedValue = false;

            if (value == TentacleData.BodyType.rigidbody)
            {
                DrawParentBody();
                DrawParentBodyOffset();
            }
        }
        
        private void DrawParentBody()
        {
            var bodyHasDifferentValues = false;
            var currentBody = _properties.tentacleData[0].ParentBody;
            for (int i = 1; i < _properties.tentacleData.Length; i++)
                if (currentBody != _properties.tentacleData[i].ParentBody)
                {
                    bodyHasDifferentValues = true;
                    break;
                }

            if (bodyHasDifferentValues) EditorGUI.showMixedValue = true;
            EditorGUI.BeginChangeCheck();

            Rigidbody2D body;
            if (_properties.IsPropertyModified(typeof(HingeJoint2D), "ConnectedRigidBody"))
            {
                TentacleEditorUtil.BeginBoldLabels();
                body = (Rigidbody2D)EditorGUILayout.ObjectField(
                    new GUIContent("   Parent Body", "Attach tentacle to this parent."), currentBody,
                    typeof(Rigidbody2D), true);
                TentacleEditorUtil.EndBoldLabels();
            }
            else
                body = (Rigidbody2D)EditorGUILayout.ObjectField(
                    new GUIContent("   Parent Body", "Attach tentacle to this parent."), currentBody,
                    typeof(Rigidbody2D), true);

            if (EditorGUI.EndChangeCheck())
                for (int i = 0; i < _properties.tentacleData.Length; i++)
                    _properties.tentacleData[i].SetParentBody(body);
            if (bodyHasDifferentValues) EditorGUI.showMixedValue = false;
        }

        private void DrawParentBodyOffset()
        {
            var hasDifferentValues = false;
            var currentOffset = _properties.tentacleData[0].ParentBodyOffset;
            for (int i = 1; i < _properties.tentacleData.Length; i++)
                if (currentOffset != _properties.tentacleData[i].ParentBodyOffset)
                {
                    hasDifferentValues = true;
                    break;
                }

            if (hasDifferentValues) EditorGUI.showMixedValue = true;
            EditorGUI.BeginChangeCheck();

            Vector2 value;
            if (_properties.IsPropertyModified(typeof(HingeJoint2D), "ConnectedAnchor.x") ||
                _properties.IsPropertyModified(typeof(HingeJoint2D), "ConnectedAnchor.y"))
            {
                TentacleEditorUtil.BeginBoldLabels();
                value = EditorGUILayout.Vector2Field(new GUIContent("   Parent Body Offset", "."), currentOffset);
                TentacleEditorUtil.EndBoldLabels();
            }
            else
                value = EditorGUILayout.Vector2Field(new GUIContent("   Parent Body Offset", "."), currentOffset);

            if (EditorGUI.EndChangeCheck())
                for (int i = 0; i < _properties.tentacleData.Length; i++)
                    _properties.tentacleData[i].SetParentBodyOffset(value);
            if (hasDifferentValues) EditorGUI.showMixedValue = false;
        }

        private void DrawTarget()
        {
            if (DragAndDrop.objectReferences.Length > 0 && isDraggedHasRigidbody ||
                DragAndDrop.objectReferences.Length == 0 &&
                _properties.tentacleTargetRigidbody.objectReferenceValue != null)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(_properties.tentacleTargetRigidbody,
                    new GUIContent("Target", "Tentacle will be trying to reach this target."));
                if (EditorGUI.EndChangeCheck())
                    _properties.tentacleTarget.objectReferenceValue =
                        ((Rigidbody2D)_properties.tentacleTargetRigidbody.objectReferenceValue)?.transform;
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(_properties.tentacleTarget,
                    new GUIContent("Target", "Tentacle will be trying to reach this target."));
                if (EditorGUI.EndChangeCheck())
                    _properties.tentacleTargetRigidbody.objectReferenceValue = null;
            }
        }

        private void DrawCatchReleaseButtons()
        {
            if (!EditorApplication.isPlaying) return;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Catch target",
                    "Catch the target set in the above field. For test purposes. Active in the playmode only.")))
            {
                for (int i = 0; i < _properties.tentacleData.Length; i++)
                    _properties.tentacleData[i].Tentacle.Catch();
            }

            if (GUILayout.Button(new GUIContent("Release target",
                    "Release catched target set in the above field. For test purposes. Active in the playmode only.")))
            {
                for (int i = 0; i < _properties.tentacleData.Length; i++)
                    _properties.tentacleData[i].Tentacle.Release();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawAnimations()
        {
            EditorGUILayout.PropertyField(_properties.animation,
                new GUIContent("Animation", "Additional physics-based animation of the tentacle."));
            //var enumValue = animation.enumValueIndex;
            if (_properties.animation.enumValueIndex != 0)
            {
                EditorGUILayout.PropertyField(_properties.frequency,
                    new GUIContent("   Frequency", "Frequency of the tentacle's animations."));
                EditorGUILayout.PropertyField(_properties.amplitude,
                    new GUIContent("   Amplitude", "Strength of the tentacle's animations."));
                EditorGUILayout.PropertyField(_properties.animationDelay,
                    new GUIContent("   Delay", "Delay to start the animation (for randomization purposes)."));
            }
        }

        private void DrawMass()
        {
            var haveDifferentValues = false;
            var mass = _properties.tentacleData[0].Mass;
            if (mass.Item2)
                haveDifferentValues = true;
            else
                for (int i = 1; i < _properties.tentacleData.Length; i++)
                    if (mass.Item1 != _properties.tentacleData[i].Mass.Item1)
                    {
                        haveDifferentValues = true;
                        break;
                    }

            if (haveDifferentValues) EditorGUI.showMixedValue = true;
            EditorGUI.BeginChangeCheck();

            float value;
            if (_properties.IsPropertyModified(typeof(Rigidbody2D), "Mass"))
            {
                TentacleEditorUtil.BeginBoldLabels();
                value = EditorGUILayout.FloatField(
                    new GUIContent("Mass", "The mass of an each segment of this tentacle."), mass.Item1);
                TentacleEditorUtil.EndBoldLabels();
            }
            else
                value = EditorGUILayout.FloatField(
                    new GUIContent("Mass", "The mass of an each segment of this tentacle."), mass.Item1);

            if (EditorGUI.EndChangeCheck())
                for (int i = 0; i < _properties.tentacleData.Length; i++)
                    _properties.tentacleData[i].SetMass(value);
            if (haveDifferentValues) EditorGUI.showMixedValue = false;
        }

        private void DrawDrag()
        {
            var haveDifferentValues = false;
            var drag = _properties.tentacleData[0].Drag;
            if (drag.Item2)
            {
                haveDifferentValues = true;
            }
            else
                for (int i = 1; i < _properties.tentacleData.Length; i++)
                {
                    if (drag.Item1 != _properties.tentacleData[i].Drag.Item1)
                    {
                        haveDifferentValues = true;
                        break;
                    }
                }

            if (haveDifferentValues) EditorGUI.showMixedValue = true;
            EditorGUI.BeginChangeCheck();

            float value;
            if (_properties.IsPropertyModified(typeof(Rigidbody2D), "LinearDrag"))
            {
                TentacleEditorUtil.BeginBoldLabels();
                value = EditorGUILayout.FloatField(
                    new GUIContent("Drag", "The drag of an each segment of this tentacle."), drag.Item1);
                TentacleEditorUtil.EndBoldLabels();
            }
            else
                value = EditorGUILayout.FloatField(
                    new GUIContent("Drag", "The drag of an each segment of this tentacle."), drag.Item1);

            if (EditorGUI.EndChangeCheck())
                for (int i = 0; i < _properties.tentacleData.Length; i++)
                    _properties.tentacleData[i].SetDrag(value);
            if (haveDifferentValues) EditorGUI.showMixedValue = false;
        }

        private void DrawGravity()
        {
            var haveDifferentValues = false;
            var gravity = _properties.tentacleData[0].Gravity;
            if (gravity.Item2)
                haveDifferentValues = true;
            else
                for (int i = 1; i < _properties.tentacleData.Length; i++)
                    if (gravity.Item1 != _properties.tentacleData[i].Gravity.Item1)
                    {
                        haveDifferentValues = true;
                        break;
                    }

            if (haveDifferentValues) EditorGUI.showMixedValue = true;
            EditorGUI.BeginChangeCheck();

            float value;
            if (_properties.IsPropertyModified(typeof(Rigidbody2D), "GravityScale"))
            {
                TentacleEditorUtil.BeginBoldLabels();
                value = EditorGUILayout.FloatField(new GUIContent("Gravity", "How much gravity affects the tentacle."),
                    gravity.Item1);
                TentacleEditorUtil.EndBoldLabels();
            }
            else
                value = EditorGUILayout.FloatField(new GUIContent("Gravity", "How much gravity affects the tentacle."),
                    gravity.Item1);

            if (EditorGUI.EndChangeCheck())
                for (int i = 0; i < _properties.tentacleData.Length; i++)
                    _properties.tentacleData[i].SetGravity(value);
            if (haveDifferentValues) EditorGUI.showMixedValue = false;
        }

        private void DrawStiffness()
        {
            var haveDifferentValues = false;
            var stiffness = _properties.tentacleData[0].Stiffness;
            if (stiffness.Item2)
                haveDifferentValues = true;
            else
                for (int i = 1; i < _properties.tentacleData.Length; i++)
                    if (stiffness.Item1 != _properties.tentacleData[i].Stiffness.Item1)
                    {
                        haveDifferentValues = true;
                        break;
                    }

            if (haveDifferentValues) EditorGUI.showMixedValue = true;
            EditorGUI.BeginChangeCheck();

            float value;
            if (_properties.IsPropertyModified(typeof(SpringJoint2D), "Frequency"))
            {
                TentacleEditorUtil.BeginBoldLabels();
                value = EditorGUILayout.FloatField(
                    new GUIContent("Stiffness", "The stiffness of an each segment of this tentacle."), stiffness.Item1);
                TentacleEditorUtil.EndBoldLabels();
            }
            else
                value = EditorGUILayout.FloatField(
                    new GUIContent("Stiffness", "The stiffness of an each segment of this tentacle."), stiffness.Item1);

            if (EditorGUI.EndChangeCheck())
                for (int i = 0; i < _properties.tentacleData.Length; i++)
                    _properties.tentacleData[i].SetStiffness(value);
            if (haveDifferentValues) EditorGUI.showMixedValue = false;
        }
    }
}