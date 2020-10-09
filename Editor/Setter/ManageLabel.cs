using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine.AddressableAssets;
using static AddressableManager.AddressableSetter.Editor.Utilities;

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
            if (!IsLabelIn(label, LabelReferences)) LabelReferences.Add(new AssetLabelReference { labelString = label });
            if (!LabelsToStringList(AssetSettings).Contains(label)) AssetSettings.AddLabel(label);
            if (!LabelsToStringList(DefaultAssetSettings).Contains(label)) DefaultAssetSettings.AddLabel(label);
        }
        public void RemoveLabel(string label)
        {
            if (LabelReferences.Count > 0) LabelReferences = LabelReferences.Where(o => o.labelString != label).ToList();
            if (CustomLabelList.Count > 0) CustomLabelList = CustomLabelList.Where(o => o != label).ToList();
            RemoveLabelFrom(label, AssetSettings);
            RemoveLabelFrom(label, DefaultAssetSettings);
        }

        public void RemoveLabels()
        {
            RemoveAndClearLabelFrom(LabelReferences, AssetSettings, DefaultAssetSettings);
            RemoveAndClearLabelFrom(CustomLabelList, AssetSettings, DefaultAssetSettings);
        }
    }
}