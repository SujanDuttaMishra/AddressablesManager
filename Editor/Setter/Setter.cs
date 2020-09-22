using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

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
        public SearchOption include = SearchOption.AllDirectories;
        public AssetType excludeType;
        public List<string> customLabelList;
        public List<AData> noAutoLoadList;
        public List<AData> onAwakeList;
        public List<AData> onStartList;


        public List<AssetLabelReference> labelReferences;

        public AddressableAssetGroupTemplate template;
        public static AddressableAssetSettings AssetSettings { get; private set; }
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
            assetSettings = AssetSettings;
        }


        [MenuItem("Assets/Addressable > SetEntry > FromFolder")]
        public static void CreateNew()
        {
            if (!AddressableAssetSettingsDefaultObject.SettingsExists)
            {
                EditorUtility.DisplayDialog("Folder Setter", "Oops ! You Forget To \" Create \" Addressable Settings. " +
                                                "\n \nGo To \"Addressable Groups\" And Click \" Create \"" +
                                                "\nWe Then Can Create \"Addressable Group Settings\" ", "OK");
                return;
            }
            AssetSettings = Utilities.GetOrCreateInstances<AddressableAssetSettings>(Constants.AddressableAssetSettingsName, true);
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
                ManageEntry.Entries = new List<AddressableAssetEntry>();
                pathsToImport.ForEach(o => ManageEntry.CreateOrMoveEntry(o));
                ManageEntry?.CreateAData();
            }
            if (!Utilities.SettersList.Contains(this)) Utilities.SettersList.Add(this);
            AssetDatabase.SaveAssets();
        }

        public List<string> PathsToImport(string groupName)
        {
            var exclude = new List<string>
            {
                ".meta", ".DS_Store", $"{name}.asset",
                $"{Constants.GlobalOnStartList}.asset",
                $"{Constants.GlobalOnAwakeList}.asset"
            };

            Utilities.GetAssets<Setter>().ForEachWithCondition(o => exclude.Add(o.name + ".asset"), o => !exclude.Contains(o.name + ".asset"));

            var stringList = Utilities.GetAssetPathsFromLocation<Setter>(groupName, exclude, include);


            if (excludeType == AssetType.All) return new List<string>();
            if (excludeType == AssetType.None) return stringList;

            Exclude(stringList, AssetType.Textures);
            Exclude(stringList, AssetType.Audio);
            Exclude(stringList, AssetType.Particle);
            Exclude(stringList, AssetType.Prefab);


            return stringList;

        }

        private void Exclude(IList<string> stringList, AssetType assetType)
        {
            var type = typeof(Object);
            switch (assetType)
            {
                case AssetType.Textures: type = typeof(Texture); break;
                case AssetType.Audio: type = typeof(AudioClip); break;
                case AssetType.Particle: type = typeof(ParticleSystem); break;
                case AssetType.Prefab: type = typeof(GameObject); break;
            }

            for (int i = 0; i < stringList.Count; i++)
            {
                var loadAssetAtPath = AssetDatabase.LoadAssetAtPath(stringList[i], type);
                if (loadAssetAtPath == null) continue;
                if (type == typeof(GameObject) && excludeType.HasFlag(AssetType.Particle))
                {
                    var go = (GameObject)loadAssetAtPath;
                    var hasParticle = go.GetComponentInChildren<ParticleSystem>() != null;
                    if (hasParticle) Exclude(stringList, i);
                }
                Exclude(stringList, excludeType.HasFlag(assetType), i);
            }
        }

        private static void Exclude(IList<string> stringList, bool hasFlag, int i) { if (hasFlag) stringList.Remove(stringList[i]); }

        private static void Exclude(IList<string> stringList, int i) => stringList.Remove(stringList[i]);

        public void Update()
        {
            if (!IsGroup) return;
            ManageEntry.UpdateEntry();

        }
        public void Remove()
        {
            ManageLabel.RemoveLabels();
            ManageEntry.RemoveEntry();
            ManageGroup.RemoveGroup(GroupName);
            autoLoad = AutoLoad.None;
            if (Utilities.SettersList.Contains(this)) Utilities.SettersList.Remove(this);
            AssetDatabase.SaveAssets();

        }

        public void Reset()
        {
            ManageLabel.RemoveLabels();
            ManageEntry.RemoveEntry();
            ManageGroup.RemoveGroup(GroupName);
            Add();

        }

    }
}