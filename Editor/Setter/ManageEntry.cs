using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using static AddressableManager.AddressableSetter.Editor.Utilities;

namespace AddressableManager.AddressableSetter.Editor
{
    public class ManageEntry
    {
        private List<AData> OnAwakeList { get => Setter.onAwakeList; set => Setter.onAwakeList = value; }
        private List<AData> OnStartList { get => Setter.onStartList; set => Setter.onStartList = value; }
        private List<AData> NoAutoLoadList { get => Setter.noAutoLoadList; set => Setter.noAutoLoadList = value; }
        public List<AddressableAssetEntry> Entries { get; set; } = new List<AddressableAssetEntry>();
        public ManageEntry(Setter setter) { Setter = setter; }
        private Setter Setter { get; }
        public bool EntriesAdded { get; set; }
        public void CreateOrMoveEntry(string assetPath)
        {
            var group = Setter.ManageGroup.CreateGroupIfNotExists(Setter.GroupName,Setter.template);
            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            var entry = Setter.assetSettings.CreateOrMoveEntry(guid, group);
            entry.address = Path.GetFileNameWithoutExtension(assetPath);
            if (!Entries.Contains(entry)) Entries.Add(entry);
        }
        public void CreateAData() => Entries.ForEach(o => _ = new AData(o, Setter));

        public void UpdateEntry()
        {
            if (Entries.Count > 0)
            {
                VerifyGroupEntries(Setter.Group);
                Entries.ForEach(UpdateAdata);
            }

            AssetDatabase.SaveAssets();
        }
        public bool IsEntriesAdded()
        {
            EntriesAdded = false;
            List<string> pathList;
            if (!Setter.IsGroup) return EntriesAdded;

            if (Entries.Count > 0)
            {
                EntriesAdded = Entries != null && Setter.PathsToImport.Count == Entries.Count;
                if (EntriesAdded) return EntriesAdded = true;

                Setter.ReCalculatePathToImport(out pathList);
                return EntriesAdded = pathList.Count == Entries.Count;
            }

            if (!Setter.ReCalculatePathToImport(out pathList)) return EntriesAdded;
            EntriesAdded = pathList.Count == Setter.Group.entries.Count;
            Setter.Group.entries.ForEach(o=> Entries.AddIfNotContains(o));
            return EntriesAdded ;
        }

        public void VerifyGroupEntries(AddressableAssetGroup group) => group.entries?.ForEachWithCondition(o => group.RemoveAssetEntry(o), o => !Entries.Contains(o));

        private void UpdateAdata(AddressableAssetEntry o)
        {
            var adata = FindInLists(o, o.guid, new List<List<AData>>()
            {
                OnStartList,
                OnAwakeList,
                NoAutoLoadList
            });

            adata?.Update(adata.autoLoad);
        }

        public void RefreshEntryLabels()
        {
            if (EntriesAdded) Entries.ForEach(UpdateAdataLabels);
            AssetDatabase.SaveAssets();
        }

        private void UpdateAdataLabels(AddressableAssetEntry o)
        {
            var adata = FindInLists(o, o.guid, new List<List<AData>>()
            {
                OnStartList,
                OnAwakeList,
                NoAutoLoadList
            });
            adata?.RefreshLabels();
        }

        private AData FindInLists(AddressableAssetEntry o, string id, List<List<AData>> alList) => alList?.Find(l => l.Find(Match(id)) != null)?.Find(Match(id)) ?? new AData(o, Setter);
        private static Predicate<AData> Match(string id) => aData => CompareOrdinal(aData.ID, id);
        public void RemoveEntry()
        {
            RemoveAdataFrom(OnStartList, GlobalOnStartList);
            RemoveAdataFrom(OnAwakeList, GlobalOnAwakeList);
            NoAutoLoadList?.Clear();
            OnStartList?.Clear();
            OnAwakeList?.Clear();

        }




    }
}
