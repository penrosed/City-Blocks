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
    private EntityManager _entityManager;

    void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PrimitivePrefab>();
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    void OnUpdate(ref SystemState state) 
    {
        state.Enabled = false;

        var primitiveBuffer = SystemAPI.GetSingletonBuffer<PrimitivePrefab>(true);

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (transform, palette, layout, root)
            in SystemAPI.Query<RefRW<LocalTransform>, DynamicBuffer<PropPalette>, DynamicBuffer<PropInstance>>().WithEntityAccess())
        {
            foreach (var datum in layout)
            {
                var propRoot = ecb.CreateEntity();
                quaternion propRotation = quaternion.identity;
                if (!datum.transform.rotation.Equals(float3.zero))
                {
                    propRotation = math.mul(propRotation, quaternion.RotateX(math.radians(datum.transform.rotation.x)));
                    propRotation = math.mul(propRotation, quaternion.RotateY(math.radians(datum.transform.rotation.y)));
                    propRotation = math.mul(propRotation, quaternion.RotateZ(math.radians(datum.transform.rotation.z)));
                }
                ecb.AddComponent(propRoot, LocalTransform.FromPositionRotation(datum.transform.position, propRotation));
                ecb.AddComponent(propRoot, new Parent { Value = root });
                ecb.AddComponent(propRoot, new LocalToWorld { Value = float4x4.identity });
                ecb.SetName(propRoot, _entityManager.GetName(palette[datum.index]));

                var propBuffer = SystemAPI.GetBuffer<Primitive>(palette[datum.index]);
                foreach (var primitive in propBuffer)
                {
                    var newPrim = ecb.Instantiate(primitiveBuffer[primitive.type]);
                    ecb.AddComponent(newPrim, new Parent { Value = propRoot });

                    quaternion primRotation = quaternion.identity;
                    if (!primitive.transform.rotation.Equals(float3.zero))
                    {
                        Debug.Log(primitive.type);
                        primRotation = math.mul(primRotation, quaternion.RotateX(math.radians(primitive.transform.rotation.x)));
                        primRotation = math.mul(primRotation, quaternion.RotateY(math.radians(primitive.transform.rotation.y)));
                        primRotation = math.mul(primRotation, quaternion.RotateZ(math.radians(primitive.transform.rotation.z)));
                    }
                    ecb.SetComponent(newPrim, LocalTransform.FromPositionRotation(primitive.transform.position, primRotation));

                    ecb.AddComponent(newPrim, new PostTransformMatrix
                    {
                        Value = float4x4.Scale(primitive.transform.scale.x, primitive.transform.scale.y, primitive.transform.scale.z)
                    });

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

        ecb.Playback(_entityManager);
    }
}
