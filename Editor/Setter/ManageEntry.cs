using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace AddressableManager.AddressableSetter.Editor
{
    public class ManageEntry
    {
        private List<AData> OnAwakeList { get => Setter.onAwakeList; set => Setter.onAwakeList = value; }
        private List<AData> OnStartList { get => Setter.onStartList; set => Setter.onStartList = value; }
        private List<AData> NoAutoLoadList { get => Setter.noAutoLoadList; set => Setter.noAutoLoadList = value; }
        public List<AddressableAssetEntry> Entries => Setter.Group != null && Setter.Group.entries.Count > 0 ? Setter.Group.entries.ToList() : new List<AddressableAssetEntry>();
        public ManageEntry(Setter setter) { Setter = setter; }
        private Setter Setter { get; }
        public bool EntriesAdded { get; set; }
        public void CreateOrMoveEntry(string assetPath)
        {
            var group = Setter.ManageGroup.CreateGroupIfNotExists(Setter.template);
            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            var entry = Setter.assetSettings.CreateOrMoveEntry(guid, group);
            entry.address = Path.GetFileNameWithoutExtension(assetPath);
            if (!Entries.Contains(entry)) Entries.Add(entry);
        }
        public void CreateAData() => Entries.ForEach(o => _ = new AData(o, Setter));

        public void UpdateEntry()
        {
            if (EntriesAdded) Entries.ForEach(UpdateAdata);
            AssetDatabase.SaveAssets();
        }
        public bool IsEntriesAdded(List<string> pathsToImport) => EntriesAdded = pathsToImport.Count == Entries?.Count;
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
        private static Predicate<AData> Match(string id) => aData => Utilities.CompareOrdinal(aData.ID, id);
        public void RemoveEntry()
        {
            Utilities.RemoveAdataFrom(OnStartList, Utilities.GlobalOnStartList);
            Utilities.RemoveAdataFrom(OnAwakeList, Utilities.GlobalOnAwakeList);
            NoAutoLoadList.Clear();
            OnStartList.Clear();
            OnAwakeList.Clear();
        }

      

      
    }
}
