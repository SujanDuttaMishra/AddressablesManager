using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AddressableManager.AddressableSetter.Editor
{
    internal class OptionEditor<T> where T : ScriptableObject
    {
        private bool IsFolderInAssetData => Setter.IsFolderInAssetData;
        private UnityEditor.Editor MainEditor { get; }
        private Setter Setter => (Setter)MainEditor.target;
        private bool Foldout { get; set; } = true;

       

        internal OptionEditor(UnityEditor.Editor editor)
        {
            MainEditor = editor;
            
        }
        internal void Init()
        {

            Foldout = EditorGUILayout.BeginFoldoutHeaderGroup(Foldout, "Options");
            if (Foldout)
            {
                Search();
                if(Setter.include == SearchOption.AllDirectories) AutoOrganize();

                Utilities.ApplyModifiedProperties(MainEditor);
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
            GUILayout.Space(5);

        }

        private void AutoOrganize()
        {
            EditorGUILayout.BeginVertical("Box");
            var header = new List<string> { "Auto Organize" };
            Headers(header, header.Count);
            header = IsFolderInAssetData? new List<string> { "Create UnityType Folders" } : new List<string> { "Move To AssetData", "Create UnityType Folders"};
            Headers(header, header.Count);
            EditorGUILayout.BeginHorizontal("box");
            if(!IsFolderInAssetData) Utilities.PropertyField(MainEditor, nameof(Setter.moveToAssetData), header, Setter.Organize);
            Utilities.PropertyField(MainEditor, nameof(Setter.createUnityTypeFolders), header, Setter.Organize);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }



        private void Search()
        {
            EditorGUILayout.BeginVertical("Box");
            var header = new List<string> { "Search Under", "Exclude Type" };
            Headers(header, header.Count);
            EditorGUILayout.BeginHorizontal("box");
            Utilities.PropertyField(MainEditor, nameof(Setter.include), header, Setter.Reset);
            EnumFlagsField(MainEditor, header, Setter.Reset);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void Headers(IEnumerable<string> headers, int column)
        {
            EditorGUILayout.BeginHorizontal();
            headers.ForEach(o => Utilities.Label(o, column));
            EditorGUILayout.EndHorizontal();
        }

        public void EnumFlagsField(UnityEditor.Editor mainEditor, List<string> header, Action action)
        {
            EditorGUI.BeginChangeCheck();
            Setter.excludeType = (AssetType)EditorGUILayout.EnumFlagsField(Setter.excludeType);
            Utilities.ApplyModifiedProperties(mainEditor);
            if (EditorGUI.EndChangeCheck()) action();
        }

    }


}




