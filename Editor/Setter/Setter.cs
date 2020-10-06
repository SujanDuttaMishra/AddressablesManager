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

       
        public string FolderPath => moveToAssetData ? Constants.AssetDataPath.Replace('/', '\\') + GroupName : FolderDirectory.Replace('/', '\\');
        private string FolderName(string typeBase) => Constants.AssetDataPath + GroupName + "/" + typeBase;
       
        public bool deleteEmptyDirectory;

        internal void Organize()
        {


            if (moveToAssetData)
            {

                if (!IsFolderInAssetData) AssetDatabase.MoveAsset(FolderDirectory, FolderPath);
            }


            AssetDatabase.Refresh();

             
            if (createUnityTypeFolders && IsFolderInAssetData)
            {
                var data = Directory.EnumerateFiles(FolderPath, "*", SearchOption.AllDirectories).ToList();
                
                for (var i = 0; i < data.Count; i++)
                {
                    var asset = AssetDatabase.LoadAssetAtPath(data[i], typeof(Object));
                    var assetPath = AssetDatabase.GetAssetPath(asset);
                    var type = AssetDatabase.GetMainAssetTypeAtPath(assetPath);

                    if (type != null && type != typeof(Setter))
                    {
                        
                        var typeBase = type.ToString().Remove(0, 12);
                        var obj = asset as GameObject;
                        var haveParticle = obj != null && obj.GetComponentsInChildren<ParticleSystem>()?.Length > 0;

                        if (type == typeof(GameObject)) CreateDirectoryAndMove(haveParticle ? FolderName("/ParticleSystem") : FolderName("/Prefabs"), type, haveParticle, asset);
                        else CreateDirectoryAndMove(FolderName(typeBase), type, haveParticle, asset);
                       
                    }
                }

                var length = Directory.GetDirectories(FolderPath).Length;
               
                
            }


        }



        private  void CreateDirectoryAndMove(string path, Type type, bool haveParticle, Object asset)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            AssetDatabase.Refresh();
            var directoryName = type != typeof(GameObject) ? Path.GetFileName(path) : haveParticle ? "ParticleSystem" : "Prefabs";

            MoveAsset(asset, directoryName, path);
            
        }

        private  void MoveAsset(Object asset, string directoryName, string path)
        {
            var assetPath = AssetDatabase.GetAssetPath(asset);
            var checkedDirectory = Path.GetFileName(Path.GetDirectoryName(assetPath));
            if (checkedDirectory == directoryName) return;
            path += "/" + Path.GetFileName(AssetDatabase.GetAssetPath(asset));
            var moved = AssetDatabase.MoveAsset(assetPath, path);
            Debug.Log($"moved {moved}");
            AssetDatabase.Refresh();
            
        }





        public bool ProcessDirectory(string folderPath,out string emptyFolder )
        {
            emptyFolder=string.Empty;
            foreach (var directory in Directory.GetDirectories(folderPath))
            {
               
                if(Directory.GetFiles(directory).Length == 0 && Directory.GetDirectories(directory).Length == 0)
                {
                    emptyFolder = directory;
                    return true;
                }
            }
            return false;
        }



        public void DeleteEmptyDirectory()
        {
            ProcessDirectory(FolderPath, out var emptyFolder);
            Directory.Delete(emptyFolder, false);
            AssetDatabase.Refresh();
        }
    }
}