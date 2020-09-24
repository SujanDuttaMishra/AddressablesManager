using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditorInternal;
using UnityEngine;

namespace AddressableManager.AddressableSetter.Editor
{
    internal class GlobalSettersEditor<T> where T : ScriptableObject
    {
        private int m_count;
        public UnityEditor.Editor MainEditor { get; set; }
        private T Target => (T)MainEditor.target;
        private bool Foldout { get; set; } = true;
        public List<Setter> AllSettersList { get; } = Utilities.SettersList;

        public GlobalSettersEditor(UnityEditor.Editor editor)
        {
            MainEditor = editor;

        }

        internal void Init()
        {
            m_count = AllSettersList.Count;

            if (m_count <= 1) return;

            Foldout = EditorGUILayout.BeginFoldoutHeaderGroup(Foldout, $" Global Setter");
            if (Foldout)
            {

                var isOnlySetter = Target != null && AllSettersList[0] == Target;

                Utilities.PingButton("All Setters List", Utilities.GetAsset<SetterList>(nameof(SetterList)));

                if (m_count <= 0 || m_count == 1 && isOnlySetter) return;

                var allSettersList = new ReorderableList(AllSettersList, typeof(AddressableAssetEntry), false, true, false, false)
                {
                    drawElementCallback = DrawEntry,
                    drawHeaderCallback = DrawHeader,
                    elementHeight = 30,

                };
                allSettersList.DoLayoutList();

            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            GUILayout.Space(5);
        }


        private void DrawHeader(Rect rect)
        {
            var totalAssets = 0;
            AllSettersList.ForEach(o => totalAssets += o.AssetCount);
            GUI.Label(rect, $"Total Folder Setters : {m_count} | Total OnAwake asset {Utilities.GlobalOnAwakeList.Count} | Total OnStart asset {Utilities.GlobalOnStartList.Count} | Total Addressable Asset : {totalAssets}");
        }

        private void DrawEntry(Rect rect, int index, bool isActive, bool isFocused)
        {
            var style = new GUIStyle { richText = true };
            Utilities.PingButton(rect, AllSettersList[index].GroupName, AllSettersList[index], 100, 29);
            
            var text = $" <color=grey> OnStart : <color=yellow> [{AllSettersList[index].onStartList.Count}] </color>" +
                       $"+ OnAwake : <color=yellow> [{ AllSettersList[index].onAwakeList.Count }] </color>" +
                       $"+ No AutoLoad: <color=yellow> [{AllSettersList[index].noAutoLoadList.Count}] </color> " +
                       $"= <color=green> {AllSettersList[index].AssetCount} </color> </color>";

            GUI.Label(Position(rect, 110),text, style) ;

        }

        private static Rect Position(Rect rect, int offset) => new Rect(rect.position + new Vector2(offset, 0), rect.size);
    }
}