using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AddressableManager.AddressableSetter
{
    internal class ListsEditor<T> where T : ScriptableObject
    {
        private bool AutoUpdate { get; set; }
        private UnityEditor.Editor MainEditor { get; }
        private bool ShowList { get; set; }
        private CreateDisplayList DisplayList { get; } = new CreateDisplayList();
        internal ListsEditor(UnityEditor.Editor editor, bool showList = false)
        {
            MainEditor = editor;
            ShowList = showList;
        }
        internal bool Init(string status, Dictionary<string, Tuple<List<AData>, AutoLoad>> lists)
        {
            ShowList = EditorGUILayout.BeginFoldoutHeaderGroup(ShowList, status);
            if (ShowList)
            {
                AutoUpdate = GUILayout.Toggle(AutoUpdate, "Auto Update");
                EditorGUILayout.BeginVertical("Box");
                lists.ForEach(o => DisplayList.Create(o.Value.Item1, MainEditor.serializedObject.FindProperty(o.Key), o.Value.Item2, MainEditor));
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            GUILayout.Space(5);
            return AutoUpdate;
        }

    }
}