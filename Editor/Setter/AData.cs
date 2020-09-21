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
        private List<AData> GlobalOnAwake => Utilities.GlobalOnAwakeList;
        private List<AData> GlobalOnStart => Utilities.GlobalOnStartList;
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
            Setter = Setter != null ? Setter : Utilities.GetSetter(group.Name);



            if (entry.MainAsset == null || entry.TargetAsset == null || entry.MainAsset.GetType() == typeof(Setter))
            {
                Setter.ManageEntry.Entries.Remove(entry);
                Utilities.RemoveAdataFrom(this, Setter.onStartList);
                Utilities.RemoveAdataFrom(this, Setter.onAwakeList);
                Utilities.RemoveAdataFrom(this, Setter.noAutoLoadList);
                Utilities.RemoveAdataFrom(this, GlobalOnStart);
                Utilities.RemoveAdataFrom(this, GlobalOnAwake);

                if (group.entries.Contains(entry)) group.RemoveAssetEntry(entry);

                return;
            }

            switch (load)
            {
                case AutoLoad.OnStart:

                    ManageLabel.AddOnStart(entry);
                    ManageLabel.RemoveOnAwake(entry);

                    Utilities.AddAdataTo(this, GlobalOnStart);
                    Utilities.RemoveAdataFrom(this, GlobalOnAwake);

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

                    Utilities.RemoveAdataFrom(this, GlobalOnStart);
                    Utilities.AddAdataTo(this, GlobalOnAwake);


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

                    Utilities.RemoveAdataFrom(this, GlobalOnStart);
                    Utilities.RemoveAdataFrom(this, GlobalOnAwake);

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

        public void RefreshLabels() => labels = ManageLabel.AddLabels(entry, Setter.labelReferences);
    }


}