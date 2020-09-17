using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace AddressableManager.AddressableSetter
{
    internal class GroupsEditor<T> where T : ScriptableObject
    {
        private UnityEditor.Editor MainEditor { get; }
        private Setter Setter => (Setter)MainEditor.target;
        private int Column { get; } = 4;
        private int TemplateIndex { get; set; }
        private int GroupsNameIndex { get; set; }
        private bool GroupTemplateSettings { get; set; } = true;
        private bool NewGroup { get; set; } = true;
        private bool FolderNameGroup { get; set; } = true;
        private bool Button { get; set; }
        private bool CleanEmptyGroup { get; set; }
        private bool IsNewTemplateValid => !Utilities.IsNullEmptyWhiteSpace(Setter.newTemplate);
        internal GroupsEditor(UnityEditor.Editor editor)
        {
            MainEditor = editor;
            
        }
        internal void Init()
        {
            GroupTemplateSettings = EditorGUILayout.BeginFoldoutHeaderGroup(GroupTemplateSettings, "Groups Settings");
            if (GroupTemplateSettings)
            {
                EditorGUILayout.BeginVertical("Box");
                var headers = IsNewTemplateValid ?
                    new[] { "  Templates Choice", "Selected Template", "Settings" } : new[] { "Enter New Templates", "Templates Choice", "Selected Template", "Settings" };
                Headers(headers);
                Body();
                if (Setter.ManageGroup.EmptyGroupExists) CleanEmptyGroup = GUILayout.Button("Clean Empty Groups", GUILayout.MaxWidth(150));
                if (CleanEmptyGroup) Setter.ManageGroup.CleanEmptyGroup();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            GUILayout.Space(5);
        }

        private void Headers(IEnumerable<string> headers)
        {
            EditorGUILayout.BeginHorizontal();
            if (IsNewTemplateValid) Button = GUILayout.Button("Add & Apply Template ", Utilities.MaxWidth(Column));
            headers.ForEach(o => Utilities.Label(o, Column));
            EditorGUILayout.EndHorizontal();
        }

        private void Body()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal();
            Utilities.PropertyField(MainEditor, nameof(Setter.newTemplate), Column);
            if (Button)
            {
                Setter.ManageTemplate.AddTemplates(Setter.newTemplate);
                Setter.template = Setter.ManageTemplate.Templates.Find(o => o.Name == Setter.newTemplate);
            }
            var displayedOptions = Setter.ManageTemplate.TemplatesNames.ToArray();
            TemplateIndex = EditorGUILayout.Popup(TemplateIndex, displayedOptions, Utilities.MaxWidth(Column));
            Setter.template = Setter.ManageTemplate.Templates.Find(o => o.Name == displayedOptions[TemplateIndex]);

            Utilities.PropertyField(MainEditor, nameof(Setter.template), Column);
            Utilities.PropertyField(MainEditor, nameof(Setter.assetSettings), Column);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            NewGroup = EditorGUILayout.Toggle("Create New Group", NewGroup);
            if (!FolderNameGroup) EditorGUILayout.LabelField("Custom Group Name");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (NewGroup)
            {
                FolderNameGroup = EditorGUILayout.Toggle("Add Folder Name Group", FolderNameGroup);
                if (FolderNameGroup) Setter.newGroupName = Setter.name;
                else Utilities.PropertyField(MainEditor, nameof(Setter.newGroupName), Column);
            }
            else
            {
                GroupsNameIndex = EditorGUILayout.Popup("Available Groups", GroupsNameIndex, Setter.ManageGroup.GroupsNames);
                Setter.newGroupName = Setter.ManageGroup.GroupsNames[GroupsNameIndex];
            }
            Utilities.ApplyModifiedProperties(MainEditor);
            EditorGUILayout.EndHorizontal();
            if (!EditorGUI.EndChangeCheck()) return;
            Setter.ManageTemplate.ApplyTemplate();
        }
        
    }
}