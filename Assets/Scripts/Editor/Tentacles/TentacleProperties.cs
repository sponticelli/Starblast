using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Starblast.Tentacles
{
    public class TentacleProperties
    {
        public SerializedProperty material,
            color,
            textureType,
            smoothness,
            pivotCapSmoothness,
            tipCapSmoothness,
            width,
            shape,
            reduction,
            tentacleTarget,
            tentacleTargetRigidbody,
            speed,
            animation,
            frequency,
            amplitude,
            animationDelay;
        
        public TentacleData[] tentacleData;
        public TentacleProperties(SerializedObject serializedObject, Object[] targets)
        {
            Initialize(serializedObject);
            CreateTentacleData(targets);
        }

        private void CreateTentacleData(Object[] targets)
        {
            tentacleData = new TentacleData[targets.Length];
            for (int i = 0; i < targets.Length; i++)
                tentacleData[i] = new TentacleData((Tentacle)targets[i]);
        }

        private void Initialize(SerializedObject serializedObject)
        {
            color = serializedObject.FindProperty("color");
            material = serializedObject.FindProperty("material");
            textureType = serializedObject.FindProperty("textureType");
            smoothness = serializedObject.FindProperty("smoothness");
            pivotCapSmoothness = serializedObject.FindProperty("pivotCapSmoothness");
            tipCapSmoothness = serializedObject.FindProperty("tipCapSmoothness");
            width = serializedObject.FindProperty("width");
            shape = serializedObject.FindProperty("shape");
            reduction = serializedObject.FindProperty("reduction");
            tentacleTarget = serializedObject.FindProperty("target");
            tentacleTargetRigidbody = serializedObject.FindProperty("targetRigidbody");
            speed = serializedObject.FindProperty("speed");
            animation = serializedObject.FindProperty("animation");
            frequency = serializedObject.FindProperty("frequency");
            amplitude = serializedObject.FindProperty("amplitude");
            animationDelay = serializedObject.FindProperty("animationDelay");
        }
        
        public bool IsPropertyModified(Type type, string property)
        {
            if (EditorApplication.isPlaying) return false;
            if (tentacleData.Length == 1)
            {
                var modifications = PrefabUtility.GetPropertyModifications(tentacleData[0].Tentacle);
                if (modifications != null)
                    for (int j = 0; j < modifications.Length; j++)
                    {
                        var modification = modifications[j];
                        if (modification.target.GetType() == type && modification.propertyPath == $"m_{property}")
                            return true;
                    }
            }

            return false;
        }
    }
}