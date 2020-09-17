using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AddressableManager.AddressableSetter.Editor
{
    internal class CreateDisplayList
    {
        private string[] UnloadOptions { get; }
        private string[] AutoLoadOptions { get; }
        private static string[] SceneNamesArray { get; set; }
        private int AutoLoadPopUpOnStart { get; set; }
        private int OnAwakeLoadWithSceneIndex { get; set; }
        private int UnloadOnAwakeIndex { get; set; }
        private int AutoLoadPopUpNone { get; set; }
        private int AutoLoadPopUpOnAwake { get; set; }
        private int OnStartLoadWithSceneIndex { get; set; }
        private int UnloadOnStartIndex { get; set; }
        private int LoadWithSceneIndex { get; set; }
        private bool ApplyToAllButton { get; set; }
        private AutoLoad AutoLoadIndexValue { get; set; }
        private Unload UnloadIndexValue { get; set; }

        internal CreateDisplayList()
        {
            UnloadOptions = Enum.GetNames(typeof(Unload));
            AutoLoadOptions = Enum.GetNames(typeof(AutoLoad));
            SceneNamesArray = Utilities.SceneNames();
        }

        internal void Create(IReadOnlyList<AData> list, SerializedProperty serializedProperty, AutoLoad autoLoadLabel, UnityEditor.Editor mainEditor)
        {
            mainEditor.serializedObject.Update();
            EditorGUILayout.BeginVertical("Box");
            var column = Headers(list, autoLoadLabel).Length;
            EditorGUILayout.BeginHorizontal("Box");
            ApplyToAllButton = Utilities.Button("Apply To All", column);
            switch (autoLoadLabel)
            {
                case AutoLoad.None:

                    AutoLoadPopUpNone = EditorGUILayout.Popup(AutoLoadPopUpNone, AutoLoadOptions, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth / column));
                    AutoLoadIndexValue = (AutoLoad)Enum.Parse(typeof(AutoLoad), AutoLoadOptions[AutoLoadPopUpNone]);
                    LoadWithSceneIndex = 0;
                    UnloadIndexValue = Unload.None;
                    break;

                case AutoLoad.OnStart:
                    AutoLoadPopUpOnStart = EditorGUILayout.Popup(AutoLoadPopUpOnStart, AutoLoadOptions, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth / column));
                    AutoLoadIndexValue = (AutoLoad)Enum.Parse(typeof(AutoLoad), AutoLoadOptions[AutoLoadPopUpOnStart]);

                    OnStartLoadWithSceneIndex = EditorGUILayout.Popup(OnStartLoadWithSceneIndex, SceneNamesArray, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth / column));
                    LoadWithSceneIndex = OnStartLoadWithSceneIndex;

                    UnloadOnStartIndex = EditorGUILayout.Popup(UnloadOnStartIndex, UnloadOptions, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth / column));
                    UnloadIndexValue = (Unload)Enum.Parse(typeof(Unload), UnloadOptions[UnloadOnStartIndex]);

                    break;
                case AutoLoad.OnAwake:

                    AutoLoadPopUpOnAwake = EditorGUILayout.Popup(AutoLoadPopUpOnAwake, AutoLoadOptions, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth / column));
                    AutoLoadIndexValue = (AutoLoad)Enum.Parse(typeof(AutoLoad), AutoLoadOptions[AutoLoadPopUpOnAwake]);

                    OnAwakeLoadWithSceneIndex = EditorGUILayout.Popup(OnAwakeLoadWithSceneIndex, SceneNamesArray, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth / column));
                    LoadWithSceneIndex = OnAwakeLoadWithSceneIndex;

                    UnloadOnAwakeIndex = EditorGUILayout.Popup(UnloadOnAwakeIndex, UnloadOptions, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth / column));
                    UnloadIndexValue = (Unload)Enum.Parse(typeof(Unload), UnloadOptions[UnloadOnAwakeIndex]);

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(autoLoadLabel), autoLoadLabel, null);
            }
            EditorGUILayout.EndHorizontal();

            if (list.Count > 0)
            {
                for (var i = 0; i < list.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    var item = serializedProperty.GetArrayElementAtIndex(i);
                    var obj = item.FindPropertyRelative(Constants.Obj);
                    var unload = item.FindPropertyRelative(Constants.Unload);
                    var autoLoad = item.FindPropertyRelative(Constants.AutoLoad);

                    EditorGUILayout.PropertyField(obj, GUIContent.none, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth / column));
                    EditorGUILayout.PropertyField(autoLoad, GUIContent.none, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth / column));

                    if (autoLoadLabel != AutoLoad.None)
                    {
                        list[i].loadOnSceneIndex = EditorGUILayout.Popup(list[i].loadOnSceneIndex, SceneNamesArray, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth / column));
                        EditorGUILayout.PropertyField(unload, GUIContent.none, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth / column));
                    }

                    if (ApplyToAllButton)
                    {
                        list[i].loadOnSceneIndex = LoadWithSceneIndex;
                        list[i].unload = UnloadIndexValue;
                        list[i].autoLoad = AutoLoadIndexValue;
                    }

                    serializedProperty.serializedObject.ApplyModifiedProperties();
                    serializedProperty.serializedObject.Update();

                    EditorGUILayout.EndHorizontal();
                }
            }
            mainEditor.serializedObject.ApplyModifiedProperties();
            mainEditor.serializedObject.Update();
            EditorGUILayout.EndVertical();
        }
        private static string[] Headers(IReadOnlyCollection<AData> list, AutoLoad autoLoadLabel)
        {
            string[] headers;
            switch (autoLoadLabel)
            {
                case AutoLoad.OnStart:
                    headers = new[] { $"{Constants.OnStart} : Total = {list.Count}", $"AutoLoad", $"load With Scene", $"Unload" };
                    break;
                case AutoLoad.OnAwake:
                    headers = new[] { $"{ Constants.OnAwake} : Total = {list.Count}", $"AutoLoad", $"load With Scene", $"Unload" };
                    break;
                case AutoLoad.None:
                    headers = new[] { $"No AutoLoad : Total = {list.Count}", $"AutoLoad" };
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(autoLoadLabel), autoLoadLabel, null);
            }
            EditorGUILayout.BeginHorizontal();
            Utilities.Labels(headers, headers.Length);
            EditorGUILayout.EndHorizontal();
            return headers;
        }
    }
}