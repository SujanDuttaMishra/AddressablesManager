using UnityEditor;
using UnityEngine;

namespace AddressableManager.AddressableSetter.Editor
{
    internal class ButtonsEditor
    {
        private bool AddButton { get; set; }
        private bool UpdateButton { get; set; }
        private bool RemoveButton { get; set; }
        private MainEditor MainEditor { get; }
        private Setter Setter => MainEditor.Setter;
        internal ButtonsEditor(MainEditor editor) { MainEditor = editor; }
        internal void Init()
        {
            var style = new GUIStyle(GUI.skin.button)
            {
                hover = { textColor = Color.green },
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter
            };



            if (!Setter.ManageEntry.IsEntriesAdded(Setter.PathsToImport))
            {

                AddButton = Utilities.Button("Add", style, 100, 40);
                if (AddButton) Setter.Add();

            }
            else if (Setter.IsGroup)
            {
                EditorGUILayout.BeginVertical("Box");
                EditorGUILayout.BeginHorizontal();

                style.hover.textColor = Color.yellow;
                UpdateButton = Utilities.Button("Update", style, 100, 40);
                if (UpdateButton) Setter.Update();

                style.hover.textColor = Color.red;
                RemoveButton = Utilities.Button("Remove", style, 100, 40);
                if (RemoveButton)
                {
                    var setterGroupName = Setter.GroupName;
                    Setter.Remove();
                    AllAddressableSettersEditor.AllSettersList.RemoveAll(o => o == Setter || o.GroupName == Setter.GroupName);
                    Setter.ManageGroup.RemoveGroup(setterGroupName);
                    var assetObject = Utilities.GetAsset<Setter>(Setter.name) ?? Utilities.GetAsset<Setter>(setterGroupName);
                    if (assetObject != null) AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(assetObject));
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
        }


    }
}