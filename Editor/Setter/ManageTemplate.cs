using System.Collections.Generic;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using static AddressableManager.AddressableSetter.Editor.Utilities;

namespace AddressableManager.AddressableSetter.Editor
{
    public class ManageTemplate
    {
        public List<AddressableAssetGroupTemplate> Templates { get; set; } = new List<AddressableAssetGroupTemplate>();
        public List<string> AllProfile => Setter.assetSettings.profileSettings.GetAllProfileNames();
        private Setter Setter { get; }
        public ManageTemplate(Setter setter)
        {
            Setter = setter;
        }

        public void InitTemplate() => LoadTemplateFromPackagePath(new string[] { "Remote", "Local", "Default" });

        private void LoadTemplateFromPackagePath(IEnumerable<string> templateName)
        {
            templateName.ForEach(o =>
            {
                LoadAssetFromPackagePath<AddressableAssetGroupTemplate>(Constants.AddressablesManagerSettings, o, out var asset);
                if (!AllProfile.Contains(asset.Name)) Setter.assetSettings.profileSettings.AddProfile(asset.Name, null);
                Templates.AddIfNotContains(asset);
            });
            ApplyTemplate();
        }

        public void ApplyTemplate()
        {
            if (Templates.Count > 0 && Templates[0] !=null && Setter.template == null) Setter.template = Templates[0];
            if (Setter.template == null ) return;

            if (Setter.ManageGroup.IsGroup(Setter.GroupName)) Setter.template.ApplyToAddressableAssetGroup(Setter.ManageGroup.Group);
            if (!AllProfile.Contains(Setter.template.Name)) Setter.assetSettings.profileSettings.AddProfile(Setter.template.Name, null);
            Setter.assetSettings.name = Constants.AddressableAssetSettingsName;
            Setter.assetSettings.BuildRemoteCatalog = true;
            Setter.assetSettings.AddLabel(Constants.OnStart);
            Setter.assetSettings.AddLabel(Constants.OnAwake);
            Setter.assetSettings.activeProfileId = Setter.assetSettings.profileSettings.GetProfileId(Setter.template.Name);
        }


        public void AddNewTemplates(string names)
        {
            if (GetTemplate(names)) return;
            Setter.AssetSettings.CreateAndAddGroupTemplate(names, "Pack assets into asset bundles.", typeof(BundledAssetGroupSchema), typeof(ContentUpdateGroupSchema));
            var template = GetAsset<AddressableAssetGroupTemplate>(names);
            Templates.AddIfNotContains(template);
        }

        private bool GetTemplate(string names)
        {
            var template = GetAsset<AddressableAssetGroupTemplate>(names);
            if (template == null) return false;
            Templates.AddIfNotContains(template);
            return true;
        }
    }
}
