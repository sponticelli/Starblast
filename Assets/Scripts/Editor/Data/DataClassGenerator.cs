using UnityEngine;
using UnityEditor;
using System.IO;

namespace Starblast.Data
{
    public class StubClassGenerator : EditorWindow
    {
        private string entityName = "";
        private string typeName = "";
        private string targetFolder = "Assets";

        [MenuItem("Starblast/Tools/Data Class Generator")]
        public static void ShowWindow()
        {
            GetWindow<StubClassGenerator>("Data Class Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Data Class Generator", EditorStyles.boldLabel);

            entityName = EditorGUILayout.TextField("Entity Name", entityName);
            typeName = EditorGUILayout.TextField("Type", typeName);

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
            if (string.IsNullOrEmpty(entityName) || string.IsNullOrEmpty(typeName))
            {
                EditorUtility.DisplayDialog("Error", "Entity Name and Type are required.", "OK");
                return;
            }

            string entityNames = PluralizeNoun(entityName);
            string types = PluralizeNoun(typeName);

            GenerateFile($"I{entityName}{typeName}Data.cs",
                GetIEntityTypeDataContent(entityName, typeName, entityNames, types));
            GenerateFile($"I{entityName}{typeName}DataProvider.cs",
                GetIEntityTypeDataProviderContent(entityName, typeName, entityNames, types));
            GenerateFile($"{entityName}{typeName}Data.cs",
                GetEntityTypeDataContent(entityName, typeName, entityNames, types));
            GenerateFile($"{entityName}{typeName}DataProvider.cs",
                GetEntityTypeDataProviderContent(entityName, typeName, entityNames, types));
            GenerateFile($"{entityName}{typeName}DataProviderMB.cs",
                GetEntityTypeDataProviderMBContent(entityName, typeName, entityNames, types));

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Success", "Stub classes generated successfully.", "OK");
        }

        private void GenerateFile(string fileName, string content)
        {
            string filePath = Path.Combine(targetFolder, fileName);
            File.WriteAllText(filePath, content);
        }

        private string GetIEntityTypeDataContent(string entityName, string typeName, string entityNames, string types)
        {
            return $@"using UnityEngine;

namespace Starblast.Data.{entityNames}.{types}
{{
    public interface I{entityName}{typeName}Data : IData
    {{
        // Add your data here
    }}
}}";
        }

        private string GetIEntityTypeDataProviderContent(string entityName, string typeName, string entityNames,
            string types)
        {
            return $@"namespace Starblast.Data.{entityNames}.{types}
{{
    public interface I{entityName}{typeName}DataProvider : IDataProvider<I{entityName}{typeName}Data>
    {{
        
    }}
}}";
        }

        private string GetEntityTypeDataContent(string entityName, string typeName, string entityNames, string types)
        {
            return $@"using UnityEngine;

namespace Starblast.Data.{entityNames}.{types}
{{
    [CreateAssetMenu(fileName = ""New{entityName}{typeName}Data"", menuName = ""Starblast/{entityName}/Data/{typeName} Data"")]
    public class {entityName}{typeName}DataSO : ScriptableObject, I{entityName}{typeName}Data
    {{
        // Add your data here
    }}
}}";
        }

        private string GetEntityTypeDataProviderContent(string entityName, string typeName, string entityNames,
            string types)
        {
            return $@"namespace Starblast.Data.{entityNames}.{types}
{{
    public class {entityName}{typeName}DataProvider : I{entityName}{typeName}DataProvider
    {{
        public {entityName}{typeName}DataProvider(I{entityName}{typeName}Data data)
        {{
            Data = data;
        }}

        public I{entityName}{typeName}Data Data {{ get; }}
    }}
}}";
        }

        private string GetEntityTypeDataProviderMBContent(string entityName, string typeName, string entityNames,
            string types)
        {
            return $@"using UnityEngine;

namespace Starblast.Data.{entityNames}.{types}
{{
    [AddComponentMenu(""Starblast/{entityName}/Data/{typeName} Data Provider"")]
    public class {entityName}{typeName}DataProviderMB : MonoBehaviour, I{entityName}{typeName}DataProvider
    {{
        [field: SerializeField] private {entityName}{typeName}DataSO _data;
        
        public I{entityName}{typeName}Data Data => _data;
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