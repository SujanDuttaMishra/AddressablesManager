using System;

namespace AddressableManager
{
    public enum AutoLoad { None, OnStart, OnAwake }
    public enum Unload { None, OnSceneChange, ExplicitCall, DoNotUnload }
    [Flags]
    public enum AssetType
    {
        None = 0,
        Textures = 1,
        Audio = 2,
        Particle = 4,
        Prefab = 8,
        Shader = 16,
        Sprite=32,
        SkyBox = 64,

        All = ~0,
    }
}