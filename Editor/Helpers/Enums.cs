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
        //TexturesAudioParticle = Textures | Audio| Particle,
        //TexturesAudioPrefab = Textures | Audio | Prefab,
        //TexturesAudio = Textures | Audio,
        //TexturesPrefab = Textures | Prefab,
        //TexturesParticle = Textures | Particle,
        //AudioParticle =   Audio| Particle,
        //AudioPrefab = Audio | Prefab,
        //ParticlePrefab = Particle | Prefab,

        All = ~0,
    }
}