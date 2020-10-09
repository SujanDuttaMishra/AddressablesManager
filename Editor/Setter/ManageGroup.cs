using System.Collections.Generic;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using static AddressableManager.AddressableSetter.Editor.Utilities;

namespace AddressableManager.AddressableSetter.Editor
{
    public class ManageGroup
    {
        private AddressableAssetSettings AssetSettings => Setter.assetSettings;
        private List<AddressableAssetGroup> AssetSettingsGroups => AssetSettings.groups;
        public string[] GroupsNames => AssetSettingsGroups.ConvertAll(o => o.Name).ToArray();
        public AddressableAssetGroup Group { get; private set; }
        private Setter Setter { get; }

        public ManageGroup(Setter setter)
        {
            Setter = setter;
        }
       
        public AddressableAssetGroup TryGetGroup(string groupName) => Group = 
            FindGroupName(groupName, AssetSettingsGroups, out var outGroup)? outGroup : 
            FindGroupName(groupName, DefaultGroups, out outGroup) ? outGroup :
            null;



        public AddressableAssetGroup CreateGroupIfNotExists(string groupName,AddressableAssetGroupTemplate groupTemplate) => Group = 
            TryGetGroup(groupName,out var group) ? group : CreateGroup<BundledAssetGroupSchema>(groupName,groupTemplate);

        public bool IsGroup(string groupName) => TryGetGroup(groupName) != null;
        public bool TryGetGroup(string groupName,out AddressableAssetGroup group) => (group = TryGetGroup(groupName)) != null;
        private AddressableAssetGroup CreateGroup<TSchemaType>(string groupName,AddressableAssetGroupTemplate groupTemplate)
        {
            var group = AssetSettings.CreateGroup(groupName, false, false, false, new List<AddressableAssetGroupSchema> { AssetSettings.DefaultGroup.Schemas[0] }, typeof(TSchemaType));

            if (groupTemplate != null) groupTemplate.ApplyToAddressableAssetGroup(group);

            return group;
        }

        public void RemoveGroup(string groupName) { if(TryGetGroup(groupName,out var group)) DefaultAddressableSettings.RemoveGroup(group) ; }
    }
}
