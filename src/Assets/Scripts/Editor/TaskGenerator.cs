using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    /// <summary>
    /// タスクを自動生成するクラス
    /// </summary>
    static class TaskGenerator
    {
        private static readonly string TaskPath = @"Scripts\GameMain\Task";

        [MenuItem("GameObject/Task", priority = -100000)]
        static void GenerateTask()
        {
            string dirPath = Path.Combine(Application.dataPath, TaskPath);
            string fileName = CreateFileName();
            string filePath = Path.Combine(dirPath, fileName);
            string className = Path.ChangeExtension(fileName, null);

            using (FileStream fs = File.Create(filePath))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    string code = CreateCode(className);
                    sw.Write(code);
                }
            }

            GeneratingFlagHolder.instance.IsGenerating = true;
            GeneratingFlagHolder.instance.ClassName = className;

            AssetDatabase.Refresh();
        }


        [InitializeOnLoadMethod]
        static void OnCompileScripts()
        {
            if (!GeneratingFlagHolder.instance.IsGenerating)
                return;

            GameObject gameObject = new GameObject("Task");
            gameObject.tag = "Task";
            gameObject.layer = LayerMask.NameToLayer("Task");
            gameObject.isStatic = true;

            gameObject.AddComponent(FindGeneratedType());
            gameObject.AddComponent<SphereCollider>();

            GeneratingFlagHolder.instance.IsGenerating = false;
        }

        private static Type FindGeneratedType()
        {
            Type generatedType = null;
            foreach (System.Reflection.Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.Name == GeneratingFlagHolder.instance.ClassName)
                    {
                        generatedType = type;
                        break;
                    }
                }

                if (generatedType != null)
                    break;
            }

            return generatedType;
        }

        private static string CreateFileName()
        {
            return $"Task_{Guid.NewGuid().ToString("N")}.cs";
        }

        private static string CreateCode(string className)
        {
            return $@"using Module.Task;

namespace GameMain.Task
{{
    public class {className} : BaseTask
    {{
        public override void ExecuteJob() {{ }}
    }}
}}
";
        }
    }

    public class GeneratingFlagHolder : ScriptableSingleton<GeneratingFlagHolder>
    {
        public bool IsGenerating
        {
            get => isGenerating;
            set
            {
                isGenerating = value;
                Save(false);
            }
        }

        public string ClassName
        {
            get => className;
            set
            {
                className = value;
                Save(false);
            }
        }

        private bool isGenerating;
        private string className;
    }
}