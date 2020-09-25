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

        public bool moveToAssetData;
        public bool createUnityTypeFolders;

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

        public List<string> PathsToImport { get; set; }

        public List<AssetLabelReference> labelReferences;

        public AddressableAssetGroupTemplate template;
        public static AddressableAssetSettings AssetSettings { get; private set; }
        public string FolderDirectory => Path.GetDirectoryName(AssetDatabase.GetAssetPath(this));
        public bool IsFolderInAssetData => FolderDirectory != null && FolderDirectory.StartsWith(Constants.AssetDataPath.Replace('/', '\\'));

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


        private void OnEnable() => ManageTemplate.InitTemplate();
        public void Add()
        {
            noAutoLoadList = new List<AData>();
            onAwakeList = new List<AData>();
            onStartList = new List<AData>();


            if (ReCalculatePathToImport(out var outPathList))
            {
                ManageEntry.Entries = new List<AddressableAssetEntry>();
                outPathList.ForEach(o => ManageEntry.CreateOrMoveEntry(o));
                ManageEntry?.CreateAData();
            }
            if (!Utilities.SettersList.Contains(this)) Utilities.SettersList.Add(this);


            AssetDatabase.SaveAssets();
        }

        public bool ReCalculatePathToImport(out List<string> outPathList) => (PathsToImport = outPathList = ExcludeFromSetter.Exclude(GetPathsToImport(), this)).Count > 0;

        public List<string> GetPathsToImport()
        {
            var exclude = new List<string>
            {
                ".cs",".asmdef",".meta", ".DS_Store", $"{name}.asset",
                $"{Constants.GlobalOnStartList}.asset",
                $"{Constants.GlobalOnAwakeList}.asset"
            };
            Utilities.GetAssets<Setter>().ForEachWithCondition(o => exclude.Add(o.name + ".asset"), o => !exclude.Contains(o.name + ".asset"));

            return Utilities.GetAssetPathsFromLocation<Setter>(GroupName, exclude, include);
        }


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
            if (!IsGroup || !ManageEntry.EntriesAdded) return;

            ManageLabel.RemoveLabels();
            ManageEntry.RemoveEntry();
            ManageGroup.RemoveGroup(GroupName);
            Add();

        }

        internal void Organize()
        {
            string folderPath;
            if (moveToAssetData)
            {
                folderPath = Constants.AssetDataPath.Replace('/', '\\') + GroupName;
                if (!IsFolderInAssetData) AssetDatabase.MoveAsset(FolderDirectory, folderPath);
            }
            else folderPath = FolderDirectory.Replace('/', '\\');

            if (createUnityTypeFolders && IsFolderInAssetData)
            {
                int countType = 0;
                var data = Directory.EnumerateFiles(folderPath, "*", SearchOption.AllDirectories).ToList();
                for (var i = 0; i < data.Count; i++)
                {
                    var asset = AssetDatabase.LoadAssetAtPath(data[i], typeof(Object));
                    var assetPath = AssetDatabase.GetAssetPath(asset);
                    var type = AssetDatabase.GetMainAssetTypeAtPath(assetPath);

                    if (type != null && type != typeof(Setter))
                    {
                        countType++;
                        var typeBase = type.ToString().Remove(0, 12);
                        var obj = asset as GameObject;
                        var haveParticle = obj != null && obj.GetComponentsInChildren<ParticleSystem>()?.Length > 0;

                        if (type == typeof(GameObject)) CreateDirectoryAndMove(haveParticle ? FolderName("/ParticleSystem") : FolderName("/Prefabs"), typeBase, asset);
                        else CreateDirectoryAndMove(FolderName(typeBase), typeBase, asset);
                    }
                }

                UnityTypeFolderCreated = CheckIfFoldersExistForEachType();
            }

        }
        private string FolderName(string typeBase)
        {
            return Constants.AssetDataPath + GroupName + "/" + typeBase;
        }

        private static void CreateDirectoryAndMove(string path, string typeBase, Object asset)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            MoveAsset(asset, typeBase, Path.GetFileName(path), path);
        }

        private static void MoveAsset(Object asset, string typeBase, string directoryName,string path)
        {
            if (typeBase != directoryName) return;
            
           // path = path.Replace('/', '\\');

           // Debug.Log($"moved {path} {Directory.Exists(path)}");

           //  Debug.Log($"ready to move asset {asset.name}  to  path {path}");

           


            var newPath = string.Concat(path, asset.name, ".asset");
            var oldPath = AssetDatabase.GetAssetPath(asset);
            var moved = string.IsNullOrEmpty(AssetDatabase.ValidateMoveAsset(oldPath, newPath)) ? 
                AssetDatabase.MoveAsset(oldPath, newPath) :
                $"Failed to move asset from path '{oldPath}' to '{newPath}'.";

            
            Debug.Log($"moved {moved}");
        }

        public bool UnityTypeFolderCreated { get; set; }

        private bool CheckIfFoldersExistForEachType()
        {
            return true;
        }
    }
}