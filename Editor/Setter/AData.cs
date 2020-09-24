using System;
using System.Collections.Generic;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace AddressableManager.AddressableSetter.Editor
{
    [Serializable]
    public class AData
    {
        public string name;
        public AddressableAssetGroup group;
        public AddressableAssetEntry entry;
        public AssetReference assetReference;
        public string assetPath;
        public Object obj;
        public List<string> labels;
        public int loadOnSceneIndex;
        public Scene loadOnScene;
        public AutoLoad autoLoad;
        public Unload unload;
        private Setter Setter { get; set; }
        public string ID { get; set; }
        private List<AData> GlobalOnAwakeList => Utilities.GlobalOnAwakeList;
        private List<AData> GlobalOnStartList => Utilities.GlobalOnStartList;
        public AData(AddressableAssetEntry addressableAssetEntry, Setter setter)
        {
            Setter = setter;
            ID = addressableAssetEntry.guid;
            entry = addressableAssetEntry;
            assetReference = new AssetReference(ID);
            assetPath = addressableAssetEntry.AssetPath;
            obj = addressableAssetEntry.MainAsset;
            loadOnScene = SceneManager.GetSceneByBuildIndex(loadOnSceneIndex);
            autoLoad = Setter.autoLoad;
            unload = Unload.None;
            name = addressableAssetEntry.MainAsset.name;
            group = setter.Group;
            ManageAutoLoad(autoLoad);
        }

        public void Update(AutoLoad load) => ManageAutoLoad(load);

        private void ManageAutoLoad(AutoLoad load)
        {
            if (Setter == null) Setter = Utilities.GetSetter(group.Name);
            if (Setter == null) { RemoveAssetInternal(); return; }
            if (!CheckEntryExist()) return;

            switch (load)
            {
                case AutoLoad.OnStart:
                    ManageLabel.AddOnStart(entry);
                    ManageLabel.RemoveOnAwake(entry);
                    Utilities.AddAdataTo(this, GlobalOnStartList);
                    Utilities.RemoveAdataFrom(this, GlobalOnAwakeList);
                    if (Setter != null)
                    {
                        Utilities.AddAdataTo(this, Setter.onStartList);
                        Utilities.RemoveAdataFrom(this, Setter.onAwakeList);
                        Utilities.RemoveAdataFrom(this, Setter.noAutoLoadList);
                    }
                    break;
                case AutoLoad.OnAwake:
                    ManageLabel.AddOnAwake(entry);
                    ManageLabel.RemoveOnStart(entry);
                    Utilities.RemoveAdataFrom(this, GlobalOnStartList);
                    Utilities.AddAdataTo(this, GlobalOnAwakeList);
                    if (Setter != null)
                    {
                        Utilities.AddAdataTo(this, Setter.onAwakeList);
                        Utilities.RemoveAdataFrom(this, Setter.onStartList);
                        Utilities.RemoveAdataFrom(this, Setter.noAutoLoadList);
                    }
                    break;
                case AutoLoad.None:
                    ManageLabel.RemoveOnAwake(entry);
                    ManageLabel.RemoveOnStart(entry);
                    Utilities.RemoveAdataFrom(this, GlobalOnStartList);
                    Utilities.RemoveAdataFrom(this, GlobalOnAwakeList);
                    if (Setter != null)
                    {
                        Utilities.AddAdataTo(this, Setter.noAutoLoadList);
                        Utilities.RemoveAdataFrom(this, Setter.onAwakeList);
                        Utilities.RemoveAdataFrom(this, Setter.onStartList);
                    }
                    break;
                default: throw new ArgumentOutOfRangeException();
            }

            RefreshLabels();
            entry = Utilities.DefaultAssetSettings.CreateOrMoveEntry(ID, group);

        }

        public bool CheckEntryExist()
        {
            if (entry != null && !Utilities.IsNullEmptyWhiteSpace(entry.address) && entry.MainAsset != null && entry.TargetAsset != null) return true;
            RemoveAssetInternal();
            return false;
        }

        private void RemoveAssetInternal()
        {
            if (Setter !=null)
            {
                Setter.ManageEntry.Entries.Remove(entry);
                Utilities.RemoveAdataFrom(this, Setter.onStartList);
                Utilities.RemoveAdataFrom(this, Setter.onAwakeList);
                Utilities.RemoveAdataFrom(this, Setter.noAutoLoadList);
            }
            Utilities.RemoveAdataFrom(this, GlobalOnStartList);
            Utilities.RemoveAdataFrom(this, GlobalOnAwakeList);
            if (group != null && group.entries.Contains(entry)) group.RemoveAssetEntry(entry);
        }

        public void RefreshLabels() => labels = ManageLabel.AddLabels(entry, Setter.labelReferences);
    }


}