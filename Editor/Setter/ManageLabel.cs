using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine.AddressableAssets;

namespace AddressableManager.AddressableSetter.Editor
{
    public class ManageLabel
    {
        private AddressableAssetSettings AssetSettings => Setter.assetSettings;
        private List<AssetLabelReference> LabelReferences { get => Setter.labelReferences; set => Setter.labelReferences = value; }
        private List<string> CustomLabelList { get => Setter.customLabelList; set => Setter.customLabelList = value; }
        private Setter Setter { get; }
        public ManageLabel(Setter setter) { Setter = setter; }
        public void ManageAutoLoad()
        {
            switch (Setter.autoLoad)
            {
                case AutoLoad.OnStart: AddLabel(Constants.OnStart); RemoveLabel(Constants.OnAwake); break;
                case AutoLoad.OnAwake: AddLabel(Constants.OnAwake); RemoveLabel(Constants.OnStart); break;
                case AutoLoad.None: RemoveLabel(Constants.OnAwake); RemoveLabel(Constants.OnStart); break;
                default: throw new ArgumentOutOfRangeException();
            }
        }
        public void AddLabel(string label)
        {
            if (!Utilities.IsLabelIn(label, LabelReferences)) LabelReferences.Add(new AssetLabelReference { labelString = label });
            if (!Utilities.LabelsToStringList(AssetSettings).Contains(label)) AssetSettings.AddLabel(label);
            if (!Utilities.LabelsToStringList(Utilities.DefaultAssetSettings).Contains(label)) Utilities.DefaultAssetSettings.AddLabel(label);
        }
        public void RemoveLabel(string label)
        {
            if (LabelReferences.Count > 0) LabelReferences = LabelReferences.Where(o => o.labelString != label).ToList();
            if (CustomLabelList.Count > 0) CustomLabelList = CustomLabelList.Where(o => o != label).ToList();
            Utilities.RemoveLabelFrom(label, AssetSettings);
            Utilities.RemoveLabelFrom(label, Utilities.DefaultAssetSettings);
        }
        public static void AddLabelToEntry(AddressableAssetEntry entry, string label) { if (!entry.labels.Contains(label)) entry.labels.Add(label); }
        private static void RemoveLabelFromEntry(AddressableAssetEntry entry, string label) { if (entry.labels.Contains(label)) entry.labels.Remove(label); }
        public static void RemoveOnStart(AddressableAssetEntry entry) => RemoveLabelFromEntry(entry, Constants.OnStart);
        public static void RemoveOnAwake(AddressableAssetEntry entry) => RemoveLabelFromEntry(entry, Constants.OnAwake);
        public static void AddOnStart(AddressableAssetEntry entry) => AddLabelToEntry(entry, Constants.OnStart);
        public static void AddOnAwake(AddressableAssetEntry entry) => AddLabelToEntry(entry, Constants.OnAwake);
        public void RemoveLabels()
        {
            RemoveAndClearLabelFrom(LabelReferences, AssetSettings, Utilities.DefaultAssetSettings);
            RemoveAndClearLabelFrom(CustomLabelList, AssetSettings, Utilities.DefaultAssetSettings);
        }
        public static void RemoveAndClearLabelFrom(List<AssetLabelReference> list, AddressableAssetSettings settings, AddressableAssetSettings defaultSettings)
        {
            if (list.Count <= 0) return;
            list.ForEach(label => { Utilities.RemoveLabelFrom(label.labelString, settings); Utilities.RemoveLabelFrom(label.labelString, defaultSettings); });
            list.Clear();
        }
        public static void RemoveAndClearLabelFrom(List<string> list, AddressableAssetSettings settings, AddressableAssetSettings defaultSettings)
        {
            if (list.Count <= 0) return;
            list.ForEach(label => { Utilities.RemoveLabelFrom(label, settings); Utilities.RemoveLabelFrom(label, defaultSettings); });
            list.Clear();
        }
        public static List<string> AddLabels(AddressableAssetEntry entry, List<AssetLabelReference> labelReferences)
        {
            if (labelReferences.Count <= 0) return entry.labels.ToList();
            labelReferences.ForEach(o => { if (o.labelString != Constants.OnAwake && o.labelString != Constants.OnStart) AddLabelToEntry(entry, o.labelString); });
            return entry.labels.ToList();
        }
    }
}