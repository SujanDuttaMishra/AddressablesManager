using System.Collections.Generic;
using UnityEditor.AddressableAssets.Settings;
using UnityEditorInternal;
using UnityEngine;
using static AddressableManager.AddressableSetter.Editor.Utilities;
using static UnityEditor.EditorGUILayout;

namespace AddressableManager.AddressableSetter.Editor
{
    internal class ListDisplayEditor<T> where T : ScriptableObject
    {
        private int Count => AllSettersList.Count;
        public UnityEditor.Editor MainEditor { get; set; }
        private T Target => (T)MainEditor.target;
        private bool Foldout { get; set; } = true;
        public List<Setter> AllSettersList { get => AllSetters.settersList; set => AllSetters.settersList = value; }

        public ListDisplayEditor(UnityEditor.Editor editor)
        {
            MainEditor = editor;

        }

        internal void Init()
        {
           

            Foldout = BeginFoldoutHeaderGroup(Foldout, $" Global Setter");
            if (Foldout)
            {
                
                PingButton("All Setters List", AllSetters.Instance);

                var list = new ReorderableList(AllSettersList, typeof(AddressableAssetEntry), false, true, false, false)
                {
                    drawElementCallback = DrawEntry,
                    drawHeaderCallback = DrawHeader,
                    elementHeight = 30,
                };
                list.DoLayoutList();

            }
            EndFoldoutHeaderGroup();
            Space(5);

        }


        private void DrawHeader(Rect rect)
        {

            var totalAssets = 0;
            AllSettersList.ForEach(o => totalAssets += o.AssetCount);
            GUI.Label(rect, $"Total Folder Setters : {Count} |" +
                            $" Total OnAwake asset {GlobalOnAwakeList.Count} |" +
                            $" Total OnStart asset {GlobalOnStartList.Count} |" +
                            $" Total Assets : {totalAssets}");
        }

        private void DrawEntry(Rect rect, int index, bool isActive, bool isFocused)
        {

            var style = new GUIStyle { richText = true };
            var text = string.Empty;

            if (AllSettersList[index].AssetCount <= 0)
            {
                text = $" Asset :R:15; Not Added Yet! :Y;".Interpolate();
                Button(rect, index);
                GUI.Label(Position(rect, 110), text, style);
                return;
            }

            Button(rect, index);

            text = $" <color=grey> OnStart : <color=yellow> [{AllSettersList[index].onStartList.Count}] </color>" +
                       $"+ OnAwake : <color=yellow> [{ AllSettersList[index].onAwakeList.Count }] </color>" +
                       $"+ No AutoLoadStr: <color=yellow> [{AllSettersList[index].noAutoLoadList.Count}] </color> " +
                       $"= <color=green> {AllSettersList[index].AssetCount} </color> </color>";

            GUI.Label(Position(rect, 110), text, style);

        }

        private void Button(Rect rect, int index)
        {
            PingButton(rect, AllSettersList[index].GroupName, AllSettersList[index], 100, 29);


        }

        private static Rect Position(Rect rect, int offset) => new Rect(rect.position + new Vector2(offset, 0), rect.size);
    }
}