using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditorInternal;
using UnityEngine;

namespace AddressableManager.AddressableSetter.Editor
{
    internal class AllAddressableSettersEditor
    {
        private int m_count;
        public MainEditor MainEditor { get; set; }
        private Setter Setter => MainEditor.Setter;
        private bool AllFolderAddressable { get; set; }
        public static List<Setter> AllSettersList { get;} = Utilities.SettersList;

        public AllAddressableSettersEditor(MainEditor editor)
        {
            MainEditor = editor;
           
        }

        internal void Init()
        {
            m_count = AllSettersList.Count;
           
            if (m_count <= 1) return;

            AllFolderAddressable = EditorGUILayout.BeginFoldoutHeaderGroup(AllFolderAddressable, $" Global Setters {m_count}");
            if (AllFolderAddressable)
            {
                Utilities.PingButton("All Setters List", Utilities.GetAsset<SetterList>(nameof(SetterList)));
                if (m_count <= 0 || (m_count == 1 && AllSettersList[0] == Setter)) return;

                var allSettersList = new ReorderableList(AllSettersList, typeof(AddressableAssetEntry), false, true, false, false)
                { drawElementCallback = DrawEntry, drawHeaderCallback = DrawHeader };

                allSettersList.DoLayoutList();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            GUILayout.Space(5);
        }


        private static void DrawHeader(Rect rect)
        {
            EditorGUILayout.BeginHorizontal("box");

            GUI.Label(rect, "Asset Entries");

            EditorGUILayout.EndVertical();
        }

        private static void DrawEntry(Rect rect, int index, bool isActive, bool isFocused)
        {
            GUI.Label(rect, AllSettersList[index].GroupName);

        }


    }
}