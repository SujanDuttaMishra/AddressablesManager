using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace AddressableManager.AddressableSetter.Editor
{
    public class Setter : ScriptableObject
    {
        public ManageTemplate ManageTemplate { get; set; }
        public ManageEntry ManageEntry { get; set; }
        public ManageLabel ManageLabel { get; set; }
        public ManageGroup ManageGroup { get; set; }


        public string newGroupName;
        public string newTemplate;
        public string customLabel;
        public AutoLoad autoLoad;

        public List<string> customLabelList;
        public List<AData> noAutoLoadList;
        public List<AData> onAwakeList;
        public List<AData> onStartList;


        public List<AssetLabelReference> labelReferences;

        public AddressableAssetGroupTemplate template;
        public AddressableAssetSettings assetSettings;
        public AddressableAssetGroup Group => ManageGroup.TryGetGroup();
        public bool IsGroup => ManageGroup.IsGroup();
        public string GroupName => Utilities.IsNullEmptyWhiteSpace(newGroupName) ? name : newGroupName;
        public int AssetCount => (noAutoLoadList?.Count ?? 0) + (onAwakeList?.Count ?? 0) + (onStartList?.Count ?? 0);

        public Setter()
        {
            ManageTemplate = new ManageTemplate(this);
            ManageEntry = new ManageEntry(this);
            ManageLabel = new ManageLabel(this);
            ManageGroup = new ManageGroup(this);
            labelReferences = new List<AssetLabelReference>();
            customLabelList = new List<string>();
        }


        [MenuItem("Assets/Addressable > SetEntry > FromFolder")]
        public static void CreateNew()
        {
            var selection = AssetDatabase.GetAssetPath(Selection.activeObject);
            var fileName = Path.GetFileNameWithoutExtension(selection);
            if (!Utilities.GetAsset<Setter>(fileName)) Utilities.CreateNew<Setter>(fileName, selection);
        }


        private void OnEnable()
        {
            ManageTemplate.InitTemplate();


        }


        public void Add()
        {

            noAutoLoadList = new List<AData>();
            onAwakeList = new List<AData>();
            onStartList = new List<AData>();
            

            var pathsToImport = PathsToImport(GroupName);

            if (pathsToImport.Count > 0)
            {
                ManageEntry?.Entries.Clear();
                pathsToImport.ForEach(o => ManageEntry.CreateOrMoveEntry(o));
                ManageEntry?.AddEntryToList();
            }
            AssetDatabase.SaveAssets();
        }

        public List<string> PathsToImport(string groupName) => Utilities.GetAssetPathsFromLocation<Setter>(groupName, new string[]
            {
                ".meta", ".DS_Store", $"{name}.asset",
                $"{Constants.GlobalOnStartList}.asset",
                $"{Constants.GlobalOnAwakeList}.asset"
            });



        public void Update()
        {
            if (!IsGroup) return;
            ManageEntry.UpdateEntry();

        }
        public void Remove()
        {
            ManageLabel.RemoveLabel();
            ManageEntry.RemoveEntry();
            autoLoad = AutoLoad.None;
            AssetDatabase.SaveAssets();

        }



    }
}