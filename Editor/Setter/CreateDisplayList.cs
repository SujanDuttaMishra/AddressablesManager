using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static AddressableManager.AddressableSetter.Editor.Utilities;
using static AddressableManager.Constants;
using static UnityEditor.EditorGUILayout;
using static UnityEditor.EditorGUIUtility;



namespace AddressableManager.AddressableSetter.Editor
{
    internal class CreateDisplayList
    {
        private string[] UnloadOptions { get; }
        private string[] AutoLoadOptions { get; }
        private static string[] SceneNamesArray { get; set; }
        private int AutoLoadPopUpOnStart { get; set; } = 1;
        private int OnAwakeLoadWithSceneIndex { get; set; }
        private int UnloadOnAwakeIndex { get; set; }
        private int AutoLoadPopUpNone { get; set; }
        private int AutoLoadPopUpOnAwake { get; set; } = 2;
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
            SceneNamesArray = SceneNames();
        }

        internal void Create(IReadOnlyList<AData> list, SerializedProperty serializedProperty, AutoLoad autoLoadLabel, UnityEditor.Editor mainEditor)
        {
            if (list.Count <= 0) return;
            mainEditor.serializedObject.Update();
            BeginVertical("Box");

            var column = Headers(list, autoLoadLabel).Length;
            BeginHorizontal("Box");
            ApplyToAllButton = UButton("Apply To All", column);
            switch (autoLoadLabel)
            {
                case AutoLoad.None:

                    AutoLoadPopUpNone = Popup(AutoLoadPopUpNone, AutoLoadOptions, GUILayout.MaxWidth(currentViewWidth / column));
                    AutoLoadIndexValue = (AutoLoad)Enum.Parse(typeof(AutoLoad), AutoLoadOptions[AutoLoadPopUpNone]);
                    LoadWithSceneIndex = 0;
                    UnloadIndexValue = Unload.None;
                    break;

                case AutoLoad.OnStart:
                    AutoLoadPopUpOnStart = Popup(AutoLoadPopUpOnStart, AutoLoadOptions, GUILayout.MaxWidth(currentViewWidth / column));
                    AutoLoadIndexValue = (AutoLoad)Enum.Parse(typeof(AutoLoad), AutoLoadOptions[AutoLoadPopUpOnStart]);

                    OnStartLoadWithSceneIndex = Popup(OnStartLoadWithSceneIndex, SceneNamesArray, GUILayout.MaxWidth(currentViewWidth / column));
                    LoadWithSceneIndex = OnStartLoadWithSceneIndex;

                    UnloadOnStartIndex = Popup(UnloadOnStartIndex, UnloadOptions, GUILayout.MaxWidth(currentViewWidth / column));
                    UnloadIndexValue = (Unload)Enum.Parse(typeof(Unload), UnloadOptions[UnloadOnStartIndex]);

                    break;
                case AutoLoad.OnAwake:

                    AutoLoadPopUpOnAwake = Popup(AutoLoadPopUpOnAwake, AutoLoadOptions, GUILayout.MaxWidth(currentViewWidth / column));
                    AutoLoadIndexValue = (AutoLoad)Enum.Parse(typeof(AutoLoad), AutoLoadOptions[AutoLoadPopUpOnAwake]);

                    OnAwakeLoadWithSceneIndex = Popup(OnAwakeLoadWithSceneIndex, SceneNamesArray, GUILayout.MaxWidth(currentViewWidth / column));
                    LoadWithSceneIndex = OnAwakeLoadWithSceneIndex;

                    UnloadOnAwakeIndex = Popup(UnloadOnAwakeIndex, UnloadOptions, GUILayout.MaxWidth(currentViewWidth / column));
                    UnloadIndexValue = (Unload)Enum.Parse(typeof(Unload), UnloadOptions[UnloadOnAwakeIndex]);

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(autoLoadLabel), autoLoadLabel, null);
            }
            EndHorizontal();

            if (list.Count > 0)
            {
                for (var i = 0; i < list.Count; i++)
                {
                    BeginHorizontal();
                    var item = serializedProperty.GetArrayElementAtIndex(i);
                    var obj = item.FindPropertyRelative(Obj);
                    var unload = item.FindPropertyRelative(UnloadStr);
                    var autoLoad = item.FindPropertyRelative(AutoLoadStr);

                    PropertyField(obj, GUIContent.none, GUILayout.MaxWidth(currentViewWidth / column));
                    PropertyField(autoLoad, GUIContent.none, GUILayout.MaxWidth(currentViewWidth / column));

                    if (autoLoadLabel != AutoLoad.None)
                    {
                        list[i].loadOnSceneIndex = Popup(list[i].loadOnSceneIndex, SceneNamesArray, GUILayout.MaxWidth(currentViewWidth / column));
                        PropertyField(unload, GUIContent.none, GUILayout.MaxWidth(currentViewWidth / column));
                    }

                    if (ApplyToAllButton)
                    {
                        list[i].loadOnSceneIndex = LoadWithSceneIndex;
                        list[i].unload = UnloadIndexValue;
                        list[i].autoLoad = AutoLoadIndexValue;
                    }

                    serializedProperty.serializedObject.ApplyModifiedProperties();
                    serializedProperty.serializedObject.Update();

                    EndHorizontal();
                }
            }
            mainEditor.serializedObject.ApplyModifiedProperties();
            mainEditor.serializedObject.Update();
            EndVertical();
        }
        private static string[] Headers(IReadOnlyCollection<AData> list, AutoLoad autoLoadLabel)
        {
            string[] headers;
            string buttonName;
            GlobalList asset;
            switch (autoLoadLabel)
            {
                case AutoLoad.OnStart:
                    headers = new[] { $"{OnStart} : Total = {list.Count}", $"AutoLoadStr", $"load With Scene", $"UnloadStr" };
                    buttonName = "Global OnStart";
                    asset = GetAsset<GlobalList>(Constants.GlobalOnStartList);
                    break;
                case AutoLoad.OnAwake:
                    headers = new[] { $"{ OnAwake} : Total = {list.Count}", $"AutoLoadStr", $"load With Scene", $"UnloadStr" };
                    buttonName = "Global OnAwake";
                    asset = GetAsset<GlobalList>(Constants.GlobalOnAwakeList);
                    break;
                case AutoLoad.None:
                    headers = new[] { $"No AutoLoadStr : Total = {list.Count}", $"AutoLoadStr" };
                    buttonName = string.Empty;
                    asset = null;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(autoLoadLabel), autoLoadLabel, null);
            }



            BeginVertical();

            PingButton(buttonName, asset);

            BeginHorizontal("Box");
            Labels(headers, headers.Length);
            EndHorizontal();

            EndVertical();

            return headers;
        }
    }
}