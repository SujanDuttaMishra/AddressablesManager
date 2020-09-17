using System;
using System.Collections.Generic;
using UnityEditor;

namespace AddressableManager.AddressableSetter
{
    [CustomEditor(typeof(Setter), true)]
    internal class MainEditor : UnityEditor.Editor
    {
        internal Setter Setter { get; private set; }
        private HeaderEditor<Setter> HeaderEditor { get; set; }
        private GroupsEditor<Setter> GroupsEditor { get; set; }
        private LabelsEditor LabelsEditor { get; set; }
        private ListsEditor<Setter> ListsEditor { get; set; }
        private AllAddressableSettersEditor AllAddressableSettersEditor { get; set; }
        private ButtonsEditor ButtonsEditor { get; set; }

        private void OnEnable()
        {
            if (target == null) return;
            Setter = (Setter)target;
            HeaderEditor = new HeaderEditor<Setter>(this);
            GroupsEditor = new GroupsEditor<Setter>(this);
            LabelsEditor = new LabelsEditor(this);
            ListsEditor = new ListsEditor<Setter>(this);
            AllAddressableSettersEditor = new AllAddressableSettersEditor(this);
            ButtonsEditor = new ButtonsEditor(this);
        }
        public override void OnInspectorGUI()
        {
            if (Setter == null) return;
            serializedObject.Update();
            if (!HeaderEditor.Init(out var assetPath)) return;
            GroupsEditor.Init();
            LabelsEditor.Init();
            Lists(assetPath);
            AllAddressableSettersEditor.Init();
            ButtonsEditor.Init();
            serializedObject.Update();
        }

        private void Lists(string assetPath)
        {
            EditorGUI.BeginChangeCheck();
            var status = $"Asset Found @ {assetPath} Asset Count : {Setter.AssetCount}";
            var autoUpdate = ListsEditor.Init(status, new Dictionary<string, Tuple<List<AData>, AutoLoad>>
            {
                {nameof(Setter.noAutoLoadList), new Tuple<List<AData>, AutoLoad>(Setter.noAutoLoadList, AutoLoad.None)},
                {nameof(Setter.onAwakeList), new Tuple<List<AData>, AutoLoad>(Setter.onAwakeList, AutoLoad.OnAwake)},
                {nameof(Setter.onStartList), new Tuple<List<AData>, AutoLoad>(Setter.onStartList, AutoLoad.OnStart)}
            });
            if (!EditorGUI.EndChangeCheck() || !autoUpdate) return;
            Setter.Update();
        }


        
    }
}
