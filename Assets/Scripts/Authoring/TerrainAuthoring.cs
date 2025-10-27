using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using Unity.Physics.Hybrid;
using UnityEngine;
using Material = Unity.Physics.Material;
using TerrainCollider = Unity.Physics.TerrainCollider;

namespace Authoring
{
    public class TerrainAuthoring : MonoBehaviour
    {
        public PhysicsMaterialTemplate physicsTemplate;
    
        private class TerrainAuthoringBaker : Baker<TerrainAuthoring>
        {
            public override void Bake(TerrainAuthoring authoring)
            {
                var terrain = GetComponent<UnityEngine.Terrain>();

                DependsOn(terrain.terrainData);
                var data = terrain.terrainData;

                var resolution = data.heightmapResolution;
                var size = new int2(resolution, resolution);
                float3 scale = data.heightmapScale;

                var colliderHeights = new NativeArray<float>(resolution * resolution, Allocator.Temp);
                var terrainHeights = data.GetHeights(0, 0, resolution, resolution);

                for (var j = 0; j < size.y; j++)
                {
                    for (var i = 0; i < size.x; i++)
                    {
                        var h = terrainHeights[i, j];
                        colliderHeights[j + i * size.x] = h;
                    }
                }

                var template = authoring.physicsTemplate;

                var filter = new CollisionFilter
                {
                    BelongsTo = template.BelongsTo.Value,
                    CollidesWith = template.CollidesWith.Value
                };

                var material = new Material
                {
                    FrictionCombinePolicy = template.Friction.CombineMode,
                    RestitutionCombinePolicy = template.Restitution.CombineMode,
                    CustomTags = template.CustomTags.Value,
                    Friction = template.Friction.Value,
                    Restitution = template.Restitution.Value,
                    CollisionResponse = template.CollisionResponse,
                    EnableMassFactors = false,
                    EnableSurfaceVelocity = false
                };

                const TerrainCollider.CollisionMethod collisionMethod =
                    TerrainCollider.CollisionMethod.Triangles;

                var collider = new PhysicsCollider
                {
                    Value = TerrainCollider.Create(colliderHeights, size, scale, collisionMethod,
                        filter, material)
                };
            
                AddBlobAsset(ref collider.Value, out _);

                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, collider);
                AddSharedComponent(entity, new PhysicsWorldIndex());
                AddBuffer<PhysicsColliderKeyEntityPair>(entity);
                //removed GenerateMesh script because it currently isn't useful
                colliderHeights.Dispose();
            }
        }
    }
}
