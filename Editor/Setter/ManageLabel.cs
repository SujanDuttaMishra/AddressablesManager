using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine.AddressableAssets;

namespace AddressableManager.AddressableSetter.Editor
{
    public class ManageLabel
    {
        private List<string> LabelsInAssetSettings => Utilities.LabelsToStringList(Setter.assetSettings);
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
            if (!LabelsInAssetSettings.Contains(label)) AssetSettings.AddLabel(label);
        }
        public void RemoveLabel(string label)
        {
            if (LabelReferences.Count > 0) LabelReferences = LabelReferences.Where(o => o.labelString != label).ToList();
            if (CustomLabelList.Count > 0) CustomLabelList = CustomLabelList.Where(o => o != label).ToList();
            Utilities.RemoveLabelFrom(label, AssetSettings);
            Utilities.RemoveLabelFrom(label, Setter.ManageGroup.DefaultAddressableSettings);
        }
        public static void AddLabel(string label, AddressableAssetEntry entry)
        {
            if (!entry.labels.Contains(label)) entry.labels.Add(label);
        }
        public static void RemoveOnStart(AddressableAssetEntry entry)
        {
            if (entry.labels.Contains(Constants.OnStart)) entry.labels.Remove(Constants.OnStart);
        }
        public static void RemoveOnAwake(AddressableAssetEntry entry)
        {
            if (entry.labels.Contains(Constants.OnAwake)) entry.labels.Remove(Constants.OnAwake);
        }
        public static void AddOnStart(AddressableAssetEntry entry)
        {
            if (!entry.labels.Contains(Constants.OnStart)) entry.labels.Add(Constants.OnStart);
        }
        public static void AddOnAwake(AddressableAssetEntry entry)
        {
            if (!entry.labels.Contains(Constants.OnAwake)) entry.labels.Add(Constants.OnAwake);
        }
        public void RemoveLabel()
        {
            if (LabelReferences.Count <= 0) return;
            LabelReferences.ForEach(label =>
            {
                Utilities.RemoveLabelFrom(label.labelString, AssetSettings);
                Utilities.RemoveLabelFrom(label.labelString, Setter.ManageGroup.DefaultAddressableSettings);

            });
            LabelReferences.Clear();
        }
        public List<string> AddLabels(AddressableAssetEntry entry)
        {
            if (LabelReferences.Count <= 0) return new List<string>();

            LabelReferences.ForEach(o =>
            {
                if (o.labelString != Constants.OnAwake && o.labelString != Constants.OnStart)
                {
                    AddLabel(o.labelString, entry);
                }

            });
            return entry.labels.ToList();
        }

        public static List<string> AddLabels(AddressableAssetEntry entry, List<string> labelReferences)
        {
            if (labelReferences?.Count <= 0) return new List<string>();

            labelReferences?.ForEach(o =>
            {
                if (o != Constants.OnAwake && o != Constants.OnStart)
                {
                    AddLabel(o, entry);
                }

            });
            return entry.labels.ToList();
        }
    }
}