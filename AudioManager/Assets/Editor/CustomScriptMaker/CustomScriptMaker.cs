using System;
using System.CodeDom;
using System.Collections;
using System.IO;
using System.Linq;
using System.Xml;
using AudioEngine;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Object = System.Object;

namespace CustomScriptingEngine
{
    public class CustomScriptMaker
    {
        [MenuItem("Assets/Set Editor Script")]
        public static void SetEditorScript()
        {
            //The asset path for the 
            var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            var classFileName = assetPath.Split('/')[assetPath.Split('/').Length - 1].Split('.')[0];
            Debug.Log($"[{classFileName}Editor.cs]: Find in <Assets/Editor>");

            //The directory path for the Editor Script Files
            var directoryPath = Path.Combine($"{Application.dataPath}", "Editor");

            //Check if the assetpath selected is from a cs file, not the actual script and not an editor
            if (Selection.activeObject.GetType() == typeof(MonoScript) && Selection.activeObject.name.Split('.')[0] != "CustomScriptMaker" && !Selection.activeObject.name.Contains("Editor"))
            {
                //Check if the editor directory path exists / if not create the directory
                if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

                var editorFilePath = Path.Combine($"{directoryPath}", $"{classFileName}Editor.cs");

                if (!File.Exists(editorFilePath))
                {
                    var file = File.CreateText(editorFilePath);
                    file.WriteAsync(PopulateEditorScript());
                    file.Close();
                }

                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogWarning("The object selected is not a script, is the actual script of creation, or is already an editor script.\n You must select a normal script");
            }

            string PopulateEditorScript()
            {
                string editorScriptTemaplate;
                var dependencies = "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\nusing UnityEditor;\n";
                var customEditorSetting = $"\n[ExecuteInEditMode]\n[CustomEditor(typeof({classFileName}))]\n";
                var editorClassCreation = $"public class {classFileName}Editor : Editor\n" +
                                          "{\n" +
                                          $"     {classFileName} manager;\n\n" +
                                          "     public override void OnInspectorGUI()\n" +
                                          "     {\n" +
                                          $"         manager = target as {classFileName};\n\n" +
                                          "         DrawDefaultInspector();\n" +
                                          "     }\n" + 
                                          "\n}";

                editorScriptTemaplate = dependencies + customEditorSetting + editorClassCreation;
                return editorScriptTemaplate;
            }
        }
    }
}

