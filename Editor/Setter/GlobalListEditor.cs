using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AddressableManager.AddressableSetter
{
    [CustomEditor(typeof(GlobalList), true)]
    internal class GlobalListEditor : UnityEditor.Editor
    {
        private GlobalList GlobalList { get; set; }
        private HeaderEditor<GlobalList> HeaderEditor { get; set; }
        private ListsEditor<GlobalList> ListsEditor { get; set; }
        private void OnEnable()
        {
            if (target == null) return;
            GlobalList = (GlobalList)target;
            HeaderEditor = new HeaderEditor<GlobalList>(this);
            ListsEditor = new ListsEditor<GlobalList>(this, true);
        }
        public override void OnInspectorGUI()
        {
            if (GlobalList == null) return;
            HeaderEditor.Init(out _);
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
            UpdateAdatas();
        }

        internal void Button()
        {
            var style = new GUIStyle(GUI.skin.button)
            {
                hover = { textColor = Color.green },
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter
            };
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.BeginHorizontal();

            style.hover.textColor = Color.yellow;
            UpdateButton = Utilities.Button("Update", style, 100, 40);
            if (UpdateButton)
            {
                UpdateAdatas();
            }

            style.hover.textColor = Color.red;
            ClearButton = Utilities.Button("Clear All", style, 100, 40);
            if (ClearButton) GlobalList.aDataList.Clear();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void UpdateAdatas()
        {
            for (int i = 0; i < GlobalList.aDataList.Count; i++)
            {
                var aData = GlobalList.aDataList[i];
                aData.Update(aData.autoLoad);
                // Debug.Log($" name {GlobalList.name} Count {GlobalList.aDataList.Count} aData Group {aData.group}");
            }
        }

        public bool UpdateButton { get; set; }
        public bool ClearButton { get; private set; }
    }
}
