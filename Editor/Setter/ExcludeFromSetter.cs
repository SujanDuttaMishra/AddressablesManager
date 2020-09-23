using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AddressableManager.AddressableSetter.Editor
{
    public static class ExcludeFromSetter
    {
        public static List<string> Exclude(List<string> stringList, Setter setter)
        {
            var excludeType = setter.excludeType;
            switch (excludeType)
            {
                case AssetType.None: return stringList;
                case AssetType.All: return new List<string>();
            }
            GetExcludeType(excludeType, out var types);
           
            types.ForEachWithRecursiveNullCheck(o=>
            {
                stringList = Exclude(stringList, o);

            });
            
            return stringList;
        }
        private static List<string> Exclude(IEnumerable<string> stringList, Type type) => stringList.Where(o => AssetDatabase.LoadAssetAtPath(o, type) == null).ToList();

        private static List<string> Exclude(IEnumerable<string> stringList) => stringList.Where(o => ((GameObject)AssetDatabase.LoadAssetAtPath(o, typeof(GameObject))).GetComponentInChildren<ParticleSystem>() == null).ToList();

        private static GameObject LoadAssetAtPath(string o) => (GameObject)AssetDatabase.LoadAssetAtPath(o, typeof(GameObject));

        private static bool GetExcludeType(AssetType excludeType, out List<Type> types)
        {
            types = new List<Type>();

            foreach (AssetType value in Enum.GetValues(typeof(AssetType)))
            {
                if (value == AssetType.None || value == AssetType.All) continue;
                PopulateTypes(excludeType, types, value);
            }

            return types != null;
        }

        private static void PopulateTypes(AssetType excludeType, ICollection<Type> types, AssetType assetType)
        {
            var type = GetMatchingType(assetType);
            if (excludeType.HasFlag(assetType)) types.Add(type); else types.Remove(type);

        }

        private static Type GetMatchingType(AssetType assetType)
        {
            switch (assetType)
            {
                case AssetType.Textures:
                    return typeof(Texture);
                case AssetType.Audio:
                    return typeof(AudioClip);
                case AssetType.Particle:
                    return typeof(ParticleSystem);
                case AssetType.Prefab:
                    return typeof(GameObject);
                case AssetType.Shader:
                    return typeof(Shader);
                case AssetType.Sprite:
                    return typeof(Sprite);
                case AssetType.SkyBox:
                    return typeof(Skybox);
                default:
                    return null;
            }
        }
    }
}