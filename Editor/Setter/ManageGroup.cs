using System.Collections.Generic;
using System.Linq;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;

namespace AddressableManager.AddressableSetter
{
    public class ManageGroup
    {
        private AddressableAssetSettings AssetSettings => Setter.assetSettings;
        public AddressableAssetSettings DefaultAddressableSettings => AddressableAssetSettingsDefaultObject.Settings;
        private List<AddressableAssetGroup> DefaultAddressableSettingsGroups => DefaultAddressableSettings.groups;
        public bool EmptyGroupExists => DefaultAddressableSettingsGroups.Any(o => o.entries.Count == 0 && !o.IsDefaultGroup());
        private AddressableAssetGroup AssetSettingsDefaultGroup => AssetSettings.DefaultGroup;
        private List<AddressableAssetGroup> AssetSettingsGroups => AssetSettings.groups;
        public string[] GroupsNames => AssetSettingsGroups.ConvertAll(o => o.Name).ToArray();
        public AddressableAssetGroup Group { get; private set; }
        private Setter Setter { get; }

        public ManageGroup(Setter setter)
        {
            Setter = setter;
        }

        public void CleanEmptyGroup() => DefaultAddressableSettingsGroups.ForEach(o => {if (EmptyGroupExists) DefaultAddressableSettings.RemoveGroup(o);});
        public AddressableAssetGroup TryGetGroup() => Group = AssetSettingsGroups.Find(o => o.Name == Setter.GroupName);
        public AddressableAssetGroup CreateGroupIfNotExists(AddressableAssetGroupTemplate addressableAssetGroupTemplate) => Group = IsGroup(out var group) ? group : CreateGroup<BundledAssetGroupSchema>(addressableAssetGroupTemplate);
        public bool IsGroup() => TryGetGroup() != null;
        public bool IsGroup(out AddressableAssetGroup group) => (group = TryGetGroup()) != null;
        private AddressableAssetGroup CreateGroup<TSchemaType>(AddressableAssetGroupTemplate addressableAssetGroupTemplate)
        {
            var group = AssetSettings.CreateGroup(Setter.GroupName, false, false, false, new List<AddressableAssetGroupSchema> { AssetSettingsDefaultGroup.Schemas[0] }, typeof(TSchemaType));

            if (addressableAssetGroupTemplate != null) addressableAssetGroupTemplate.ApplyToAddressableAssetGroup(group);

            return group;
        }

        public void RemoveGroup(string groupName) { if(IsGroup(out var group)) DefaultAddressableSettings.RemoveGroup(group) ; }
    }
}
