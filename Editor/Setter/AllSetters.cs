using System.Collections.Generic;
using UnityEngine;
using static AddressableManager.AddressableSetter.Editor.Utilities;

namespace AddressableManager.AddressableSetter.Editor
{
    public class AllSetters:ScriptableObject
    {
        public static List<Setter> settersList  = new List<Setter>();

        public static AllSetters Instance { get; private set; }

        private void OnEnable()
        {
            if (Instance == null) 
                Instance = LoadAssetFromPackagePath<AllSetters>(Constants.AddressablesManagerSettings, nameof(AllSetters), out var setterList) ? 
                    setterList : GetOrCreateInstances<AllSetters>(nameof(AllSetters));
            Instance.name = nameof(AllSetters);
            RemoveNullOrUnpopulated();
        }

        public static void RemoveNullOrUnpopulated()
        {
            settersList.ForEachReversed(o =>
            {
                if (o == null || o.AssetCount <= 0) settersList.Remove(o);
            });
        }

       
    }
}
