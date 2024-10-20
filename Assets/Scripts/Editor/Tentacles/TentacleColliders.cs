using UnityEditor;
using UnityEngine;

namespace Starblast.Tentacles
{
    public class TentacleColliders
    {
        private TentacleProperties _properties;
        private SerializedObject _serializedObject;

        public TentacleColliders(SerializedObject serializedObject, TentacleProperties props)
        {
            _properties = props;
            _serializedObject = serializedObject;
        }

        public void DrawControls()
        {
            DrawColliderType();
            DrawIsTrigger();
            var isPolyColliderEnabled = DrawPolygonalCollider();
            DrawReduction(isPolyColliderEnabled);
            DrawIsPolygonalTrigger(isPolyColliderEnabled);
        }

        private void DrawColliderType()
        {
            var haveDifferentValues = false;
            var currentType = _properties.tentacleData[0].CurrentColliderType;
            if (currentType == (TentacleData.ColliderType)(-1))
            {
                haveDifferentValues = true;
            }
            else
            {
                for (int i = 1; i < _properties.tentacleData.Length; i++)
                {
                    if (currentType != _properties.tentacleData[i].CurrentColliderType)
                    {
                        haveDifferentValues = true;
                        break;
                    }
                }
            }

            if (haveDifferentValues)
            {
                EditorGUI.showMixedValue = true;
            }

            EditorGUI.BeginChangeCheck();

            TentacleData.ColliderType value;
            if (_properties.IsPropertyModified(typeof(CircleCollider2D), "Enabled"))
            {
                EditorUtil.BeginBoldLabels();
                value = (TentacleData.ColliderType)EditorGUILayout.EnumPopup(
                    new GUIContent("Collider", "Type of the collider the tentacle will use."), currentType);
                EditorUtil.EndBoldLabels();
            }
            else
            {
                value = (TentacleData.ColliderType)EditorGUILayout.EnumPopup(
                    new GUIContent("Collider", "Type of the collider the tentacle will use."), currentType);
            }

            if (EditorGUI.EndChangeCheck())
            {
                for (int i = 0; i < _properties.tentacleData.Length; i++)
                {
                    _properties.tentacleData[i].SetCollider(value);
                }
            }

            if (haveDifferentValues)
            {
                EditorGUI.showMixedValue = false;
            }
        }

        private bool DrawIsTrigger()
        {
            var haveDifferentValues = false;
            var isTrigger = _properties.tentacleData[0].IsTrigger;
            if (isTrigger.Item2)
            {
                haveDifferentValues = true;
            }
            else
            {
                for (int i = 1; i < _properties.tentacleData.Length; i++)
                {
                    if (isTrigger.Item1 != _properties.tentacleData[i].IsTrigger.Item1)
                    {
                        haveDifferentValues = true;
                        break;
                    }
                }
            }

            if (haveDifferentValues) EditorGUI.showMixedValue = true;

            EditorGUI.BeginChangeCheck();

            bool value;
            if (_properties.IsPropertyModified(typeof(CircleCollider2D), "IsTrigger"))
            {
                EditorUtil.BeginBoldLabels();
                value = EditorGUILayout.Toggle(
                    new GUIContent("Is Trigger", "Whether the collider behaves as a trigger or not."), isTrigger.Item1);
                EditorUtil.EndBoldLabels();
            }
            else
            {
                value = EditorGUILayout.Toggle(
                    new GUIContent("Is Trigger", "Whether the collider behaves as a trigger or not."), isTrigger.Item1);
            }

            if (EditorGUI.EndChangeCheck())
            {
                for (int i = 0; i < _properties.tentacleData.Length; i++)
                {
                    _properties.tentacleData[i].SetTrigger(value);
                }
            }

            if (haveDifferentValues)
            {
                EditorGUI.showMixedValue = false;
            }

            return value;
        }

        private bool DrawPolygonalCollider()
        {
            var haveDifferentValues = false;
            var isPolygonalEnabled = _properties.tentacleData[0].PolygonalEnabled;
            for (int i = 1; i < _properties.tentacleData.Length; i++)
            {
                if (isPolygonalEnabled != _properties.tentacleData[i].PolygonalEnabled)
                {
                    haveDifferentValues = true;
                    break;
                }
            }

            if (haveDifferentValues)
            {
                EditorGUI.showMixedValue = true;
            }

            EditorGUI.BeginChangeCheck();

            bool value;
            if (_properties.IsPropertyModified(typeof(PolygonCollider2D), "Enabled"))
            {
                EditorUtil.BeginBoldLabels();
                value = EditorGUILayout.Toggle(
                    new GUIContent("Polygonal Collider", "Whether the polygonal collider enabled or not."),
                    isPolygonalEnabled);
                EditorUtil.EndBoldLabels();
            }
            else
            {
                value = EditorGUILayout.Toggle(
                    new GUIContent("Polygonal Collider", "Whether the polygonal collider enabled or not."),
                    isPolygonalEnabled);
            }

            if (EditorGUI.EndChangeCheck())
            {
                for (int i = 0; i < _properties.tentacleData.Length; i++)
                {
                    _properties.tentacleData[i].PolygonalEnabled = value;
                }
            }

            if (haveDifferentValues) EditorGUI.showMixedValue = false;

            return value;
        }

        private void DrawReduction(bool enabled)
        {
            if (enabled)
                EditorGUILayout.IntSlider(_properties.reduction, 0, 32,
                    new GUIContent("   Reduction",
                        "Simplify polygonal collider for better performance. 0 = no reduction."));
        }

        private void DrawIsPolygonalTrigger(bool enabled)
        {
            if (enabled)
            {
                GUI.enabled = false;
                EditorGUILayout.Toggle(
                    new GUIContent("   Is Trigger",
                        "Polygonal collider is building a the realtime so it can only by set as trigger, otherwise it won't work with Unity physics correctly."),
                    true);
                GUI.enabled = true;
            }
        }
    }
}