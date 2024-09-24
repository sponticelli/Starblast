using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Starblast.Audio
{
    [CustomEditor(typeof(SoundInfoList))]
    public class SoundInfoListEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Generate SoundNameEnum"))
            {
                GenerateSoundNameEnum();
            }
        }

        private void GenerateSoundNameEnum()
        {
            // Get the target SoundInfoList
            SoundInfoList soundInfoList = (SoundInfoList)target;

            // Get the names from the SoundInfoList
            List<string> soundNames = new List<string>();

            foreach (var sound in soundInfoList.Sounds)
            {
                if (sound != null && !string.IsNullOrEmpty(sound.Name))
                {
                    string enumName = sound.Name;

                    // Ensure enum name is valid
                    enumName = enumName.Replace(" ", "_");
                    enumName = RemoveInvalidChars(enumName);

                    soundNames.Add(enumName);
                }
            }

            // Get the path of the SoundInfoList script
            MonoScript monoScript = MonoScript.FromScriptableObject(soundInfoList);
            string scriptPath = AssetDatabase.GetAssetPath(monoScript);

            // Determine the directory path
            string directoryPath = Path.GetDirectoryName(scriptPath);

            // Define the enum file path
            string enumFilePath = Path.Combine(directoryPath, "SoundNameEnum.cs");

            // Generate the enum code
            StringBuilder enumBuilder = new StringBuilder();

            enumBuilder.AppendLine("using UnityEngine;");
            enumBuilder.AppendLine();
            enumBuilder.AppendLine("namespace Starblast.Audio");
            enumBuilder.AppendLine("{");
            enumBuilder.AppendLine("    public enum SoundNameEnum");
            enumBuilder.AppendLine("    {");

            for (int i = 0; i < soundNames.Count; i++)
            {
                string name = soundNames[i];
                string line = $"        {name}";
                if (i < soundNames.Count - 1)
                    line += ",";
                enumBuilder.AppendLine(line);
            }

            enumBuilder.AppendLine("    }");
            enumBuilder.AppendLine("}");

            // Write the enum code to the new file
            File.WriteAllText(enumFilePath, enumBuilder.ToString());

            // Refresh the AssetDatabase
            AssetDatabase.ImportAsset(enumFilePath);
            AssetDatabase.Refresh();

            Debug.Log("SoundNameEnum generated at " + enumFilePath);
        }

        private string RemoveInvalidChars(string name)
        {
            // Remove invalid characters for identifiers and ensure it doesn't start with a digit
            var sb = new StringBuilder();
            foreach (var c in name)
            {
                if (char.IsLetterOrDigit(c) || c == '_')
                    sb.Append(c);
                else
                    sb.Append('_'); // Replace invalid chars with underscore
            }

            // Ensure it doesn't start with a digit
            if (sb.Length > 0 && char.IsDigit(sb[0]))
                sb.Insert(0, '_');

            return sb.ToString();
        }
    }
}