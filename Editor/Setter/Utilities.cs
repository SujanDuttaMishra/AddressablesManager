using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;


namespace AddressableManager.AddressableSetter.Editor
{
    internal static class Utilities
    {
        internal static bool Button(string buttonName, int column) => GUILayout.Button(buttonName, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth / column));
        internal static GUILayoutOption MaxWidth(int column) => GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth / column);
        internal static bool Button(string buttonName, GUIStyle style, int width, int height) => GUILayout.Button(buttonName, style, GUILayout.Width(width), GUILayout.Height(height));
        internal static void Label(string content, int column = 1) => EditorGUILayout.LabelField(FUpper(content), GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth / column));
        internal static string FUpper(string content) => Char.ToUpper(content[0]) + ((content.Length > 1) ? content.Substring(1) : String.Empty);
        internal static void Labels(string[] contents, int column = 1) => contents.ForEach(o => Label(o, column));
        internal static string[] SceneNames()
        {
            var sceneNumber = SceneManager.sceneCountInBuildSettings;
            var sceneNames = new List<string> { "None" };
            for (var i = 0; i < sceneNumber; i++) sceneNames.Add(Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i)));
            sceneNames.Add("Any");
            return sceneNames.ToArray();
        }
        internal static void ShowProperty<T>(SerializedProperty serializedProperty, List<T> list) where T : ScriptableObject
        {
            if (list.Count <= 0) return;
            EditorGUILayout.BeginVertical("box");
            for (var i = 0; i < list.Count; i++)
            { EditorGUILayout.PropertyField(serializedProperty.GetArrayElementAtIndex(i), GUIContent.none); }
            EditorGUILayout.EndVertical();
        }
        internal static void PropertyField(UnityEditor.Editor mainEditor, string path, int column) =>
            EditorGUILayout.PropertyField(mainEditor.serializedObject.FindProperty(path), GUIContent.none, MaxWidth(column));
        internal static void ApplyModifiedProperties(UnityEditor.Editor mainEditor) { if (mainEditor.serializedObject.hasModifiedProperties) mainEditor.serializedObject.ApplyModifiedProperties(); }
        public static List<string> GetAssetPathsFromLocation<T>(string folderSetterName, List<string> toExclude, SearchOption allDirectories) where T : ScriptableObject
        {
            var setterPath = GetAssetPath<T>(folderSetterName);
            if (IsNullEmptyWhiteSpace(setterPath)) return new List<string>();

            setterPath = Path.GetDirectoryName(setterPath);
            var pathsToImport = new List<string>();
            var isDirectory = Directory.Exists(setterPath);
            if (!isDirectory) return pathsToImport;
            var filesToAdd = Directory.EnumerateFiles(setterPath, "*", allDirectories);


            foreach (var file in filesToAdd)
            {
                if (!toExclude.Any(o => file.EndsWith(o) || o == file)) pathsToImport.Add(file.Replace('\\', '/'));

            }

            return pathsToImport;
        }
        internal static T GetOrCreateInstances<T>(string assetName, bool getFirstOrDefault = true) where T : ScriptableObject =>
            GetAsset(assetName, out List<T> allAssetOfType) ?? allAssetOfType.Count > 0 && getFirstOrDefault ? allAssetOfType.FirstOrDefault() : CreateNew<T>(assetName, Constants.AddressableAssetsDataPath);
        internal static T GetOrCreateInstances<T>(string assetName) where T : ScriptableObject => GetAsset<T>(assetName) ?? CreateNew<T>(assetName, Constants.AddressableAssetsDataPath);
        internal static T GetOrCreateInstance<T>(string assetDataPath, string parentFolder, string newFolderName, string fileName, out string path) where T : ScriptableObject
        {
            var asset = GetAsset<T>(fileName);
            if (asset != null) { path = AssetDatabase.GetAssetPath(asset); return asset; }
            path = GetOrCreateDirectory(assetDataPath, parentFolder, newFolderName);
            var instance = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(instance, AssetDatabase.GenerateUniqueAssetPath(path + $"/{fileName}.asset"));
            AssetDatabase.SaveAssets();
            return GetAsset<T>(fileName);
        }
        public static T CreateNew<T>(string assetName, string dataPath) where T : ScriptableObject
        {
            if (IsNullEmptyWhiteSpace(assetName)) throw new Exception("name to create is null or blank");
            var defaultPath = dataPath + $"/{assetName}.asset";
            var groupTemplatePath = AssetDatabase.GenerateUniqueAssetPath(Constants.AddressableAssetsDataPath + $"/AssetGroupTemplates/{assetName}.asset");
            var path = typeof(T) == typeof(AddressableAssetGroupTemplate) ? groupTemplatePath : defaultPath;
            var asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            var instance = GetAsset<T>(assetName);
            AssetDatabase.SaveAssets();
            return instance;
        }
        public static string GetOrCreateDirectory(string assetDataPath, string parentFolder, string newFolderName) => !AssetDatabase.IsValidFolder(assetDataPath) ? AssetDatabase.GUIDToAssetPath(AssetDatabase.CreateFolder(parentFolder, newFolderName)) : assetDataPath;
        public static T GetAsset<T>(string assetName, out List<T> allAssetOfType) where T : ScriptableObject => (allAssetOfType = Resources.FindObjectsOfTypeAll<T>().ToList()).Find(o => o.name == assetName);
        public static T GetAsset<T>(string assetName) where T : ScriptableObject => GetAssets<T>().Find(o => o.name == assetName);
        public static List<T> GetAssets<T>() where T : ScriptableObject => Resources.FindObjectsOfTypeAll<T>().ToList();

        public static string GetAssetPath<T>(string fileName) where T : ScriptableObject => AssetDatabase.GetAssetPath(GetAsset<T>(fileName));
        public static string GetAssetPath<T>(T asset) where T : ScriptableObject => AssetDatabase.GetAssetPath(asset);

        public static bool IsLabelIn(string label, AddressableAssetSettings settings, out List<string> labels) => (labels = LabelsToStringList(settings)).Contains(label);
        public static bool IsLabelIn(string label, AddressableAssetSettings settings) => LabelsToStringList(settings).Contains(label);
        public static bool IsLabelIn(string label, List<AssetLabelReference> references) => references.Any(o => o.labelString == label);
        public static AutoLoad GetAutoLoad(AddressableAssetEntry o) => o.labels.Contains(Constants.OnAwake) ? AutoLoad.OnAwake : o.labels.Contains(Constants.OnStart) ? AutoLoad.OnStart : AutoLoad.None;
        public static void RemoveLabelFrom(string label, AddressableAssetSettings settings)
        { if (label != Constants.OnStart && label != Constants.OnAwake && IsLabelIn(label, settings)) settings.RemoveLabel(label); }
        public static List<string> LabelsToStringList(AddressableAssetSettings settings) => settings.GetLabels();
        public static void RemoveAdataFrom(List<AData> list, List<AData> globalList) => list?.ForEach(o => RemoveAdataFrom(o, globalList));
        public static void RemoveAdataFrom(AData aData, List<AData> list) { if (FindAdataIn(list, aData, out var outAdata)) list.Remove(outAdata); }
        public static void AddAdataTo(AData aData, List<AData> list) { if (!FindAdataIn(list, aData)) list.Add(aData); }
        public static bool FindAdataIn(List<AData> list, AData aData, out AData outAData) => (outAData = list.Find(o => CompareOrdinal(aData, o))) != null;
        public static bool FindAdataIn(List<AData> list, AData aData) => (list.Find(o => CompareOrdinal(aData, o))) != null;
        public static bool CompareOrdinal(AData aData, AData o) => CompareOrdinal(aData.ID, o.ID);
        public static bool CompareOrdinal(string a, string b) => String.CompareOrdinal(a, b) == 0;
        public static bool IsNullEmptyWhiteSpace(string str) => String.IsNullOrEmpty(str) || String.IsNullOrWhiteSpace(str);
        public static List<AData> GlobalOnAwakeList => LoadAssetFromPackagePath<GlobalList>(Constants.AddressablesManagerSettings, Constants.GlobalOnAwakeList, out var globalList) ? globalList.aDataList : GlobalList.GetOrCreateInstance(Constants.GlobalOnAwakeList).aDataList;
        public static List<AData> GlobalOnStartList => LoadAssetFromPackagePath<GlobalList>(Constants.AddressablesManagerSettings, Constants.GlobalOnStartList, out var globalList) ? globalList.aDataList : GlobalList.GetOrCreateInstance(Constants.GlobalOnStartList).aDataList;
        public static List<Setter> SettersList => LoadAssetFromPackagePath<SetterList>(Constants.AddressablesManagerSettings, nameof(SetterList), out var globalList) ? globalList.settersList : GetOrCreateInstances<SetterList>(nameof(SetterList)).settersList;
        public static bool LoadAssetFromPackagePath<T>(string packagesPath, string assetName, out T outAsset) where T : ScriptableObject => (outAsset = (T)AssetDatabase.LoadAssetAtPath($"{packagesPath}{assetName}.asset", typeof(T))) != null;
        internal static Setter GetSetter(string groupName) => GetAsset<Setter>(groupName);
        public static AddressableAssetSettings DefaultAssetSettings => AddressableAssetSettingsDefaultObject.Settings;
        public static void PingButton<T>(string buttonName, T asset) where T : ScriptableObject
        {
            var style = new GUIStyle(GUI.skin.button)
            {
                hover = { textColor = Color.green },
                fontSize = 11
            };

            if (String.IsNullOrEmpty(buttonName)) return;
            if (Button(buttonName, style, 100, 25) && asset != null) EditorGUIUtility.PingObject(asset);
        }
        public static void PropertyField(UnityEditor.Editor mainEditor, string path, GUIContent content, int column) =>
            EditorGUILayout.PropertyField(mainEditor.serializedObject.FindProperty(path), content, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth / column));
        public static void PropertyField(UnityEditor.Editor mainEditor, string path, List<string> header, Action action)
        {
            EditorGUI.BeginChangeCheck();
            PropertyField(mainEditor, path, GUIContent.none, header.Count);
            ApplyModifiedProperties(mainEditor);
            if (EditorGUI.EndChangeCheck()) action();

        }


    }
}




