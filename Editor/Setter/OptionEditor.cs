using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AddressableManager.AddressableSetter.Editor
{
    internal class OptionEditor<T> where T : ScriptableObject
    {
        private UnityEditor.Editor MainEditor { get; }
        private Setter Setter => (Setter)MainEditor.target;
        private int Column { get; } = 4;
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

                Utilities.PropertyField(MainEditor,nameof(Setter.include), GUIContent.none, 1);

                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            GUILayout.Space(5);
        }

        private void Headers(IEnumerable<string> headers)
        {
            EditorGUILayout.BeginHorizontal();
            headers.ForEach(o => Utilities.Label(o, Column));
            EditorGUILayout.EndHorizontal();
        }



    }
}