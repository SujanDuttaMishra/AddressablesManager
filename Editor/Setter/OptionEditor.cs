using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEditor;
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
                var header = new List<string>() {"Search Under", "Exclude Type"};
                Headers(header, header.Count);
                EditorGUILayout.BeginHorizontal("box");
                Utilities.PropertyField(MainEditor, nameof(Setter.include), header, Setter.Reset);
                Setter.excludeType = (AssetType)EditorGUILayout.EnumFlagsField(Setter.excludeType);
                Utilities.ApplyModifiedProperties(MainEditor);
                EditorGUILayout.EndHorizontal();

               

                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            GUILayout.Space(5);


        }

        


        private void Headers(IEnumerable<string> headers,int column)
        {
            EditorGUILayout.BeginHorizontal();
            headers.ForEach(o => Utilities.Label(o, column));
            EditorGUILayout.EndHorizontal();
        }



    }


}




