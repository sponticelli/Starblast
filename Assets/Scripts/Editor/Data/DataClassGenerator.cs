using UnityEngine;
using UnityEditor;
using System.IO;

namespace Starblast.Data
{
    public class DataClassGenerator : EditorWindow
    {
        private string entityName = "";
        private string typeName = "";
        private string targetFolder = "Assets";
        private bool useType = false;

        [MenuItem("Starblast/Tools/Data Class Generator")]
        public static void ShowWindow()
        {
            GetWindow<DataClassGenerator>("Data Class Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Data Class Generator", EditorStyles.boldLabel);

            entityName = EditorGUILayout.TextField("Entity Name", entityName);
            
            useType = EditorGUILayout.Toggle("Use Type", useType);
            if (useType)
            {
                typeName = EditorGUILayout.TextField("Type", typeName);
            }

            if (GUILayout.Button("Select Target Folder"))
            {
                targetFolder = EditorUtility.OpenFolderPanel("Select Target Folder", "Assets", "");
                targetFolder = string.IsNullOrEmpty(targetFolder) ? "Assets" : 
                    targetFolder.Substring(Application.dataPath.Length - "Assets".Length);
            }

            EditorGUILayout.LabelField("Target Folder:", targetFolder);

            if (GUILayout.Button("Generate Data Classes"))
            {
                GenerateStubClasses();
            }
        }

        private void GenerateStubClasses()
        {
            if (string.IsNullOrEmpty(entityName) || (useType && string.IsNullOrEmpty(typeName)))
            {
                EditorUtility.DisplayDialog("Error", "Entity Name is required. If using Type, Type is also required.", "OK");
                return;
            }

            string entityNames = PluralizeNoun(entityName);
            string types = useType ? PluralizeNoun(typeName) : "";

            string prefix = useType ? $"{entityName}{typeName}" : entityName;
            string namespaceSuffix = useType ? $".{types}" : "";

            GenerateFile($"I{prefix}Data.cs",
                GetIEntityTypeDataContent(entityName, typeName, entityNames, types, useType));
            GenerateFile($"{prefix}DataSO.cs",
                GetEntityTypeDataContent(entityName, typeName, entityNames, types, useType));

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Success", "Stub classes generated successfully.", "OK");
        }

        private void GenerateFile(string fileName, string content)
        {
            string filePath = Path.Combine(targetFolder, fileName);
            File.WriteAllText(filePath, content);
        }

        private string GetIEntityTypeDataContent(string entityName, string typeName, string entityNames, string types, bool useType)
        {
            string namespaceSuffix = useType ? $".{types}" : "";
            string interfaceName = useType ? $"I{entityName}{typeName}Data" : $"I{entityName}Data";

            return $@"using UnityEngine;

namespace Starblast.Data.{entityNames}{namespaceSuffix}
{{
    public interface {interfaceName} : IData
    {{
        // Add your data here
    }}
}}";
        }

        private string GetEntityTypeDataContent(string entityName, string typeName, string entityNames, string types, bool useType)
        {
            string namespaceSuffix = useType ? $".{types}" : "";
            string className = useType ? $"{entityName}{typeName}DataSO" : $"{entityName}DataSO";
            string interfaceName = useType ? $"I{entityName}{typeName}Data" : $"I{entityName}Data";
            string menuName = useType ? $"Starblast/{entityName}/Data/{typeName} Data" : $"Starblast/{entityName}/Data";

            return $@"using UnityEngine;

namespace Starblast.Data.{entityNames}{namespaceSuffix}
{{
    [CreateAssetMenu(fileName = ""New{className}"", menuName = ""{menuName}"")]
    public class {className} : ScriptableObject, {interfaceName}
    {{
        // Add your data here
    }}
}}";
        }
        
        private string PluralizeNoun(string noun)
        {
            // This is a very simple pluralization method.
            // For a more robust solution, consider using a library like Humanizer.
            if (string.IsNullOrEmpty(noun))
                return noun;

            if (noun.EndsWith("s") || noun.EndsWith("sh") || noun.EndsWith("ch") || noun.EndsWith("x") ||
                noun.EndsWith("z"))
                return noun + "es";
            if (noun.EndsWith("y") && !IsVowel(noun[noun.Length - 2]))
                return noun.Substring(0, noun.Length - 1) + "ies";
            return noun + "s";
        }

        private bool IsVowel(char c)
        {
            return "aeiouAEIOU".IndexOf(c) >= 0;
        }
    }
}