using System;

namespace AddressableManager
{
    public enum AutoLoad { None, OnStart, OnAwake }
    public enum Unload { None, OnSceneChange, ExplicitCall, DoNotUnload }
    [Flags]
    public enum AssetType
    {
        None = 0,
        Textures = 1 << 0,
        Audio = 1 << 1,
        Particle = 1 << 2,
        Prefab = 1 << 3,

        All = ~0,
    }
}