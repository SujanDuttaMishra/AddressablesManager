using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace AddressableManager.AddressableSetter.Editor
{
    public class GlobalList : ScriptableObject
    {
        public List<AData> aDataList = new List<AData>();
        public List<AssetReference> AssetReferenceList => aDataList?.ConvertAll(o => o.assetReference);
        public static GlobalList GetOrCreateInstance(string fileName)
        {
            var globalList = Utilities.GetOrCreateInstance<GlobalList>( Constants.AssetDataPath, Constants.Asset, Constants.AssetData, fileName, out var path);
            OnCreatePath = path;
            globalList.name = fileName;
            return globalList;
        }
        public static string OnCreatePath { get; set; }
        public void RemoveWithoutSetter() => aDataList.ForEachReversed(o => o.RemoveOnSetterNull());
    }
}

