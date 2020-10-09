using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static AddressableManager.AddressableSetter.Editor.Utilities;
using static UnityEditor.EditorGUILayout;


namespace AddressableManager.AddressableSetter.Editor
{
    [CustomEditor(typeof(GlobalList), true)]
    internal class GlobalListEditor : UnityEditor.Editor
    {
        public bool UpdateButton { get; set; }
        public bool ClearButton { get; private set; }
        private GlobalList GlobalList { get; set; }
        private ListsEditor<GlobalList> ListsEditor { get; set; }
        private void OnEnable()
        {
            if (target == null) return;
            GlobalList = (GlobalList)target;
            ListsEditor = new ListsEditor<GlobalList>(this, true);
        }
        public override void OnInspectorGUI()
        {
            if (GlobalList == null) return;

            GlobalList.RemoveWithoutSetter();
            Lists();
            Button();
        }
        private void Lists()
        {
            EditorGUI.BeginChangeCheck();

            var autoLoad = GlobalList.name == Constants.OnAwake ? AutoLoad.OnAwake : AutoLoad.OnStart;
            var status = $" Asset Count : {GlobalList.aDataList.Count}";
            var autoUpdate = ListsEditor.Init(status, new Dictionary<string, Tuple<List<AData>, AutoLoad>>
            {
                {nameof(GlobalList.aDataList), new Tuple<List<AData>, AutoLoad>(GlobalList.aDataList, autoLoad)}
            });

            if (!EditorGUI.EndChangeCheck() || !autoUpdate) return;
            UpdateADatas();
        }

        internal void Button()
        {
            var style = new GUIStyle(GUI.skin.button)
            {
                hover = { textColor = Color.green },
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter
            };
            BeginVertical("Box");
            BeginHorizontal();

            style.hover.textColor = Color.yellow;
            UpdateButton = UButton("Update", style, 100, 40);
            if (UpdateButton)
            {
                UpdateADatas();
            }

            style.hover.textColor = Color.red;
            ClearButton = UButton("Clear All", style, 100, 40);
            if (ClearButton) GlobalList.aDataList.Clear();

            EndHorizontal();
            EndVertical();
        }

        private void UpdateADatas()
        {
            for (var i = 0; i < GlobalList.aDataList.Count; i++)
            {
                var aData = GlobalList.aDataList[i];
                aData.Update(aData.autoLoad);

                if (aData.entry.MainAsset == null) GlobalList.aDataList.Remove(aData);
            }
        }


    }
}
