using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.GUI;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace AddressableManager.AddressableSetter.Editor
{
    internal class OptionEditor<T> where T : ScriptableObject
    {

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


                EditorGUILayout.BeginVertical("Box");
                var header = new List<string> { "Search Under", "Exclude Type" };
                Headers(header, header.Count);
                EditorGUILayout.BeginHorizontal("box");
                Utilities.PropertyField(MainEditor, nameof(Setter.include), header, Setter.Reset);
                EnumFlagsField(MainEditor,header, Setter.Reset);
              
                Utilities.ApplyModifiedProperties(MainEditor);
                EditorGUILayout.EndHorizontal();

                


                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            GUILayout.Space(5);


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




