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
            var strings = Enum.GetNames(typeof(AssetType));
            if (!GetExcludeValues(excludeType, out var type)) return stringList;
            for (var i = 0; i < strings.Length; i++)
            {
                Enum.TryParse(strings[i], true, out AssetType value);
                if (value == AssetType.All || value == AssetType.None) continue;
                if (excludeType.HasFlag(value)) stringList = Exclude(stringList, type);
                if (type == typeof(GameObject) && excludeType.HasFlag(AssetType.Particle)) stringList = Exclude(stringList);
            }
            return stringList;
        }
        private static List<string> Exclude(IEnumerable<string> stringList, Type type) => stringList.Where(o => AssetDatabase.LoadAssetAtPath(o, type) == null).ToList();

        private static List<string> Exclude(IEnumerable<string> stringList) => stringList.Where(o => LoadAssetAtPath(o).GetComponentInChildren<ParticleSystem>() == null ).ToList();

        private static GameObject LoadAssetAtPath(string o) => (GameObject)AssetDatabase.LoadAssetAtPath(o,typeof(GameObject));

        private static bool GetExcludeValues(AssetType excludeType, out Type type)
        {
            type = null;
            switch (excludeType)
            {
                case AssetType.Textures: type = typeof(Texture); break;
                case AssetType.Audio: type = typeof(AudioClip); break;
                case AssetType.Particle: type = typeof(ParticleSystem); break;
                case AssetType.Prefab: type = typeof(GameObject); break;
            }

            return type!= null;
        }

    }
}