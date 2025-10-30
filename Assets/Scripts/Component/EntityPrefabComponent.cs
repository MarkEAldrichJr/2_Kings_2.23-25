﻿using Unity.Entities;

namespace Component
{
    /// <summary>
    /// Store Baked entity prefabs to spawn later.
    /// </summary>
    public struct EntityPrefabComponent : IComponentData
    {
        public Entity ThirdPersonCharacter;
        public Entity ThirdPersonPlayer;
        public Entity OrbitCamera;
    }
}