using System;
using System.Collections.Generic;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using static AddressableManager.AddressableSetter.Editor.Utilities;

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
        public Setter Setter { get; set; }
        public string ID { get; set; }

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
            if (Setter == null) Setter = GetSetter(group.Name);
            if (Setter == null) { RemoveOnSetterNull(); return; }
            if (!CheckEntry()) return;

            switch (load)
            {
                case AutoLoad.OnStart:
                    AddOnStartLabel(entry);
                    RemoveOnAwakeLabel(entry);
                    AddAdataTo(this, GlobalOnStartList);
                    RemoveAdataFrom(this, GlobalOnAwakeList);
                    SetterLists(Setter.onStartList, Setter.onAwakeList, Setter.noAutoLoadList);
                    break;
                case AutoLoad.OnAwake:
                    AddOnAwakeLabel(entry);
                    RemoveOnStartLabel(entry);
                    RemoveAdataFrom(this, GlobalOnStartList);
                    AddAdataTo(this, GlobalOnAwakeList);
                    SetterLists(Setter.onAwakeList, Setter.onStartList, Setter.noAutoLoadList);
                    break;
                case AutoLoad.None:
                    RemoveOnAwakeLabel(entry);
                    RemoveOnStartLabel(entry);
                    RemoveAdataFrom(this, GlobalOnStartList);
                    RemoveAdataFrom(this, GlobalOnAwakeList);
                    SetterLists(Setter.noAutoLoadList, Setter.onAwakeList, Setter.onStartList);

                    break;
                default: throw new ArgumentOutOfRangeException();
            }

            RefreshLabels();
            entry = DefaultAssetSettings.CreateOrMoveEntry(ID, group);

        }

        private void SetterLists(List<AData> addList, List<AData> removeListA, List<AData> removeListB)
        {
            if (Setter == null) return;
            AddAdataTo(this, addList);
            RemoveAdataFrom(this, removeListA);
            RemoveAdataFrom(this, removeListB);
        }

        public bool CheckEntry()
        {
            if (entry != null && !IsNullEmptyWhiteSpace(entry.address) && entry.MainAsset != null && entry.TargetAsset != null) return true;
            RemoveAssetInternal();
            return false;
        }

        public void RemoveAssetInternal()
        {
            if (Setter != null)
            {
                Setter.ManageEntry.Entries.Remove(entry);
                RemoveAdataFrom(this, Setter.onStartList);
                RemoveAdataFrom(this, Setter.onAwakeList);
                RemoveAdataFrom(this, Setter.noAutoLoadList);
               
            }

            RemoveOnSetterNull();
        }

        public void RemoveOnSetterNull()
        {
            if (Setter != null) return;
            RemoveAdataFrom(this, GlobalOnStartList);
            RemoveAdataFrom(this, GlobalOnAwakeList);
            if (group != null && group.entries.Contains(entry)) group.RemoveAssetEntry(entry);
        }

        public void RefreshLabels() => labels = AddLabels(entry, Setter.labelReferences);
    }


}