using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Wanna.DebugEx;

namespace Module.Task.Editor
{
    [CustomEditor(typeof(BaseTask), true)]
    public class BaseTaskEditor : UnityEditor.Editor
    {
        bool foldGeneral = false;
        bool foldDebug = false;
        bool foldMain = false;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawGeneralSettings();
            DrawMainSettings();
            DrawDebugSettings();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawGeneralSettings()
        {
            EditorGUILayout.Space();
            foldGeneral = Foldout("General Settings", foldGeneral);
            if (foldGeneral)
            {
                SerializedProperty taskSize = serializedObject.FindProperty("taskSize");
                SerializedProperty mw = serializedObject.FindProperty("mw");

                taskSize.floatValue = EditorGUILayout.Slider("検出サイズ", taskSize.floatValue, 0f, 6f);
                mw.intValue = EditorGUILayout.IntField("MonoWork", mw.intValue);
                mw.intValue = Mathf.Clamp(mw.intValue, 1, int.MaxValue);
            }
        }

        private void DrawMainSettings()
        {
            EditorGUILayout.Space();
            foldMain = Foldout("Main Settings", foldMain);
            if (foldMain)
            {
                DrawDefaultProperty();
            }
        }

        private void DrawDebugSettings()
        {
            EditorGUILayout.Space();
            foldDebug = Foldout("Debug View", foldDebug);
            if (foldDebug)
            {
                SerializedProperty debugAssignPoints = serializedObject.FindProperty("debugAssignPoints");
                SerializedProperty state = serializedObject.FindProperty("state");
                SerializedProperty currentProgress = serializedObject.FindProperty("currentProgress");
                SerializedProperty currentWorkerCount = serializedObject.FindProperty("currentWorkerCount");

                debugAssignPoints.boolValue = EditorGUILayout.Toggle("アサインポイントを表示", debugAssignPoints.boolValue);

                EditorGUILayout.LabelField("タスクの状態", ((TaskState)state.enumValueFlag).ToString());
                EditorGUILayout.LabelField("進捗率", $"{currentProgress.floatValue * 100f}%");
                EditorGUILayout.LabelField("ワーカー数", currentWorkerCount.intValue.ToString());
            }
        }

        private void DrawDefaultProperty()
        {
            SerializedProperty iterator = serializedObject.GetIterator();

            //既に表示してるやつはスキップ
            for (int i = 0; i < 7; i++)
            {
                iterator.NextVisible(true);
            }

            for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
            {
                EditorGUILayout.PropertyField(iterator, true);
            }
        }

        private static bool Foldout(string title, bool display)
        {
            var style = new GUIStyle("ShurikenModuleTitle");
            style.font = EditorStyles.whiteBoldLabel.font;
            style.border = new RectOffset(15, 7, 4, 4);
            style.fixedHeight = 22;
            style.contentOffset = new Vector2(20f, -2f);
            style.fontSize = 13;

            var rect = GUILayoutUtility.GetRect(16f, 22f, style);
            GUI.Box(rect, title, style);

            var e = Event.current;

            var toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
            if (e.type == EventType.Repaint)
            {
                EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
            }

            if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
            {
                display = !display;
                e.Use();
            }

            return display;
        }
    }
}