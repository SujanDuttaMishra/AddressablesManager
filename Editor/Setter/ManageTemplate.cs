using System.Collections.Generic;
using UnityEditor.AddressableAssets.Settings;

namespace AddressableManager.AddressableSetter
{
    public class ManageTemplate
    {

        public List<string> TemplatesNames => Templates.ConvertAll(o => o.Name);
        public List<AddressableAssetGroupTemplate> Templates { get; set; } = new List<AddressableAssetGroupTemplate>();
        private List<string> AllProfile { get; set; }
        private Setter Setter { get; }
        public ManageTemplate(Setter setter)
        {
            Setter = setter;
           
        }

        public void InitTemplate()
        {
            Setter.assetSettings = Utilities.GetOrCreateInstances<AddressableAssetSettings>(Constants.AddressableAssetSettingsName, true);
            AllProfile = Setter.assetSettings.profileSettings.GetAllProfileNames();
            ApplyTemplate();

        }

        public void ApplyTemplate()
        {
            SetTemplates(new[] { "Remote", "Local" });
            if (Setter.template == null) Setter.template = Templates[0];
            if (Setter.assetSettings == null || Setter.template == null) return;
            if (!Setter.assetSettings.GroupTemplateObjects.Contains(Setter.template)) Setter.assetSettings.GroupTemplateObjects.Add(Setter.template);
            if (Setter.ManageGroup.IsGroup()) Setter.template.ApplyToAddressableAssetGroup(Setter.ManageGroup.Group);
            if (!AllProfile.Contains(Setter.template.Name)) Setter.assetSettings.profileSettings.AddProfile(Setter.template.Name, null);
            Setter.assetSettings.name = Constants.AddressableAssetSettingsName;
            Setter.assetSettings.BuildRemoteCatalog = true;
            Setter.assetSettings.AddLabel(Constants.OnStart);
            Setter.assetSettings.AddLabel(Constants.OnAwake);
            Setter.assetSettings.activeProfileId = Setter.assetSettings.profileSettings.GetProfileId(Setter.template.Name);

        }
        public void SetTemplates(string[] names) => names.ForEach(AddTemplates);
        public void AddTemplates(string names)
        {
            if (Utilities.IsNullEmptyWhiteSpace(names)) return;
            var template = Utilities.GetOrCreateInstances<AddressableAssetGroupTemplate>(names);
            if (!Templates.Contains(template)) Templates.Add(template);
        }
    }
}
