using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AddressableManager.AddressableSetter.Editor
{
    internal class LabelsEditor
    {
        public Setter Setter => MainEditor.Setter;
        public static LabelsEditor Instance { get; set; }
        public MainEditor MainEditor { get; set; }
        private bool LabelSettings { get; set; } = true;
        private bool FolderNameIsGroupName { get; set; }
        private bool RemoveFolderLabelButton { get; set; }
        private bool AddFolderLabelButton { get; set; } = true;
        private bool RemoveGroupLabelButton { get; set; }
        private bool AddGroupLabelButton { get; set; } = true;
        private bool IsValidCustomLabel { get; set; }
        private bool ApplyButton { get; set; }
        private bool RemoveLabelButton { get; set; }
        private const string Status = "Label";
        private List<string> CustomLabelList { get => Setter.customLabelList; set => Setter.customLabelList = value; }
        public int GroupsNameIndex { get; private set; }
        public LabelsEditor(MainEditor editor) { MainEditor = editor; }
        internal void Init()
        {
            FolderNameIsGroupName = Utilities.CompareOrdinal(Setter.name, Setter.newGroupName);

            LabelSettings = EditorGUILayout.BeginFoldoutHeaderGroup(LabelSettings, Status + " Settings");
            if (LabelSettings)
            {
                var headerCount = FolderNameIsGroupName ? 3 : 4;
                EditorGUILayout.BeginVertical("Box");
                EditorGUILayout.BeginHorizontal();

                var header = FolderNameIsGroupName ? 
                    new[] { "Set " + nameof(Setter.autoLoad), "[+/-] FolderLabel", "[+/-] " + nameof(Setter.customLabel) } :
                    new[] { "Set " + nameof(Setter.autoLoad), "[+/-] FolderLabel", "[+/-] GroupLabel", "[+/-] " + nameof(Setter.customLabel) };

                Utilities.Labels(header, headerCount);

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                AutoLoadLabel(headerCount);
                FolderNameLabel(headerCount);
                GroupNameLabel(headerCount);
                CustomLabel(headerCount);
                EditorGUILayout.EndHorizontal();
                SetCustomLabel();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            GUILayout.Space(5);


            LabelToApply();
        }

        private void AutoLoadLabel(int headerCount)
        {
            EditorGUI.BeginChangeCheck();

            PropertyField(nameof(Setter.autoLoad), GUIContent.none, headerCount);
            MainEditor.serializedObject.ApplyModifiedProperties();
            MainEditor.serializedObject.Update();

            if (!EditorGUI.EndChangeCheck()) return;
            Setter.ManageLabel.ManageAutoLoad();

            if (Setter.AssetCount <= 0) return;

            MainEditor.serializedObject.ApplyModifiedProperties();
            MainEditor.serializedObject.Update();
            Setter.Add();


        }

        private void FolderNameLabel(int headerCount)
        {
            EditorGUI.BeginChangeCheck();

            var labelExist = Setter.labelReferences?.Count > 0 && Setter.labelReferences.Any(o => o.labelString == Setter.name);

            if (labelExist)
            {
                RemoveFolderLabelButton = GUILayout.Button("Remove", GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth / headerCount));
                if (RemoveFolderLabelButton) Setter.ManageLabel.RemoveLabel(Setter.name);
            }
            else
            {
                AddFolderLabelButton = GUILayout.Button("Add", GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth / headerCount));
                if (AddFolderLabelButton) Setter.ManageLabel.AddLabel(Setter.name);
            }

            if (!EditorGUI.EndChangeCheck()) return;
            if (Setter.ManageEntry.EntriesAdded) Setter.ManageEntry.RefreshEntryLabels();

        }

        private void GroupNameLabel(int headerCount)
        {

            if (FolderNameIsGroupName) return;

            EditorGUI.BeginChangeCheck();

            var labelExist = Setter.assetSettings != null && Setter.assetSettings.GetLabels().Any(o => o == Setter.newGroupName);

            if (labelExist)
            {
                RemoveGroupLabelButton = GUILayout.Button("Remove", GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth / headerCount));
                if (RemoveGroupLabelButton) Setter.ManageLabel.RemoveLabel(Setter.newGroupName);
            }
            else
            {
                AddGroupLabelButton = GUILayout.Button("Add", GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth / headerCount));
                if (AddGroupLabelButton) Setter.ManageLabel.AddLabel(Setter.newGroupName);
            }

            if (!EditorGUI.EndChangeCheck()) return;
            if (Setter.ManageEntry.EntriesAdded) Setter.ManageEntry.RefreshEntryLabels();
        }

        private void CustomLabel(int headerCount)
        {
            PropertyField(nameof(Setter.customLabel), GUIContent.none, headerCount);
            IsValidCustomLabel = !string.IsNullOrEmpty(Setter.customLabel);
            if (IsValidCustomLabel) ApplyButton = GUILayout.Button("Apply", GUILayout.MaxWidth(100));
            MainEditor.serializedObject.ApplyModifiedProperties();

        }
        private void SetCustomLabel()
        {

            EditorGUILayout.BeginHorizontal();


            if (ApplyButton)
            {
                if (!CustomLabelList.Contains(Setter.customLabel))
                {
                    CustomLabelList?.Add(Setter.customLabel);
                }
                MainEditor.serializedObject?.Update();

                for (var i = 0; i < CustomLabelList.Count; i++)
                {
                    var customLabel = CustomLabelList[i];
                    Setter.ManageLabel.AddLabel(customLabel);
                }


            }

            if (CustomLabelList?.Count > 0)
            {
                RemoveLabelButton = GUILayout.Button("Remove Label", GUILayout.MinWidth(50), GUILayout.MaxWidth(100));
                var array = CustomLabelList.ToArray();
                GroupsNameIndex = EditorGUILayout.Popup(GroupsNameIndex, array, GUILayout.MinWidth(10), GUILayout.MaxWidth(150));
                var label = array[GroupsNameIndex];
                if (RemoveLabelButton)
                {
                    Setter.ManageLabel.RemoveLabel(label);
                }
            }

            EditorGUILayout.EndHorizontal();


        }

        private void LabelToApply()
        {

            EditorGUILayout.BeginVertical("Box");
            var list = Setter.labelReferences;
            Utilities.Label($"Labels To Apply : {list.Count}");

            var property = MainEditor.serializedObject.FindProperty(nameof(Setter.labelReferences));
            property.serializedObject.Update();

            if (list.Count > 0)
            {
                for (var i = 0; i < list.Count; i++)
                {
                    var item = property.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(item);
                }
            }

            EditorGUILayout.EndVertical();


        }


        public void PropertyField(string path, GUIContent content, int column) =>
            EditorGUILayout.PropertyField(MainEditor.serializedObject.FindProperty(path), content, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth / column));
    }


}