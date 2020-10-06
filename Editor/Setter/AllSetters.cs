using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static AddressableManager.AddressableSetter.Editor.Utilities;

namespace AddressableManager.AddressableSetter.Editor
{
    public class AllSetters:ScriptableObject
    {
        public static List<Setter> settersList  = new List<Setter>();

        public AllSetters Instance { get; private set; }

        private void OnEnable()
        {
            if (Instance == null) Instance = LoadAssetFromPackagePath<AllSetters>(Constants.AddressablesManagerSettings, nameof(AllSetters), out var setterList) ? setterList : GetOrCreateInstances<AllSetters>(nameof(AllSetters));

            RemoveNullOrUnpopulated();
        }

        public static void RemoveNullOrUnpopulated()
        {
            settersList.ForEach(o =>
            {
                if (o == null || o.AssetCount <= 0) settersList.Remove(o);
            });
        }
    }
}
