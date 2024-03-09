using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine;
using Unity.Rendering;
using Unity.Burst;

public partial struct PrimitiveSpawnSystem : ISystem
{
    [BurstCompile]
    void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PrimitivePrefab>();
    }

    [BurstCompile]
    void OnUpdate(ref SystemState state) 
    {
        state.Enabled = false;

        var primitiveBuffer = SystemAPI.GetSingletonBuffer<PrimitivePrefab>(true);
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (transform, palette, layout, root)
            in SystemAPI.Query<RefRW<LocalTransform>, DynamicBuffer<PropPalette>, DynamicBuffer<PropInstance>>().WithEntityAccess())
        {
            foreach (var datum in layout)
            {
                var propRoot = ecb.CreateEntity();
                ecb.AddComponent(propRoot, LocalTransform.FromPosition(datum.transform.position));
                ecb.AddComponent(propRoot, new Parent { Value = root });
                ecb.AddComponent(propRoot, new LocalToWorld { Value = float4x4.identity });
                ecb.SetName(propRoot, entityManager.GetName(palette[datum.index]));

                var propBuffer = SystemAPI.GetBuffer<Primitive>(palette[datum.index]);
                foreach (var primitive in propBuffer)
                {
                    var newPrim = ecb.Instantiate(primitiveBuffer[primitive.type]);
                    ecb.AddComponent(newPrim, new Parent { Value = propRoot });
                    ecb.SetComponent(newPrim, LocalTransform.FromPosition(primitive.transform.position));

                    var colourQuery = SystemAPI.QueryBuilder().WithAll<URPMaterialPropertyBaseColor>().Build();
                    var colourQueryMask = colourQuery.GetEntityQueryMask();
                    ecb.SetComponentForLinkedEntityGroup(newPrim, colourQueryMask, new URPMaterialPropertyBaseColor
                    {
                        Value = new float4
                        {
                            x = primitive.colour.r,
                            y = primitive.colour.g,
                            z = primitive.colour.b,
                            w = primitive.colour.a
                        }
                    });
                }
            }
        }

        ecb.Playback(entityManager);
    }
}
