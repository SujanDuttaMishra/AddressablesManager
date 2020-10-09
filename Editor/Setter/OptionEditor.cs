using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;
using static AddressableManager.AddressableSetter.Editor.Utilities;

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

            Foldout = BeginFoldoutHeaderGroup(Foldout, "Options");
            if (Foldout)
            {
                Search();
                if (Setter.include == SearchOption.AllDirectories) AutoOrganize();

                ApplyModifiedProperties(MainEditor);
            }

            EndFoldoutHeaderGroup();
            Space(5);

        }

        private void AutoOrganize()
        {
            BeginVertical("Box");
            var header = new List<string> { "Auto Organize" };
            Headers(header, header.Count);
            var processDirectory = Setter.ProcessDirectory(Setter.FolderPath, out _);
            header = IsFolderInAssetData ? processDirectory ? new List<string> { "Create UnityType Folders", "Delete Empty Folders" } : new List<string> { "Create UnityType Folders" } : new List<string> { "Move To AssetData", "Create UnityType Folders" };
            Headers(header, header.Count);
            BeginHorizontal("box");
            if (!IsFolderInAssetData) PropertyField(MainEditor, nameof(Setter.moveToAssetData), header, Setter.Organize);
            PropertyField(MainEditor, nameof(Setter.createUnityTypeFolders), header, Setter.Organize);


            if (processDirectory)
            {
                PropertyField(MainEditor, nameof(Setter.deleteEmptyDirectory), header, Setter.DeleteEmptyDirectory);
            }
            EndHorizontal();
            EndVertical();
        }



        private void Search()
        {
            BeginVertical("Box");
            var header = new List<string> { "Search Under", "Exclude Type" };
            Headers(header, header.Count);
            BeginHorizontal("box");
            PropertyField(MainEditor, nameof(Setter.include), header, Setter.Reset);
            EnumFlagsField(MainEditor, header, Setter.Reset);
            EndHorizontal();
            EndVertical();
        }

        private void Headers(IEnumerable<string> headers, int column)
        {
            BeginHorizontal();
            headers.ForEach(o => Label(o, column));
            EndHorizontal();
        }

        public void EnumFlagsField(UnityEditor.Editor mainEditor, List<string> header, Action action)
        {
            EditorGUI.BeginChangeCheck();
            Setter.excludeType = (AssetType)EditorGUILayout.EnumFlagsField(Setter.excludeType);
            ApplyModifiedProperties(mainEditor);
            if (EditorGUI.EndChangeCheck()) action();
        }

    }


}




